using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Embedding Memory Options.
/// </summary>
public class EmbeddingMemoryOptions
{
    /// <summary>
    /// Options for searching.
    /// </summary>
    [Required]
    public virtual MemorySearchOptions Search { get; set; } = new();

    /// <summary>
    /// Options for indexing.
    /// </summary>
    [Required]
    public virtual MemoryIndexOptions Indexing { get; set; } = new();

    /// <summary>
    /// Vector store configuration.
    /// </summary>
    [Required]
    public virtual VectorStoreOptions VectorStore { get; set; }
}