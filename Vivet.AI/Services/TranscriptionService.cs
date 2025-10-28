using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Transcription;
using Vivet.AI.Services.Requests.Transcription.Models.ConfigOverrides;
using Vivet.AI.Services.Responses.Transcription;
using Vivet.AI.Services.Responses.Transcription.Models;

namespace Vivet.AI.Services;

// BUG: Readme: TranscriptionService (Azure OpenAI)

/// <inheritdoc />
public class TranscriptionService(IAudioToTextService audioToTextService, PromptExecutionSettings promptExecutionSettings) : ITranscriptionService
{
    /// <inheritdoc />
    public virtual async Task<TranscribeResponse> Transcribe(TranscribeRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var blobData = await request.Blob
            .GetBlobData(cancellationToken)
            .ConfigureAwait(false);

        var content = new AudioContent(blobData.DataUri);
        var executionSettings = this.GetPromptExecutionSettings(request.ConfigOverrides);

        var textContents = await audioToTextService
            .GetTextContentsAsync(content, executionSettings, null, cancellationToken);

        stopwatch
            .Stop();

        return TranscriptionService.GetResponse(textContents, stopwatch.Elapsed);
    }


    private PromptExecutionSettings GetPromptExecutionSettings(TranscriptionConfigOverrides configOverrides)
    {
        if (configOverrides == null)
            throw new NullReferenceException(nameof(configOverrides));

        promptExecutionSettings.ModelId = configOverrides.ModelName;

        if (configOverrides.IncludeWordGranularity.HasValue)
        {
            var timestampGranularities = new List<string>
            {
                "segment"
            };

            if (configOverrides.IncludeWordGranularity.Value)
            {
                timestampGranularities
                    .Add("word");
            }

            promptExecutionSettings.ExtensionData = new Dictionary<string, object>
            {
                ["response_format"] = "verbose_json",
                ["timestamp_granularities"] = timestampGranularities
            };
        }

        return promptExecutionSettings;
    }

    private static TranscribeResponse GetResponse(IReadOnlyList<TextContent> textContents, TimeSpan elapsedTime)
    {
        if (textContents == null) 
            throw new ArgumentNullException(nameof(textContents));

        var tokenUsage = textContents
            .Aggregate(new TokenUsage(), (current, x) => current + x.GetTokenUsage());

        return new TranscribeResponse
        {
            Texts = textContents
                .Select(x =>
                {
                    var language = x
                        .GetLanguage();

                    var duration = x
                        .GetDuration();

                    var segments = x
                        .GetSegments();

                    return new TranscribedText
                    {
                        Content = x.Text,
                        StartTime = TimeSpan.Zero,
                        EndTime = duration.HasValue 
                            ? TimeSpan.FromMilliseconds(duration.Value.TotalMilliseconds) 
                            : null,
                        Language = language,
                        Duration = duration,
                        Segments = segments
                    };
                }),
            ElapsedTime = elapsedTime,
            TokenUsage = tokenUsage
        };
    }
}