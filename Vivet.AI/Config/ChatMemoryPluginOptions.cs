using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Chat Memory Plugin Options (nested class).
/// </summary>
public class ChatMemoryPluginOptions
{
    /// <summary>
    /// How far back memories will be included in queries when chatting.
    /// </summary>
    [Required]
    public virtual int RetentionInDays { get; set; } = 180;

    /// <summary>
    /// Specifies the maximum number of results to return when searching for embeddings.
    /// Note: The vector store retrieves twice this number to ensure sufficient context after duplicate entries are removed.  
    /// Make sure the limit is set high enough when index-time deduplication is enabled.
    /// </summary>
    [Required]
    public virtual int ContextQueryLimit { get; set; } = 3;

    /// <summary>
    /// The maximum number of results to return when searching for counterpart vector matches of questions and answers.
    /// </summary>
    [Required]
    public virtual int CounterpartContextQueryLimit { get; set; } = 2;

    /// <summary>
    /// Whether to deduplicate results before building the memory context for the chat prompt.
    /// Deduplication will remove similar results, that has a 95+ similary score for Fuzzy comparison.
    /// </summary>
    [Required]
    public virtual bool UseQueryDeduplication { get; set; } = true;

    /// <summary>
    /// The matchs score threshold for deduplicating similar memory results,
    /// when building the memory part of the chat prompt.
    /// </summary>
    public virtual double DeduplicationMatchScoreThreshold { get; set; } = 0.90;
}