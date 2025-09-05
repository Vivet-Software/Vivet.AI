using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Options for configuring how neighbor contexts behave.
/// </summary>
public class NeighborContextOptions
{
    /// <summary>
    /// The number of neighbors before and after and embedding that is stored as context alongside the vector.
    /// The optimal value depends on how large text chunks is being stored. The longer chunks the larger the neigbor context will also be.
    /// So the combination of <see cref="TextChunkingOptions.MaxTokens"/> and this will control how large chuncks are being embedded.
    /// 1 neighbors - Short, dense documents or chat-like inputs.
    /// 2 neighbors - Medium detail, e.g., product manuals, blog posts.
    /// 3 neighbors - Procedural docs, research papers, support knowledge bases.
    /// 5+ neighbors - Long-form narrative, legal docs, books, storytelling.
    /// </summary>
    [Required]
    public virtual int ContextWindow { get; set; } = 1;

    /// <summary>
    /// Whether to restrict the neighboring context to the same paragraph.
    /// </summary>
    [Required]
    public virtual bool RestrictToSameParagraph { get; set; } = true;
}