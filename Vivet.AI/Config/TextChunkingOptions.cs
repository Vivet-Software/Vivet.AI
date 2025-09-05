using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Options for text chunking.
/// </summary>
public class TextChunkingOptions
{
    /// <summary>
    /// The minimum number of tokens for text chunks to get embedding.
    /// </summary>
    [Required]
    public virtual int MinTokens { get; set; } = 20;

    /// <summary>
    /// The maximum number of tokens for text chunks to get embedding.
    /// Sentences will be merged together until reaching the max tokens.
    /// If a sentence is in itself is longer than max tokens, it will not be respected.
    /// </summary>
    [Required]
    public virtual int MaxTokens { get; set; } = 60;

    /// <summary>
    /// Options neighbor contexts.
    /// </summary>
    [Required]
    public virtual NeighborContextOptions NeighborContext { get; set; } = new();
}