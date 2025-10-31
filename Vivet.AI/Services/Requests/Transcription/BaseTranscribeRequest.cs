using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Transcription.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Transcription;

/// <summary>
/// Abstract base class representing a transcribe request.
/// </summary>
public abstract class BaseTranscribeRequest<T>
    where T : BaseBlob
{
    /// <summary>
    /// Gets or sets the the blob to transcribe.
    /// </summary>
    [Required]
    public virtual T Blob { get; set; }

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual TranscriptionConfigOverrides ConfigOverrides { get; } = new();
}