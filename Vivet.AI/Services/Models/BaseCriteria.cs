using System;
using System.Linq.Expressions;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Services.Models;

/// <summary>
/// Represents the base criteria used to filter collections of embeddings.
/// </summary>
/// <typeparam name="TCollection">The type of embedding collection being filtered. Must inherit from <see cref="BaseEmbedding"/>.</typeparam>
public abstract class BaseCriteria<TCollection>
    where TCollection : BaseEmbedding
{
    /// <summary>
    /// Gets or sets the user ID associated with this criteria.
    /// </summary>
    public virtual string UserId { get; set; }

    /// <summary>
    /// Gets or sets the scope ID associated with this criteria.
    /// </summary>
    public virtual string ScopeId { get; set; }

    /// <summary>
    /// Gets or sets the date range for filtering.
    /// </summary>
    public virtual DateRange DateRange { get; set; }

    /// <summary>
    /// Builds the filter expression for the collection based on the criteria.
    /// </summary>
    /// <returns>An <see cref="Expression{Func}"/> representing the filter.</returns>
    internal abstract Expression<Func<TCollection, bool>> BuildFilter();
}