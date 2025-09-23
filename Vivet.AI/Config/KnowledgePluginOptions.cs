using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Knowledge Plugin Options (nested class).
/// </summary>
public class KnowledgePluginOptions
{
    /// <summary>
    /// Specifies the maximum number of results to return when searching for embeddings.
    /// Note: The vector store retrieves twice this number to ensure sufficient context after duplicate entries are removed.  
    /// Make sure the limit is set high enough when index-time deduplication is enabled.
    /// </summary>
    [Required]
    public virtual int ContextQueryLimit { get; set; } = 3;

    /// <summary>
    /// Whether to deduplicate results before building the knoweldge context for the chat prompt.
    /// Deduplication will remove similar results, that has a 95+ similary score for Fuzzy comparison.
    /// </summary>
    [Required]
    public virtual bool UseQueryDeduplication { get; set; } = true;

    /// <summary>
    /// The matchs score threshold for deduplicating similar knowledge results,
    /// when building the knowledge part of the chat prompt.
    /// </summary>
    public virtual double DeduplicationMatchScoreThreshold { get; set; } = 0.90;
}