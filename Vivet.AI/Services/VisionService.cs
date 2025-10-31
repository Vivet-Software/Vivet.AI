using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Vision;
using Vivet.AI.Services.Requests.Vision.Models.ConfigOverrides;
using Vivet.AI.Services.Responses.Vision;
using Vivet.AI.Services.Responses.Vision.Models;

namespace Vivet.AI.Services;

/// <inheritdoc />
public class VisionService(IImageToTextService imageToTextService, PromptExecutionSettings promptExecutionSettings) 
    : IVisionService
{
    /// <inheritdoc />
    public virtual async Task<TextResponse> ExtractText<T>(BaseTextExtractionRequest<T> request, CancellationToken cancellationToken = default)
        where T : BaseBlob
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        // TODO: VisionService: Document Extract Text

        var blobData = await request.Blob
            .GetBlobData(cancellationToken)
            .ConfigureAwait(false);

        var content = new ImageContent(blobData.DataUri);
        var executionSettings = this.GetPromptExecutionSettings(request.ConfigOverrides);
    
        var textContents = await imageToTextService
            .GetTextContentsAsync(content, executionSettings, null, cancellationToken);

        stopwatch
            .Stop();

        var tokenUsage = textContents
            .Aggregate(new TokenUsage(), (current, x) => current + x.GetTokenUsage());

        return new TextResponse
        {
            Texts = textContents
                .Select(x =>
                {
                    var language = x
                        .GetLanguage();

                    return new ExtractedText
                    {
                        Content = x.Text,
                        Language = language
                    };
                }),
            ElapsedTime = stopwatch.Elapsed,
            TokenUsage = tokenUsage
        };
    }

    /// <inheritdoc />
    public virtual async Task<ImagesResponse> ExtractImages<T>(BaseImagesExtractionRequest<T> request, CancellationToken cancellationToken = default)
        where T : BaseBlob
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // TODO: VisionService: Video Extract Images
        // TODO: VisionService: Document Extract Images

        await Task.CompletedTask;

        return new ImagesResponse();
    }


    private PromptExecutionSettings GetPromptExecutionSettings(VisionConfigOverrides configOverrides)
    {
        if (configOverrides == null)
            throw new NullReferenceException(nameof(configOverrides));

        promptExecutionSettings.ModelId = configOverrides.ModelName;

        return promptExecutionSettings;
    }
}