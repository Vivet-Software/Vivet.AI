using System;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Requests.Embedding.Memory.Models;

/// <summary>
/// Represents base criteria for filtering memory entries.
/// </summary>
public abstract class BaseMemoryCriteria : BaseCriteria<Data.Models.Memory>
{
    /// <summary>
    /// Gets or sets the user ID associated with this criteria.
    /// </summary>
    public virtual Guid? UserId { get; set; }

    /// <summary>
    /// The ID of the agent associated with the memory entry.
    /// </summary>
    public virtual Guid? AgentId { get; set; }
}