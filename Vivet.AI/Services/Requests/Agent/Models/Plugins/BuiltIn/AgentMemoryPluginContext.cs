using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Plugins.BuiltIn;

namespace Vivet.AI.Services.Requests.Agent.Models.Plugins.BuiltIn;

/// <summary>
/// Represents the context for the built-in memory plugin.
/// </summary>
public class AgentMemoryPluginContext : BaseMemoryPluginContext
{
    /// <summary>
    /// Identifier of the agent processing the request.
    /// Used for memory retrieval and context matching.
    /// </summary>
    [Required]
    public virtual string AgentId { get; set; }

    /// <summary>
    /// Identifier of the user making the request.
    /// Used for personalizing memory lookups.
    /// </summary>
    public virtual string UserId { get; set; }
}