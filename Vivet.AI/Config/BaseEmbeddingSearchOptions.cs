using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Base class for embedding search options.
/// </summary>
/// <typeparam name="T">The type scoring options.</typeparam>
public abstract class BaseEmbeddingSearchOptions<T>
    where T : BaseScoringOptions, new()
{
    /// <summary>
    /// Whether to deduplicate results before building the knoweldge context for the chat prompt.
    /// Deduplication will remove similar results, that has a 95+ similary score for Fuzzy comparison.
    /// </summary>
    [Required]
    public virtual bool UseQueryDeduplication { get; set; } = true;

    /// <summary>
    /// Specifies the maximum number of results to return when searching for embeddings.
    /// Note: The vector store retrieves twice this number to ensure sufficient context after duplicate entries are removed.  
    /// Make sure the limit is set high enough when index-time deduplication is enabled.
    /// </summary>
    [Required]
    public virtual int ContextQueryLimit { get; set; } = 3;

    /// <summary>
    /// Scoring configuration. 
    /// </summary>
    [Required]
    public virtual T Scoring { get; set; } = new();
}