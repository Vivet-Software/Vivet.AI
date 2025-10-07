using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

// BUG: 111: Consider moving Embedding models to Embedding/models afterall (and rename them for simpler names)

/// <summary>
/// Represents configuration overrides specific to memory query operations.
/// </summary>
public class EmbeddingMemorySearchConfigOverrides : BaseEmbeddingSearchConfigOverrides<EmbeddingMemorySearchScoringConfigOverrides>
{
    /// <summary>
    /// The maximum number of results to return when searching for counterpart vector matches of questions and answers.
    /// </summary>
    public virtual int? CounterpartContextQueryLimit { get; set; }

    /// <summary>
    /// How far back memories will be included in queries when chatting.
    /// </summary>
    public virtual int? RetentionInDays { get; set; }
}