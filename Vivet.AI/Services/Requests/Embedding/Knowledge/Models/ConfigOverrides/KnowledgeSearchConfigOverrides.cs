using Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to knowledge query operations.
/// </summary>
public class KnowledgeSearchConfigOverrides : BaseSearchConfigOverrides<KnowledgeScoringConfigOverrides>;