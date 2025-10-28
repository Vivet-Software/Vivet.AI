using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;

namespace Vivet.AI.Extensions.Orchestration.AzureInferenceAi.Services;

/// <inheritdoc />
public class AzureAiInferenceImageToTextService : IImageToTextService
{
    /// <inheritdoc />
    public virtual IReadOnlyDictionary<string, object> Attributes { get; } = new Dictionary<string, object>();

    /// <inheritdoc />
    public Task<IReadOnlyList<TextContent>> GetTextContentsAsync(ImageContent content, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}