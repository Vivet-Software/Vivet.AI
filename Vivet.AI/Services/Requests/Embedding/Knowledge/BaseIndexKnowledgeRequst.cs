using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge;

/// <summary>
/// Represents the base request for indexing knowledge with optional configuration overrides.
/// </summary>
/// <typeparam name="TOverrides">The type of configuration overrides. Must inherit from <see cref="BaseConfigOverrides"/> and have a parameterless constructor.</typeparam>
public abstract class BaseIndexKnowledgeRequst<TOverrides> : BaseIndexRequest<TOverrides>
    where TOverrides : BaseConfigOverrides, new()
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public virtual Guid? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the sub-tenant identifier.
    /// </summary>
    public virtual Guid? SubTenantId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public virtual Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the source of the knowledge content.
    /// </summary>
    public virtual string Source { get; set; }

    /// <summary>
    /// Gets or sets the name of the creator of the knowledge content.
    /// </summary>
    public virtual string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with the knowledge content.
    /// </summary>
    [Required]
    public virtual string[] Tags { get; set; } = [];
}