using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models;

/// <summary>
/// Configuration overrides specific to knowledge blobs.
/// </summary>
public class KnowledgeBlobConfigOverrides : KnowledgeConfigOverrides
{
    /// <summary>
    /// Summarization overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingMetadataConfigOverrides Metadata { get; set; } = new();
}