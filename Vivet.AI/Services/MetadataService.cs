using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Metadata;
using Vivet.AI.Services.Responses.Metadata;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

/// <inheritdoc cref="IMetadataService"/>
public class MetadataService(MetadataOptions metadataOptions, IChatCompletionService chatCompletionService, PromptExecutionSettings promptExecutionSettings) 
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

        return new MetadataResponse
        {
            Metadata = response.Metadata,
            ElapsedTime = response.ElapsedTime,
            TokenUsage = response.TokenUsage,
            ErrorMessage = response.ErrorMessage
        };
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

        var chatHistory = new ChatHistory();

        var blobData = await request.Blob
            .GetBlobData()
            .ConfigureAwait(false);

        var maxWordsSummary = request.ConfigOverrides.SummaryMaxWords ?? this.metadataOptions.SummaryMaxWords;
        var maxWordsDescription = request.ConfigOverrides.DescriptionMaxWords ?? this.metadataOptions.DescriptionMaxWords;

        chatHistory
            .AddMetadataPrompt<T>(blobData.DataUri, maxWordsSummary, maxWordsDescription);

        var executionSettings = this.promptExecutionSettings
            .GetOverridePromptExecutionSettings(request.ConfigOverrides.ModelParameters);
        
        var chatMessageContent = await this.chatCompletionService
            .GetChatMessageContentAsync(chatHistory, executionSettings, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var answer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var response = MetadataService.GetResponseOrDefault<T>(answer);

        if (response == null)
        {
            return null;
        }

        stopwatch
            .Stop();

        response.ElapsedTime = stopwatch.Elapsed;
        response.TokenUsage = chatMessageContent
            .GetTokenUsage();

        return response;
    }


    private static MetadataResponse<T> GetResponseOrDefault<T>(string content)
        where T : class, new()
    {
        if (content == null)
        {
            return null;
        }

        var response = JsonConvert.DeserializeObject<MetadataResponse<T>>(content, Settings.ResponseSerializerSettings);

        if (response.ErrorMessage != null)
        {
            throw new AiException(response.ErrorMessage);
        }

        return response;
    }
}