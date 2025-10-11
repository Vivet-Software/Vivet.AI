using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Embedding Knowledge Options.
/// </summary>
public class EmbeddingKnowledgeOptions
{
    /// <summary>
    /// Options for indexing.
    /// </summary>
    [Required]
    public virtual KnowledgeIndexOptions Indexing { get; set; } = new();

    /// <summary>
    /// Options for searching.
    /// </summary>
    [Required]
    public virtual KnowledgeSearchOptions Search { get; set; } = new();

    /// <summary>
    /// Vector store configuration.
    /// </summary>
    [Required]
    public virtual VectorStoreOptions VectorStore { get; set; }
}