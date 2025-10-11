using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Plugins.Contexts;

namespace Vivet.AI.Services.Requests.Agents.Models.Plugins.Contexts;

/// <summary>
/// Represents the context for the built-in memory plugin.
/// </summary>
public class AgentsMemoryContext : BaseMemoryContext
{
    /// <summary>
    /// Identifier of the agent processing the request.
    /// Used for memory retrieval and context matching.
    /// </summary>
    [Required]
    public virtual Guid AgentId { get; set; }

    /// <summary>
    /// Identifier of the user making the request.
    /// Used for personalizing memory lookups.
    /// </summary>
    public virtual Guid? UserId { get; set; }
}