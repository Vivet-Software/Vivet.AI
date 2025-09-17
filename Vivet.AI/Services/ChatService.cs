using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Plugins;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Chat;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Responses.Chat;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

// BUG: Observability: https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/?pivots=programming-language-csharp
// - https://ai.azure.com/observability/applicationAnalytics?wsid=/subscriptions/b2b7a8a7-862e-478a-8761-58e41e726855/resourceGroups/AI/providers/Microsoft.CognitiveServices/accounts/vivet-software/projects/vivet-ai&tid=9071a89e-4c58-4163-9bb4-f87488ff1427
// - https://ai.azure.com/tracing?wsid=/subscriptions/b2b7a8a7-862e-478a-8761-58e41e726855/resourceGroups/AI/providers/Microsoft.CognitiveServices/accounts/vivet-software/projects/vivet-ai&tid=9071a89e-4c58-4163-9bb4-f87488ff1427

// BUG: Finish Readme 
// - Finish Plugins(consider making Plugins section with Built-in and custom underneath)
// - Filters
//   - invocation works like middleware
//   - Add them to IServiceCollection in the desired order.It will work, because they are all transfered to the Kernel in the correct order.

/// <inheritdoc cref="IChatService"/>
public class ChatService(ChatOptions options, IChatCompletionService chatCompletionService, IKernelBuilder kernelBuilder, PromptExecutionSettings promptExecutionSettings, IEmbeddingMemoryService embeddingMemoryService = null) 
    : BaseService, IChatService
{
    private readonly ChatOptions options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly IChatCompletionService chatCompletionService = chatCompletionService ?? throw new ArgumentNullException(nameof(chatCompletionService));
    private readonly PromptExecutionSettings promptExecutionSettings = promptExecutionSettings ?? throw new ArgumentNullException(nameof(promptExecutionSettings));

    /// <inheritdoc />
    public virtual async Task<ChatResponse> ChatAsync(ChatRequest request, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default)
    {
        var response = await this.ChatAsync<string>(request, onMemoryIndexed, cancellationToken)
            .ConfigureAwait(false);

        return new ChatResponse
        {
            Answer = response.Answer,
            Reasoning = response.Reasoning,
            Thinking = response.Thinking,
            InputPrompt = response.InputPrompt,
            ElapsedTime = response.ElapsedTime,
            RawResponse = response.RawResponse,
            Language = response.Language,
            TokenUsage = response.TokenUsage,
            ExternalId = response.ExternalId
        };
    }

    /// <inheritdoc />
    public virtual async Task<ChatResponse<T>> ChatAsync<T>(ChatRequest request, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default) 
        where T : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        var chatHistory = await this.BuildChatHistory<T>(request, cancellationToken)
            .ConfigureAwait(false);

        var executionSettings = this.GetPromptExecutionSettings(request);

        var kernel = this.GetKernel(request);

        var chatMessageContent = await this.chatCompletionService
            .GetChatMessageContentAsync(chatHistory, executionSettings, kernel, cancellationToken)
            .ConfigureAwait(false);
 
        if (chatMessageContent.Content == null)
        {
            return null;
        }

        var answer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var response = ChatService.GetResponseOrDefault<T>(answer);

        if (response == null)
        {
            return null;
        }

        var thinking = chatMessageContent.Content
            .GetChatResponseThinking();

        response.Thinking = thinking;
        response.RawResponse = chatMessageContent.Content;
        response.ElapsedTime = stopwatch.Elapsed;
        response.InputPrompt = chatHistory
            .GetPromptAsText();
        response.TokenUsage = chatMessageContent
            .GetTokenUsage();
        response.ExternalId = chatMessageContent
            .GetExternalId();

        stopwatch
            .Stop();

        _ = this.SaveMemory(request, response, onMemoryIndexed, cancellationToken);

        return response;
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<string> ChatStreamingAsync(ChatRequest request, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, Func<ChatResponse, Task> onChatStreamingComplete = null, [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        var chatHistory = await this.BuildChatHistory<string>(request, cancellationToken)
            .ConfigureAwait(false);

        var executionSettings = this.GetPromptExecutionSettings(request);

        var kernel = this.GetKernel(request);

        var streamingChatMessageContents = this.chatCompletionService
            .GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);

        var contentString = new StringBuilder();
        await foreach (var streamingChatMessageContent in streamingChatMessageContents.ConfigureAwait(false))
        {
            if (!string.IsNullOrEmpty(streamingChatMessageContent.Content))
            {
                contentString
                    .Append(streamingChatMessageContent.Content);

                yield return streamingChatMessageContent.Content;
            }
        }

        var rawContent = contentString
            .ToString();

        var answer = rawContent
            .GetChatResponseAnswer();

        var response = ChatService.GetResponseOrDefault(answer);

        var thinking = rawContent
            .GetChatResponseThinking();

        response.Thinking = thinking;
        response.RawResponse = rawContent;
        response.ElapsedTime = stopwatch.Elapsed;
        response.InputPrompt = chatHistory
            .GetPromptAsText();

        // TODO: Chat Streaming Token Usage / External Id (not possible through SK yet)
        response.TokenUsage = null; 
        response.ExternalId = null;
        
        stopwatch
            .Stop();

        _ = this.SaveMemory(request, response, onMemoryIndexed, cancellationToken)
            .ConfigureAwait(false);

        if (onChatStreamingComplete != null)
        {
            _ = Task.Run(() => onChatStreamingComplete
                .Invoke(response), cancellationToken);
        }
    }


    private async Task<ChatHistory> BuildChatHistory<T>(ChatRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // TODO: Remove
        //var knowledgeTask = this.GetMatchingKnowledges(request, cancellationToken);
        //var memoriesTask = this.GetMatchingMemories(request, cancellationToken);

        //var knowledgeResults = await knowledgeTask.ConfigureAwait(false);
        //var memoryResults = await memoriesTask.ConfigureAwait(false);

        var chatHistory = new ChatHistory();
        chatHistory
            .AddChatSystemPrompt<T>(request.SystemMessage)
            .AddChatPluginContextPrompt(request);
        //.AddChatKnowledgePrompt(knowledgeResults)
        //.AddChatMemoryPrompt(memoryResults, this.options.Memory.CounterpartContextQueryLimit) 

        var dataUris = await Task.WhenAll(request.Blobs
                .Select(x => x
                    .GetBinaryContent(cancellationToken)))
            .ConfigureAwait(false);

        chatHistory
            .AddChatUserPrompt(request.Question, dataUris);

        return chatHistory;
    }
    private PromptExecutionSettings GetPromptExecutionSettings(ChatRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var executionSettings = this.promptExecutionSettings
            .GetOverridePromptExecutionSettings(request.ConfigOverrides.ModelParameters);

        executionSettings.ModelId = request.ConfigOverrides.ModelName;

        return executionSettings;
    }
    private Kernel GetKernel(ChatRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var kernel = kernelBuilder
            .Build();

        if (request.ConfigOverrides.Memory.SkipMemoryContext)
        {
            var chatMemoryPlugin = kernel.Plugins
                .FirstOrDefault(x => x.Name == nameof(ChatMemoryPlugin));

            if (chatMemoryPlugin != null)
            {
                kernel.Plugins
                    .Remove(chatMemoryPlugin);
            }
        }

        if (request.ConfigOverrides.Knowledge.SkipKnowledgeContext)
        {
            var chatKnowledgePlugin = kernel.Plugins
                .FirstOrDefault(x => x.Name == nameof(ChatKnowledgePlugin));

            if (chatKnowledgePlugin != null)
            {
                kernel.Plugins
                    .Remove(chatKnowledgePlugin);
            }
        }

        foreach (var requestPlguin in request.Plugins)
        {
            kernel.Plugins
                .AddFromObject(requestPlguin);
        }

        return kernel;
    }
    // TODO: Remove
    //private async Task<MemoryResult[]> GetMatchingMemories(ChatRequest request, CancellationToken cancellationToken = default)
    //{
    //    if (request == null)
    //        throw new ArgumentNullException(nameof(request));

    //    if (embeddingMemoryService == null)
    //    {
    //        return [];
    //    }

    //    if (request.ConfigOverrides.Memory.SkipMemoryContext)
    //    {
    //        return [];
    //    }

    //    var from = DateTimeOffset.UtcNow
    //        .AddDays(-this.options.Memory.RetentionInDays);

    //    var limit = this.options.Memory.UseQueryDeduplication
    //        ? this.options.Memory.ContextQueryLimit * 2 
    //        : this.options.Memory.ContextQueryLimit;

    //    var response = await embeddingMemoryService
    //        .SearchAsync(new SearchMemoryRequest
    //        {
    //            Query = request.Question,
    //            Criteria =
    //            {
    //                UserId = request.UserId,
    //                ScopeId = request.ScopeId,
    //                AgentId = request.AgentId,
    //                DateRange = new DateRange
    //                {
    //                    From = from
    //                }
    //            },
    //            CurrentThreadId = request.CurrentThreadId,
    //            Limit = limit
    //        }, cancellationToken)
    //        .ConfigureAwait(false);

    //    var results = response.Results
    //        .Select(x => x.Result)
    //        .ToArray();

    //    if (this.options.Memory.UseQueryDeduplication)
    //    {
    //        var deduplicatedResults = ContextDeduplicator.DeduplicateMemoryResults(results, this.options.Memory.DeduplicationMatchScoreThreshold);

    //        return deduplicatedResults
    //            .Take(this.options.Memory.ContextQueryLimit)
    //            .ToArray();
    //    }

    //    return results;
    //}
    //private async Task<KnowledgeResult[]> GetMatchingKnowledges(ChatRequest request, CancellationToken cancellationToken = default)
    //{
    //    if (request == null)
    //        throw new ArgumentNullException(nameof(request));

    //    if (embeddingKnowledgeService == null)
    //    {
    //        return [];
    //    }

    //    if (request.ConfigOverrides.Knowledge.SkipKnowledgeContext)
    //    {
    //        return [];
    //    }

    //    var limit = this.options.Knowledge.UseQueryDeduplication
    //        ? this.options.Knowledge.ContextQueryLimit * 2
    //        : this.options.Knowledge.ContextQueryLimit;

    //    var response = await embeddingKnowledgeService
    //        .SearchAsync(new SearchKnowledgeRequest
    //        {
    //            Query = request.Question,
    //            Criteria =
    //            {
    //                TenantId = request.TenantId,
    //                SubTenantId = request.SubTenantId,
    //                ScopeId = request.ScopeId,
    //                UserId = request.UserId
    //            },
    //            Limit = limit
    //        }, cancellationToken)
    //        .ConfigureAwait(false);

    //    var results = response.Results
    //        .Select(x => x.Result)
    //        .ToArray();

    //    if (this.options.Knowledge.UseQueryDeduplication)
    //    {
    //        var deduplicatedResults = ContextDeduplicator.DeduplicateKnowledgeResults(results, this.options.Knowledge.DeduplicationMatchScoreThreshold);

    //        return deduplicatedResults
    //            .Take(this.options.Knowledge.ContextQueryLimit)
    //            .ToArray();
    //    }

    //    return results;
    //}
    private static ChatResponse GetResponseOrDefault(string answer)
    {
        if (answer == null)
        {
            return null;
        }

        var response = JsonConvert.DeserializeObject<ChatResponse>(answer, Settings.ResponseSerializerSettings);

        if (response.ErrorMessage != null)
        {
            throw new AiException(response.ErrorMessage);
        }

        response.Answer = answer;

        return response;
    }
    private static ChatResponse<T> GetResponseOrDefault<T>(string answer)
        where T : class
    {
        if (answer == null)
        {
            return null;
        }

        var response = JsonConvert.DeserializeObject<ChatResponse<T>>(answer, Settings.ResponseSerializerSettings);

        if (response.ErrorMessage != null)
        {
            throw new AiException(response.ErrorMessage);
        }

        var responseType = response
            .GetType();

        var jObject = JsonConvert.DeserializeObject<JObject>(answer);

        if (response.ErrorMessage != null)
        {
            throw new AiException(response.ErrorMessage);
        }

        var answerToken = jObject[nameof(ChatResponse.Answer)];

        if (response is ChatResponse<string> stringResponse)
        {
            if (answerToken != null)
            {
                stringResponse.Answer = answerToken.Type == JTokenType.String
                    ? answerToken.Value<string>()
                    : answerToken.ToString();
            }
        }
        else if (responseType is { IsGenericType: true } && responseType.GetGenericTypeDefinition() == typeof(ChatResponse<>))
        {
            if (answerToken != null)
            {
                var answerType = responseType.GetGenericArguments()[0];

                var answerValue = answerToken.Type == JTokenType.String
                    ? JsonConvert.DeserializeObject(answerToken.Value<string>(), answerType)
                    : answerToken.ToObject(answerType);

                var propertyInfo = responseType
                    .GetProperty(nameof(ChatResponse<object>.Answer));

                propertyInfo?
                    .SetValue(response, answerValue);
            }
        }

        return response;
    }
    private Task SaveMemory<T>(ChatRequest request, ChatResponse<T> response, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default)
        where T : class
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        if (response == null) 
            throw new ArgumentNullException(nameof(response));
        
        if (embeddingMemoryService == null)
        {
            return Task.CompletedTask;
        }

        if (request.ConfigOverrides.Memory.SkipSaveMemoryContext)
        {
            return Task.CompletedTask;
        }

        return Task.Run(async () =>
        {
            ChatIndexMemoryResponse chatIndexMemoryResponse;
            try
            {
                var indexMemoryResponse = await embeddingMemoryService
                    .IndexAsync(new IndexMemoryRequest<T>
                    {
                        Question = request.Question,
                        Answer = response.Answer,
                        UserId = request.UserId,
                        ThreadId = request.CurrentThreadId,
                        Language = response.Language,
                        Blobs = request.Blobs,
                        ConfigOverrides =
                        {
                            Metadata = request.ConfigOverrides.Memory.Metadata,
                            Summarization = request.ConfigOverrides.Memory.Summarization
                        }
                    }, cancellationToken)
                    .ConfigureAwait(false);

                chatIndexMemoryResponse = new ChatIndexMemoryResponse
                {
                    Result = indexMemoryResponse
                };
            }
            catch (Exception ex)
            {
                chatIndexMemoryResponse = new ChatIndexMemoryResponse
                {
                    Exception = ex
                };
            }

            if (onMemoryIndexed != null)
            {
                await onMemoryIndexed(chatIndexMemoryResponse)
                    .ConfigureAwait(false);
            }
        }, cancellationToken);
    }
}