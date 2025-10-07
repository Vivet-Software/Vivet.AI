using System.ComponentModel.DataAnnotations;
using Vivet.AI.Data.Models;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding;

/// <summary>
/// Represents the base request for searching a collection of embeddings using specific criteria.
/// </summary>
/// <typeparam name="TCriteria">The type of the search criteria. Must inherit from <see cref="BaseCriteria{TCollection}"/> and have a parameterless constructor.</typeparam>
/// <typeparam name="TOverride">The type of config override. Must inherit from <see cref="BaseEmbeddingSearchConfigOverrides"/>.</typeparam>
public abstract class BaseSearchRequest<TCriteria, TOverride>
    where TCriteria : BaseCriteria, new()
    where TOverride : BaseEmbeddingSearchConfigOverrides, new()
{
    /// <summary>
    /// Gets or sets the query string used for the search.
    /// </summary>
    [Required]
    public virtual string Query { get; set; }

    /// <summary>
    /// Gets or sets the search criteria.
    /// </summary>
    [Required]
    public virtual TCriteria Criteria { get; set; } = new();

    /// <summary>
    /// Gets or sets the maximum number of results to return.
    /// </summary>
    [Required]
    public virtual int Limit { get; set; } = 5; // BUG: 111: We never use this, it's computed, maybe nullable and then we can use it if null, otherwise from settings?

    /// <summary>
    /// The config overrides.
    /// </summary>
    [Required]
    public virtual TOverride ConfigOverrides { get; set; } = new();
}