using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Embedding Knowledge Options.
/// </summary>
public class EmbeddingKnowledgeOptions
{
    /// <summary>
    /// Configuration for automatically to retrieve metadata for blobs when saving to knowledge.
    /// This will use the configured metadata chat model and incur costs.
    /// It's recommended to enable this, in order to ensure meaningful data for similarity comparison when the memory is later queried.
    /// If disabled metadata must be passed alongisde the blob when invoking the index request.
    /// </summary>
    [Required]
    public virtual bool UseAutomaticMetadataRetrieval { get; set; } = true;

    /// <summary>
    /// Options for querying.
    /// </summary>
    [Required]
    public virtual KnowledgeQueryOptions Search { get; set; } = new();

    /// <summary>
    /// Options for text chunking.
    /// </summary>
    [Required]
    public virtual TextChunkingOptions TextChunking { get; set; } = new();

    /// <summary>
    /// Vector store configuration.
    /// </summary>
    [Required]
    public virtual VectorStoreOptions VectorStore { get; set; }
}