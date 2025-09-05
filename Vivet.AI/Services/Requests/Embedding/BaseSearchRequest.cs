using System.ComponentModel.DataAnnotations;
using Vivet.AI.Data.Models;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Requests.Embedding;

/// <summary>
/// Represents the base request for searching a collection of embeddings using specific criteria.
/// </summary>
/// <typeparam name="TCollection">The type of the collection items. Must inherit from <see cref="BaseEmbedding"/>.</typeparam>
/// <typeparam name="TCriteria">The type of the search criteria. Must inherit from <see cref="BaseCriteria{TCollection}"/> and have a parameterless constructor.</typeparam>
public abstract class BaseSearchRequest<TCollection, TCriteria>
    where TCollection : BaseEmbedding
    where TCriteria : BaseCriteria<TCollection>, new()
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
    public virtual int Limit { get; set; } = 25;
}