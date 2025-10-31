using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Transcription.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to transcription operations.
/// </summary>
public class TranscriptionConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the name of the model to use for this request, overriding the default configured model.
    /// The specified model must be supported by the registered orchestration; otherwise, the request may fail.
    /// </summary>
    public virtual string ModelName { get; set; }

    /// <summary>
    /// Whether to include word granularity in returned segments.
    /// </summary>
    public bool? IncludeWordGranularity { get; set; }
}