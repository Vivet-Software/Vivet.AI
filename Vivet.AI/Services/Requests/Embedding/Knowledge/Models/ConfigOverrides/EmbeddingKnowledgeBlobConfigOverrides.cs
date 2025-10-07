using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Metadata.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

/// <summary>
/// Configuration overrides specific to knowledge blobs.
/// </summary>
public class EmbeddingKnowledgeBlobConfigOverrides : EmbeddingKnowledgeIndexConfigOverrides
{
    /// <summary>
    /// Metadata overrides.
    /// </summary>
    [Required]
    public virtual MetadataConfigOverrides Metadata { get; internal set; } = new();
}