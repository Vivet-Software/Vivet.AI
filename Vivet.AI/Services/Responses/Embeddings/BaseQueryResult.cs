using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Responses.Embeddings;

/// <summary>
/// Represents the result of a query operation containing the result object and its size.
/// </summary>
/// <typeparam name="TResponse">The type of the result, must inherit from <see cref="BaseResult"/> and have a parameterless constructor.</typeparam>
public abstract class BaseQueryResult<TResponse>
    where TResponse : BaseResult, new()
{
    /// <summary>
    /// The result object of the query.
    /// </summary>
    public virtual TResponse Result { get; set; } = new();

    /// <summary>
    /// The size or length associated with the result.
    /// </summary>
    public virtual long Size { get; set; }
}