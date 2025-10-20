using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.ImageToText;
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
using Vivet.AI.Services.Consts;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Chat;
using Vivet.AI.Services.Requests.Chat.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Responses.Chat;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

// TODO: Text to Audio (Speech streaming)

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
            FunctionCalls = response.FunctionCalls,
            TokenUsage = response.TokenUsage,
            ExternalId = response.ExternalId,
            Exception = response.Exception
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
        var executionSettings = this.GetPromptExecutionSettings(request.ConfigOverrides);

        var chatHistory = await this.GetChatHistory<T>(request, kernel, cancellationToken)
            .ConfigureAwait(false);

        var chatMessageContent = await this.chatCompletionService
            .GetChatMessageContentAsync(chatHistory, executionSettings, kernel, cancellationToken)
            .ConfigureAwait(false);

        stopwatch
            .Stop();

        var response = ChatService.GetResponse<T>(chatMessageContent, chatHistory, kernel, stopwatch.Elapsed);

        _ = this.SaveMemory(request, response, onMemoryIndexed, cancellationToken)
            .ConfigureAwait(false);

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
        var executionSettings = this.GetPromptExecutionSettings(request.ConfigOverrides);

        var chatHistory = await GetChatHistory<string>(request, kernel, cancellationToken)
            .ConfigureAwait(false);

        var streamingChatMessageContents = this.chatCompletionService
            .GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken)
            .ConfigureAwait(false);

        var content = new StringBuilder();
        await foreach (var streamingChatMessageContent in streamingChatMessageContents)
        {
            if (!string.IsNullOrEmpty(streamingChatMessageContent.Content))
            {
                content
                    .Append(streamingChatMessageContent.Content);

                yield return streamingChatMessageContent.Content;
            }
        }

        stopwatch
            .Stop();

        var response = ChatService.GetResponse(content.ToString(), chatHistory, kernel, stopwatch.Elapsed);

        _ = this.SaveMemory(request, response, onMemoryIndexed, cancellationToken)
            .ConfigureAwait(false);

        if (onChatStreamingComplete != null)
        {
            _ = Task.Run(() => onChatStreamingComplete
                .Invoke(response), cancellationToken);
        }
    }


    private Kernel GetKernel(ChatRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var kernel = kernelBuilder
            .Build();

        kernel.Data
            .Add(KernelData.FUNCTION_CALLS, new List<AutoFunctionInvocationContext>());

        kernel
            .AddDefaultFilters()
            .RemoveSkippedBuiltInPlugins(this.options.Plugins, request.ConfigOverrides)
            .AddCustomPlugins(serviceProvider, request.Plugins.CustomPlugins);

        kernel.Plugins
            .ValidateContext(request.Plugins.Context);

        return kernel;
    }
    private PromptExecutionSettings GetPromptExecutionSettings(ChatConfigOverrides configOverrides)
    {
        if (configOverrides == null)
            throw new NullReferenceException(nameof(configOverrides));

        var executionSettings = this.promptExecutionSettings
            .GetOverridePromptExecutionSettings(configOverrides.ModelParameters);

        executionSettings.ModelId = configOverrides.ModelName;

        return executionSettings;
    }
    private async Task<ChatHistory> GetChatHistory<T>(ChatRequest request, Kernel kernel, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var chatHistory = new ChatHistory();

        var binaryContents = await Task.WhenAll(request.Blobs
                .Select(x => x
                    .GetBinaryContent(cancellationToken)))
            .ConfigureAwait(false);

        chatHistory
            .AddChatSystemPrompt<T>(request.SystemMessage)
            .AddChatPluginsContextPrompt(kernel, request)
            .AddChatUserPrompt(request.Question, binaryContents);

        return chatHistory;
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

        if (!(request.ConfigOverrides.Plugins.Memory.EnableMemoryPlugin ?? this.options.Plugins.EnableMemoryPlugin))
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
                        ConfigOverrides = request.ConfigOverrides.Plugins.Memory.Indexing
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

    private static ChatResponse GetResponse(string rawContent, ChatHistory chatHistory, Kernel kernel, TimeSpan elapsedTime)
    {
        if (rawContent == null) 
            throw new ArgumentNullException(nameof(rawContent));

        if (chatHistory == null) 
            throw new ArgumentNullException(nameof(chatHistory));

        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        var inputPrompt = chatHistory
            .GetPromptAsText();

        if (string.IsNullOrEmpty(rawContent))
        {
            var noContentException = BaseService.GetResponseExceptionOrDefault("No Content returned by the request.");

            return new ChatResponse
            {
                InputPrompt = inputPrompt,
                ElapsedTime = elapsedTime,
                Exception = noContentException
            };
        }

        var responseAnswer = rawContent
            .GetChatResponseAnswer();

        var response = JsonConvert.DeserializeObject<ChatResponse>(responseAnswer, Settings.ResponseSerializerSettings);

        ChatService.SetResponseAnswer(response, responseAnswer);

        var thinking = rawContent
            .GetChatResponseThinking();

        var functionCalls = BaseService.GetResponseFunctionCalls(kernel);
        var exception = ChatService.GetResponseExceptionOrDefault(response.ErrorMessage);

        response.Thinking = thinking;
        response.RawResponse = rawContent;
        response.InputPrompt = inputPrompt;
        response.ElapsedTime = elapsedTime;
        // TODO: Chat Streaming Token Usage / External Id (not possible through SK yet)
        response.TokenUsage = null;
        response.ExternalId = null;
        response.FunctionCalls = functionCalls;
        response.Exception = exception;

        return response;
    }
    private static ChatResponse<T> GetResponse<T>(ChatMessageContent chatMessageContent, ChatHistory chatHistory, Kernel kernel, TimeSpan elapsedTime)
        where T : class
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (chatHistory == null) 
            throw new ArgumentNullException(nameof(chatHistory));

        if (kernel == null) 
            throw new ArgumentNullException(nameof(kernel));

        var inputPrompt = chatHistory
            .GetPromptAsText();

        var tokenUsage = chatMessageContent
            .GetTokenUsage();

        var externalId = chatMessageContent
            .GetExternalId();

        if (string.IsNullOrEmpty(chatMessageContent.Content))
        {
            var noContentException = BaseService.GetResponseExceptionOrDefault("No Content returned by the request.");

            return new ChatResponse<T>
            {
                InputPrompt = inputPrompt,
                ElapsedTime = elapsedTime,
                TokenUsage = tokenUsage,
                ExternalId = externalId,
                Exception = noContentException
            };
        }

        var responseAnswer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var response = JsonConvert.DeserializeObject<ChatResponse<T>>(responseAnswer, Settings.ResponseSerializerSettings);

        ChatService.SetResponseAnswer(response, responseAnswer);

        var thinking = chatMessageContent.Content
            .GetChatResponseThinking();

        var functionCalls = ChatService.GetResponseFunctionCalls(kernel);
        var exception = BaseService.GetResponseExceptionOrDefault(response.ErrorMessage);

        response.Thinking = thinking;
        response.RawResponse = chatMessageContent.Content;
        response.InputPrompt = inputPrompt;
        response.TokenUsage = tokenUsage;
        response.ExternalId = externalId;
        response.ElapsedTime = elapsedTime;
        response.FunctionCalls = functionCalls;
        response.Exception = exception;

        return response;
    }
    private static void SetResponseAnswer<T>(ChatResponse<T> response, string responseAnswer) 
        where T : class
    {
        if (response == null) 
            throw new ArgumentNullException(nameof(response));
        
        if (responseAnswer == null) 
            throw new ArgumentNullException(nameof(responseAnswer));
        
        var responseType = response
            .GetType();

        var jObject = JsonConvert.DeserializeObject<JObject>(responseAnswer);

        var answerToken = jObject[nameof(ChatResponse.Answer)];

        if (response is ChatResponse<string> stringResponse)
        {
            if (answerToken != null)
            {
                stringResponse.Answer = answerToken.Type == JTokenType.String
                    ? answerToken.Value<string>()
                    : answerToken.ToString();

                stringResponse.Answer = string.IsNullOrEmpty(stringResponse.Answer)
                    ? null
                    : stringResponse.Answer;
            }
        }
        else if (responseType is { IsGenericType: true } && responseType.GetGenericTypeDefinition() == typeof(ChatResponse<>))
        {
            if (answerToken != null)
            {
                var answerType = responseType
                    .GetGenericArguments()[0];

                var answerValue = answerToken.Type == JTokenType.String
                    ? JsonConvert.DeserializeObject(answerToken.Value<string>(), answerType)
                    : answerToken.ToObject(answerType);

                var propertyInfo = responseType
                    .GetProperty(nameof(ChatResponse<object>.Answer));

                propertyInfo?
                    .SetValue(response, answerValue);
            }
        }
    }
}