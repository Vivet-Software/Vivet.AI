using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Metadata;
using Vivet.AI.Services.Requests.Metadata.Models;
using Vivet.AI.Services.Responses.Metadata;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

/// <inheritdoc cref="IMetadataService"/>
public class MetadataService(MetadataOptions metadataOptions, IChatCompletionService chatCompletionService, IKernelBuilder kernelBuilder, PromptExecutionSettings promptExecutionSettings) 
    : BaseService, IMetadataService
{
    private readonly MetadataOptions metadataOptions = metadataOptions ?? throw new ArgumentNullException(nameof(metadataOptions));
    private readonly IChatCompletionService chatCompletionService = chatCompletionService ?? throw new ArgumentNullException(nameof(chatCompletionService));
    private readonly PromptExecutionSettings promptExecutionSettings = promptExecutionSettings ?? throw new ArgumentNullException(nameof(promptExecutionSettings));

    /// <inheritdoc />
    public virtual async Task<MetadataResponse> GetAsync(GetMetadataRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var response = await this.GetAsync<dynamic>(request, cancellationToken)
            .ConfigureAwait(false);

        return response;
    }

    /// <inheritdoc />
    public virtual async Task<MetadataResponse<T>> GetAsync<T>(GetMetadataRequest request, CancellationToken cancellationToken = default) 
        where T : class, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var kernel = this.GetKernel();
        var executionSettings = this.GetPromptExecutionSettings(request.ConfigOverrides);

        var chatHistory = await this.BuildChatHistory<T>(request, cancellationToken)
            .ConfigureAwait(false);

        var chatMessageContent = await this.chatCompletionService
            .GetChatMessageContentAsync(chatHistory, executionSettings, kernel, cancellationToken)
            .ConfigureAwait(false);

        stopwatch
            .Stop();

        var response = MetadataService.GetResponse<T>(chatMessageContent, stopwatch.Elapsed);

        return response;
    }

    private Kernel GetKernel()
    {
        var kernel = kernelBuilder
            .Build();

        return kernel;
    }
    private PromptExecutionSettings GetPromptExecutionSettings(MetadataConfigOverrides configOverrides)
    {
        if (configOverrides == null)
            throw new NullReferenceException(nameof(configOverrides));

        var executionSettings = this.promptExecutionSettings
            .GetOverridePromptExecutionSettings(configOverrides.ModelParameters);

        executionSettings.ModelId = configOverrides.ModelName;

        return executionSettings;
    }
    private async Task<ChatHistory> BuildChatHistory<T>(GetMetadataRequest request, CancellationToken cancellationToken = default) 
        where T : class, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var chatHistory = new ChatHistory();

        var blobContent = await request.Blob
            .GetBinaryContent(cancellationToken);

        var maxWordsSummary = request.ConfigOverrides.SummaryMaxWords ?? this.metadataOptions.SummaryMaxWords;
        var maxWordsDescription = request.ConfigOverrides.DescriptionMaxWords ?? this.metadataOptions.DescriptionMaxWords;

        chatHistory
            .AddMetadataPrompt<T>(blobContent, maxWordsSummary, maxWordsDescription);

        return chatHistory;
    }

    private static MetadataResponse<T> GetResponse<T>(ChatMessageContent chatMessageContent, TimeSpan elapsedTime)
        where T : class, new()
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

            return new MetadataResponse<T>
            {
                ElapsedTime = elapsedTime,
                TokenUsage = tokenUsage,
                ExternalId = externalId,
                Exception = noContentException
            };
        }

        var answer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var response = JsonConvert.DeserializeObject<MetadataResponse<T>>(answer, Settings.ResponseSerializerSettings);

        var exception = BaseService.GetResponseExceptionOrDefault(response.ErrorMessage);

        response.TokenUsage = tokenUsage;
        response.ExternalId = externalId;
        response.ElapsedTime = elapsedTime;
        response.Exception = exception;

        return response;
    }
}