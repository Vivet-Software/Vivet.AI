using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Requests.Summarization;
using Vivet.AI.Services.Responses.Summarization;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Vivet.AI.Services.Responses;
using Vivet.AI.Services.Exceptions;

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

        SummarizationMemoryResponse response;
        ChatMessageContent chatMessageContent = null;

        var summarizationDegree = request.ConfigOverrides.SummarizationDegree ?? this.summarizationOptions.SummarizationDegree;

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

            chatMessageContent = await this.chatCompletionService
                .GetChatMessageContentAsync(chatHistory, executionSettings, kernel, cancellationToken)
                .ConfigureAwait(false);

            var answer = chatMessageContent.Content
                .GetChatResponseAnswer();

            response = SummarizationService.GetResponseOrDefault(answer);

            if (response == null)
            {
                return null;
            }
        }
        else
        {
            response = new SummarizationMemoryResponse
            {
                QuestionSummarized = request.Question,
                AnswerSummarized = request.Answer
            };
        }

        stopwatch
            .Stop();

        response.ElapsedTime = stopwatch.Elapsed;
        response.TokenUsage = chatMessageContent?
            .GetTokenUsage();

        return response;
    }

    private static SummarizationMemoryResponse GetResponseOrDefault(string content)
    {
        if (content == null)
        {
            return null;
        }

        var jObject = JObject.Parse(content);

        var errorMessage = jObject[nameof(BaseResponse.ErrorMessage)]?.ToString();

        if (errorMessage != null)
        {
            throw new AiException(errorMessage);
        }

        var questionSummarized = jObject[nameof(SummarizationMemoryResponse.QuestionSummarized)]?.ToString();
        var answerSummarized = jObject[nameof(SummarizationMemoryResponse.AnswerSummarized)]?.ToString();

        return new SummarizationMemoryResponse
        {
            QuestionSummarized = questionSummarized,
            AnswerSummarized = answerSummarized
        };
    }
}