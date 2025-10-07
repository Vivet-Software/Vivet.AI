using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Base class for configuration of embedding search config overrides.
/// </summary>
public abstract class BaseEmbeddingSearchConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Whether to deduplicate results before building the knoweldge context for the chat prompt.
    /// Deduplication will remove similar results, that has a 95+ similary score for Fuzzy comparison.
    /// </summary>
    public virtual bool? UseQueryDeduplication { get; set; }

    /// <summary>
    /// Specifies the maximum number of results to return when searching for embeddings.
    /// Note: The vector store retrieves twice this number to ensure sufficient context after duplicate entries are removed.  
    /// Make sure the limit is set high enough when index-time deduplication is enabled.
    /// </summary>
    public virtual int? ContextQueryLimit { get; set; }
}

/// <summary>
/// Base class for configuration of embedding search config overrides.
/// </summary>
public abstract class BaseEmbeddingSearchConfigOverrides<T> : BaseEmbeddingSearchConfigOverrides
    where T : BaseEmbeddingSearchScoringConfigOverrides, new()
{
    /// <summary>
    /// Scoring configuration. 
    /// </summary>
    [Required]
    public virtual T Scoring { get; internal set; } = new();
}