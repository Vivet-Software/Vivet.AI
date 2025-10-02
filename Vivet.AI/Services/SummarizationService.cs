using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Summarization;
using Vivet.AI.Services.Requests.Summarization.Models;
using Vivet.AI.Services.Responses;
using Vivet.AI.Services.Responses.Summarization;

namespace Vivet.AI.Services;

/// <inheritdoc cref="ISummarizationService"/>
public class SummarizationService(SummarizationOptions summarizationOptions, IChatCompletionService chatCompletionService, IKernelBuilder kernelBuilder, PromptExecutionSettings promptExecutionSettings) 
    : BaseService, ISummarizationService
{
    private readonly SummarizationOptions summarizationOptions = summarizationOptions ?? throw new ArgumentNullException(nameof(summarizationOptions));
    private readonly IChatCompletionService chatCompletionService = chatCompletionService ?? throw new ArgumentNullException(nameof(chatCompletionService));
    private readonly PromptExecutionSettings promptExecutionSettings = promptExecutionSettings ?? throw new ArgumentNullException(nameof(promptExecutionSettings));

    /// <inheritdoc />
    public async Task<SummarizationMemoryResponse> SummarizeMemoryAsync(SummarizeMemoryRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var summarizationDegree = request.ConfigOverrides.SummarizationDegree ?? this.summarizationOptions.SummarizationDegree;

        if (summarizationDegree > 0)
        {
            var kernel = this.GetKernel();
            var executionSettings = this.GetPromptExecutionSettings(request.ConfigOverrides);
            var chatHistory = this.GetChatHistory(request);

            var chatMessageContent = await this.chatCompletionService
                .GetChatMessageContentAsync(chatHistory, executionSettings, kernel, cancellationToken)
                .ConfigureAwait(false);

            stopwatch
                .Stop();

            return SummarizationService.GetResponse(chatMessageContent, stopwatch.Elapsed);
        }

        stopwatch
            .Stop();

        return new SummarizationMemoryResponse
        {
            QuestionSummarized = request.Question,
            AnswerSummarized = request.Answer,
            ElapsedTime = stopwatch.Elapsed
        };
    }


    private Kernel GetKernel()
    {
        var kernel = kernelBuilder
            .Build();

        return kernel;
    }
    private PromptExecutionSettings GetPromptExecutionSettings(SummarizationConfigOverrides configOverrides)
    {
        if (configOverrides == null)
            throw new NullReferenceException(nameof(configOverrides));

        var executionSettings = this.promptExecutionSettings
            .GetOverridePromptExecutionSettings(configOverrides.ModelParameters);

        executionSettings.ModelId = configOverrides.ModelName;

        return executionSettings;
    }
    private ChatHistory GetChatHistory(SummarizeMemoryRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var chatHistory = new ChatHistory();

        var summarizationDegree = request.ConfigOverrides.SummarizationDegree ?? this.summarizationOptions.SummarizationDegree;

        chatHistory
            .AddSummarizationMemoryPrompt(request.Question, request.Answer, summarizationDegree);

        return chatHistory;
    }

    private static SummarizationMemoryResponse GetResponse(ChatMessageContent chatMessageContent, TimeSpan elapsedTime)
    {
        if (chatMessageContent == null) 
            throw new ArgumentNullException(nameof(chatMessageContent));

        var tokenUsage = chatMessageContent
            .GetTokenUsage();

        var externalId = chatMessageContent
            .GetExternalId();

        if (string.IsNullOrEmpty(chatMessageContent.Content))
        {
            var noContentException = BaseService.GetResponseExceptionOrDefault("No Content returned by the request.");

            return new SummarizationMemoryResponse
            {
                ElapsedTime = elapsedTime,
                TokenUsage = tokenUsage,
                ExternalId = externalId,
                Exception = noContentException
            };
        }

        var answer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var jObject = JObject.Parse(answer);

        var errorMessage = jObject[nameof(BaseResponse.ErrorMessage)]?.ToString();
        var questionSummarized = jObject[nameof(SummarizationMemoryResponse.QuestionSummarized)]?.ToString();
        var answerSummarized = jObject[nameof(SummarizationMemoryResponse.AnswerSummarized)]?.ToString();

        var exception = BaseService.GetResponseExceptionOrDefault(errorMessage);

        return new SummarizationMemoryResponse
        {
            QuestionSummarized = questionSummarized,
            AnswerSummarized = answerSummarized,
            TokenUsage = tokenUsage,
            ExternalId = externalId,
            ElapsedTime = elapsedTime,
            Exception = exception
        };
    }
}