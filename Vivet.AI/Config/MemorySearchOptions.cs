using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Options for memory querying.
/// </summary>
public class MemorySearchOptions : BaseEmbeddingSearchOptions<MemoryScoringOptions>
{
    /// <summary>
    /// The maximum number of results to return when searching for counterpart vector matches of questions and answers.
    /// </summary>
    [Required]
    public virtual int CounterpartContextQueryLimit { get; set; } = 2;

    /// <summary>
    /// How far back memories will be included in queries when chatting.
    /// </summary>
    [Required]
    public virtual int RetentionInDays { get; set; } = 180;
}