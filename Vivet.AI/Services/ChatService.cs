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
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Chat;
using Vivet.AI.Services.Requests.Chat.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Requests.Embedding.Memory.Models;
using Vivet.AI.Services.Responses.Chat;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

/// <inheritdoc cref="IChatService"/>
public class ChatService(ChatOptions options, IChatCompletionService chatCompletionService, IKernelBuilder kernelBuilder, IServiceProvider serviceProvider, PromptExecutionSettings promptExecutionSettings, IEmbeddingMemoryService embeddingMemoryService = null) 
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

        request
            .Validate();

        var kernel = this.GetKernel(request);

        var chatHistory = await BuildChatHistory<T>(request, cancellationToken)
            .ConfigureAwait(false);

        var executionSettings = this.GetPromptExecutionSettingsOrDefault(request.ConfigOverrides);

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

        var inputPrompt = chatHistory
            .GetPromptAsText();

        var tokenUsage = chatMessageContent
            .GetTokenUsage();

        var externalId = chatMessageContent
            .GetExternalId();

        stopwatch
            .Stop();

        response.Thinking = thinking;
        response.RawResponse = chatMessageContent.Content;
        response.InputPrompt = inputPrompt;
        response.TokenUsage = tokenUsage;
        response.ExternalId = externalId;
        response.ElapsedTime = stopwatch.Elapsed;

        _ = this.SaveMemory(request, response, onMemoryIndexed, cancellationToken);

        return response;
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<string> ChatStreamingAsync(ChatRequest request, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, Func<ChatResponse, Task> onChatStreamingComplete = null, [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request
            .Validate();

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        var kernel = this.GetKernel(request);

        var chatHistory = await BuildChatHistory<string>(request, cancellationToken)
            .ConfigureAwait(false);

        var executionSettings = this.GetPromptExecutionSettingsOrDefault(request.ConfigOverrides);

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

        var inputPrompt = chatHistory
            .GetPromptAsText();

        stopwatch
            .Stop();

        response.Thinking = thinking;
        response.RawResponse = rawContent;
        response.InputPrompt = inputPrompt;
        response.ElapsedTime = stopwatch.Elapsed;
        // TODO: Chat Streaming Token Usage / External Id (not possible through SK yet)
        response.TokenUsage = null; 
        response.ExternalId = null;

        _ = this.SaveMemory(request, response, onMemoryIndexed, cancellationToken)
            .ConfigureAwait(false);

        if (onChatStreamingComplete != null)
        {
            _ = Task.Run(() => onChatStreamingComplete
                .Invoke(response), cancellationToken);
        }
    }


    private static async Task<ChatHistory> BuildChatHistory<T>(ChatRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var chatHistory = new ChatHistory();
        chatHistory
            .AddChatSystemPrompt<T>(request.SystemMessage)
            .AddBuiltInPluginsContextPrompt(request.Plugins.Context)
            .AddCustomPluginContextPrompt(request.Plugins.CustomPlugins);

        var binaryContents = await Task.WhenAll(request.Blobs
                .Select(x => x
                    .GetBinaryContent(cancellationToken)))
            .ConfigureAwait(false);

        chatHistory
            .AddChatUserPrompt(request.Question, binaryContents);

        return chatHistory;
    }
    private PromptExecutionSettings GetPromptExecutionSettingsOrDefault(ChatConfigOverrides configOverrides)
    {
        if (configOverrides == null)
        {
            return this.promptExecutionSettings;
        }

        var executionSettings = this.promptExecutionSettings
            .GetOverridePromptExecutionSettings(configOverrides.ModelParameters);

        executionSettings.ModelId = configOverrides.ModelName;

        return executionSettings;
    }
    private Kernel GetKernel(ChatRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var kernel = kernelBuilder
            .Build();

        kernel.Plugins
            .ValidateContext(request.Plugins.Context);

        kernel
            .AddPluginConfigOverridesOrDefault(request.ConfigOverrides)
            .AddCustomPlugins(serviceProvider, request.Plugins.CustomPlugins);

        return kernel;
    }
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

        if (request.ConfigOverrides?.Plugins?.Memory is { SkipMemoryContext: true })
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
                        UserId = request.Plugins.Context.Memory.UserId,
                        ThreadId = request.Plugins.Context.Memory.CurrentThreadId,
                        ScopeId = request.Plugins.Context.Memory.ScopeId,
                        Language = response.Language,
                        Blobs = request.Blobs,
                        ConfigOverrides = new MemoryConfigOverrides
                        {
                            Metadata = request.ConfigOverrides?.Plugins?.Memory?.Metadata,
                            Summarization = request.ConfigOverrides?.Plugins?.Memory?.Summarization
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