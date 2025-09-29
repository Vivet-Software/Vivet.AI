using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Summarization;
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

        SummarizationMemoryResponse response;
        if (summarizationDegree > 0)
        {
            var chatHistory = new ChatHistory();

            chatHistory
                .AddSummarizationMemoryPrompt(request.Question, request.Answer, summarizationDegree);

            var executionSettings = this.promptExecutionSettings
                .GetOverridePromptExecutionSettings(request.ConfigOverrides.ModelParameters);

            executionSettings.ModelId = request.ConfigOverrides.ModelName;

            var kernel = kernelBuilder
                .Build();

            var chatMessageContent = await this.chatCompletionService
                .GetChatMessageContentAsync(chatHistory, executionSettings, kernel, cancellationToken)
                .ConfigureAwait(false);

            stopwatch
                .Stop();

            response = SummarizationService.GetResponse(chatMessageContent, stopwatch.Elapsed);
        }
        else
        {
            stopwatch
                .Stop();

            response = new SummarizationMemoryResponse
            {
                QuestionSummarized = request.Question,
                AnswerSummarized = request.Answer,
                ElapsedTime = stopwatch.Elapsed
            };
        }

        return response;
    }

    private static SummarizationMemoryResponse GetResponse(ChatMessageContent chatMessageContent, TimeSpan elapsedTime)
    {
        if (chatMessageContent == null) 
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (string.IsNullOrEmpty(chatMessageContent.Content))
        {
            throw new AiException("No Content returned by the request.");
        }

        var answer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var jObject = JObject.Parse(answer);

        var errorMessage = jObject[nameof(BaseResponse.ErrorMessage)]?.ToString();

        if (errorMessage != null)
        {
            throw new AiException(errorMessage);
        }

        var questionSummarized = jObject[nameof(SummarizationMemoryResponse.QuestionSummarized)]?.ToString();
        var answerSummarized = jObject[nameof(SummarizationMemoryResponse.AnswerSummarized)]?.ToString();

        var tokenUsage = chatMessageContent
            .GetTokenUsage();

        var externalId = chatMessageContent
            .GetExternalId();

        return new SummarizationMemoryResponse
        {
            QuestionSummarized = questionSummarized,
            AnswerSummarized = answerSummarized,
            TokenUsage = tokenUsage,
            ExternalId = externalId,
            ElapsedTime = elapsedTime
        };
    }
}