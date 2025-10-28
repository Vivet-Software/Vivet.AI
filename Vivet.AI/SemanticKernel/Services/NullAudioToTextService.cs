using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;

namespace Vivet.AI.SemanticKernel.Services;

/// <inheritdoc />
public class NullAudioToTextService : IAudioToTextService
{
    /// <inheritdoc />
    public virtual IReadOnlyDictionary<string, object> Attributes { get; } = new Dictionary<string, object>();

    /// <inheritdoc />
    public Task<IReadOnlyList<TextContent>> GetTextContentsAsync(AudioContent content, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}