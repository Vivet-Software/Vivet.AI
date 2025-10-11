using System;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models;

/// <summary>
/// Represents base criteria for filtering knowledge entries.
/// </summary>
public abstract class BaseKnowledgeCriteria : BaseCriteria<Data.Models.Knowledge>
{
    /// <summary>
    /// The tenant identifier for filtering knowledge entries.
    /// </summary>
    public virtual Guid? TenantId { get; set; }

    /// <summary>
    /// The sub-tenant identifier for filtering knowledge entries.
    /// </summary>
    public virtual Guid? SubTenantId { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with this criteria.
    /// </summary>
    public virtual Guid? UserId { get; set; }
}