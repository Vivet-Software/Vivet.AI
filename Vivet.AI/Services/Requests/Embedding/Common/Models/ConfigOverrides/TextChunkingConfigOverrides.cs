using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides for embedding index text chunking.
/// </summary>
public class TextChunkingConfigOverrides
{
    /// <summary>
    /// The minimum number of tokens for text chunks to get embedding.
    /// </summary>
    public virtual int? MinTokens { get; set; }

    /// <summary>
    /// The maximum number of tokens for text chunks to get embedding.
    /// Sentences will be merged together until reaching the max tokens.
    /// If a sentence is in itself is longer than max tokens, it will not be respected.
    /// </summary>
    public virtual int? MaxTokens { get; set; }

    /// <summary>
    /// Overrides for neighbor contexts.
    /// </summary>
    [Required]
    public virtual NeighborContextConfigOverrides NeighborContext { get; internal set; } = new();
}