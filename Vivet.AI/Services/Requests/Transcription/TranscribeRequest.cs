using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Transcription.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Transcription;

/// <summary>
/// Represents a transcribe request.
/// </summary>
public class TranscribeRequest
{
    /// <summary>
    /// Gets or sets the the audio blob to transcribe.
    /// </summary>
    [Required]
    public virtual AudioBlob Blob { get; set; }

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual TranscriptionConfigOverrides ConfigOverrides { get; } = new();
}