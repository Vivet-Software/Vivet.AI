using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Memory.Models;

/// <summary>
/// Represents configuration overrides specific to memory operations.
/// </summary>
public class MemoryConfigOverrides : EmbedingConfigOverrides
{
    /// <summary>
    /// Metadata retrieval overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingMetadataConfigOverrides Metadata { get; internal set; } = new();

    /// <summary>
    /// Summarization overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingSummarizationConfigOverrides Summarization { get; internal set; } = new();
}