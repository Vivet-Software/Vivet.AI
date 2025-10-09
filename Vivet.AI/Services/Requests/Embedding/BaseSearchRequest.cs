using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding;

/// <summary>
/// Represents the base request for searching a collection of embeddings using specific criteria.
/// </summary>
/// <typeparam name="TCriteria">The type of the search criteria. Must inherit from <see cref="BaseCriteria{TCollection}"/> and have a parameterless constructor.</typeparam>
/// <typeparam name="TOverride">The type of config override. Must inherit from <see cref="BaseSearchConfigOverrides"/>.</typeparam>
public abstract class BaseSearchRequest<TCriteria, TOverride>
    where TCriteria : BaseCriteria, new()
    where TOverride : BaseSearchConfigOverrides, new()
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
    public virtual int? Limit { get; set; } 

    /// <summary>
    /// The config overrides.
    /// </summary>
    [Required]
    public virtual TOverride ConfigOverrides { get; set; } = new();
}