using System;

namespace Vivet.AI.Services.Models.Plugins.BuiltIn;

/// <summary>
/// Represents the context for the built-in knowledge plugin.
/// </summary>
public class KnowledgeContext
{
    /// <summary>
    /// Identifier of the tenant associated with this request.
    /// Used when looking up relevant knowledge entries.
    /// </summary>
    public virtual Guid? TenantId { get; set; }

    /// <summary>
    /// Identifier of the sub-tenant associated with this request.
    /// Used when looking up relevant knowledge entries.
    /// </summary>
    public virtual Guid? SubTenantId { get; set; }

    /// <summary>
    /// Identifier of the user making the request.
    /// Used for personalizing knowledge lookups.
    /// </summary>
    public virtual Guid? UserId { get; set; }

    /// <summary>
    /// Scope identifier for the request.
    /// Used when looking up knowledge entries.
    /// </summary>
    public virtual Guid? ScopeId { get; set; }
}