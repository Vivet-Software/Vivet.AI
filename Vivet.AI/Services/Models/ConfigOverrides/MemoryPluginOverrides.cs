using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to memory in chat operations.
/// </summary>
public class MemoryPluginOverrides
{
    /// <summary>
    /// Skips the memory invocaton and context in the prompt for this request.
    /// </summary>
    public virtual bool SkipMemoryContext { get; set; } = false;

    /// <summary>
    /// Skips saving memory context for the request.
    /// </summary>
    public virtual bool SkipSaveMemoryContext { get; set; } = false;

    /// <summary>
    /// Metadata retrieval overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingMetadataConfigOverrides Metadata { get; } = new();

    /// <summary>
    /// Summarization overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingSummarizationConfigOverrides Summarization { get; } = new();
}