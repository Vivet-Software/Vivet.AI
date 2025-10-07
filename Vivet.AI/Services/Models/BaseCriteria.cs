using System;
using System.Linq.Expressions;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Services.Models;

/// <summary>
/// Represents the base criteria used to filter collections of embeddings.
/// </summary>
public abstract class BaseCriteria
{
    /// <summary>
    /// Gets or sets the scope ID associated with this criteria.
    /// </summary>
    public virtual Guid? ScopeId { get; set; }
}

/// <summary>
/// Represents the base criteria used to filter collections of embeddings.
/// </summary>
/// <typeparam name="TCollection">The type of embedding collection being filtered. Must inherit from <see cref="BaseEmbedding"/>.</typeparam>
public abstract class BaseCriteria<TCollection> : BaseCriteria
    where TCollection : BaseEmbedding
{
    /// <summary>
    /// Builds the filter expression for the collection based on the criteria.
    /// </summary>
    /// <returns>An <see cref="Expression{Func}"/> representing the filter.</returns>
    internal abstract Expression<Func<TCollection, bool>> BuildFilter();
}