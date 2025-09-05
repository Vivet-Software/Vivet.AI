using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Responses.Embeddings;

/// <summary>
/// Represents a search result containing a score and the associated result object.
/// </summary>
/// <typeparam name="TResponse">The type of the result, must inherit from <see cref="BaseResult"/> and have a parameterless constructor.</typeparam>
public abstract class BaseSearchResult<TResponse>
    where TResponse : BaseResult, new()
{
    /// <summary>
    /// The relevance score of the search result.
    /// </summary>
    public virtual double Score { get; set; }

    /// <summary>
    /// The result object associated with the search result.
    /// </summary>
    public virtual TResponse Result { get; set; } = new();
}