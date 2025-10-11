using System;
using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Services.Models.Plugins.Contexts;

/// <summary>
/// base class for memory plugin context.
/// </summary>
public abstract class BaseMemoryContext
{
    /// <summary>
    /// Scope identifier for the request.
    /// Used when looking up memory entries.
    /// </summary>
    public virtual Guid? ScopeId { get; set; }

    /// <summary>
    /// Identifier of the current conversation thread.
    /// Used to boost the relevance of memory entries in the same thread.
    /// <para>
    /// Default: <code>Guid.NewGuid().ToString()</code>
    /// </para>
    /// </summary>
    [Required]
    public virtual Guid CurrentThreadId { get; set; } = Guid.NewGuid();
}