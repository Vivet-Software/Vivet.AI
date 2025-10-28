using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.ImageExtraction;
using Vivet.AI.Services.Requests.ImageExtraction.Models.ConfigOverrides;
using Vivet.AI.Services.Responses.ImageExtraction;
using Vivet.AI.Services.Responses.ImageExtraction.Models;

namespace Vivet.AI.Services;

// BUG: Readme: ImageExtractionService (HuggingFace)

// BUG: Document To Text ??? (Microsoft.SemanticKernel.Plugins.Document can this nuget be used)

/// <inheritdoc />
public class ImageExtractionService(IImageToTextService imageToTextService, PromptExecutionSettings promptExecutionSettings) 
    : IImageExtractionService
{
    /// <inheritdoc />
    public virtual async Task<ImageExtractionResponse> Extract(ImageExtractionRequest request, CancellationToken cancellationToken = default)
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

        var content = new ImageContent(blobData.DataUri);
        var executionSettings = this.GetPromptExecutionSettings(request.ConfigOverrides);

        // TODO: Image Extraction: Extract images from image
    
        var textContents = await imageToTextService
            .GetTextContentsAsync(content, executionSettings, null, cancellationToken);

        stopwatch
            .Stop();

        return ImageExtractionService.GetResponse(textContents, stopwatch.Elapsed);
    }


    private PromptExecutionSettings GetPromptExecutionSettings(ImageExtractionConfigOverrides configOverrides)
    {
        if (configOverrides == null)
            throw new NullReferenceException(nameof(configOverrides));

        promptExecutionSettings.ModelId = configOverrides.ModelName;

        return promptExecutionSettings;
    }

    private static ImageExtractionResponse GetResponse(IReadOnlyList<TextContent> textContents, TimeSpan elapsedTime)
    {
        if (textContents == null)
            throw new ArgumentNullException(nameof(textContents));

        var tokenUsage = textContents
            .Aggregate(new TokenUsage(), (current, x) => current + x.GetTokenUsage());

        return new ImageExtractionResponse
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
            ElapsedTime = elapsedTime,
            TokenUsage = tokenUsage
        };
    }
}