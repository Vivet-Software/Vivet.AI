using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

/// <summary>
/// Base class for controlling recency-based score boosts for search results.
/// Supports configurable decay strategies (Linear, Exponential, Sigmoid).
/// </summary>
public class EmbeddingMemorySearchScoringConfigOverrides : BaseEmbeddingSearchScoringConfigOverrides
{
    /// <summary>
    /// The score boost applied to memories that matches the current conversation thread.
    /// </summary>
    public virtual double? ThreadMatchBoost { get; set; }
}