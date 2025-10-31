using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Options for transcription.
/// </summary>
public class TranscriptionOptions
{
    /// <summary>
    /// The transcription model name.
    /// Make sure the model is configured in the choosen AI provider (e.g. Azure AI, Azure OpenAU, Ollama, etc).
    /// </summary>
    [Required]
    public virtual TranscriptionModel Model { get; set; } = new();

    /// <summary>
    /// Whether to include word granularity in returned segments.
    /// </summary>
    [Required]
    public bool IncludeWordGranularity { get; set; } = false;

    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
}