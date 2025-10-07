using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

/// <summary>
/// Base class for controlling recency-based score boosts for search results.
/// Supports configurable decay strategies (Linear, Exponential, Sigmoid).
/// </summary>
public class EmbeddingKnowledgeSearchScoringConfigOverrides : BaseEmbeddingSearchScoringConfigOverrides;