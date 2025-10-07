using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to knowledge query operations.
/// </summary>
public class EmbeddingKnowledgeSearchConfigOverrides : BaseEmbeddingSearchConfigOverrides<EmbeddingKnowledgeSearchScoringConfigOverrides>;