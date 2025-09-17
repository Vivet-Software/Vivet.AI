using System.ComponentModel.DataAnnotations;
using Vivet.AI.Data.Models;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Requests.Embedding;

/// <summary>
/// Represents the base request for querying a collection of embeddings with specific criteria.
/// </summary>
/// <typeparam name="TCollection">The type of the collection items. Must inherit from <see cref="BaseEmbedding"/>.</typeparam>
/// <typeparam name="TCriteria">The type of the query criteria. Must inherit from <see cref="BaseCriteria{TCollection}"/> and have a parameterless constructor.</typeparam>
public abstract class BaseQueryRequest<TCollection, TCriteria>
    where TCollection : BaseEmbedding
    where TCriteria : BaseCriteria<TCollection>, new()
{
    /// <summary>
    /// Gets or sets the query criteria.
    /// </summary>
    public virtual TCriteria Criteria { get; set; } = new();

    /// <summary>
    /// Gets or sets the maximum number of results to return.
    /// </summary>
    [Required]
    public virtual int Limit { get; set; } = 25;

    /// <summary>
    /// Gets or sets the number of results to skip from the start.
    /// </summary>
    [Required]
    public virtual int Skip { get; set; } = 0;
}