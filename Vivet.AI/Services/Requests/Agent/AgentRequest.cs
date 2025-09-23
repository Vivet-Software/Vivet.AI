using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Agent.Enums;
using Vivet.AI.Services.Requests.Agent.Models;
using Vivet.AI.Services.Requests.Agent.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Agent.Models.Plugins;

namespace Vivet.AI.Services.Requests.Agent;

/// <summary>
/// Represents an agent request,
/// defining the orchestration type and the agents to invoke.
/// </summary>
public class AgentRequest
{
    /// <summary>
    /// The name of the agentic orchestration.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// A description of the agentic orchestration.
    /// <para>
    /// This property is optional, but it cannot be <c>null</c>. 
    /// By default, it is set to <c>string.Empty</c>.
    /// </para>
    /// </summary>
    [Required]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// The input to pass to the agents.
    /// </summary>
    [Required]
    public virtual string Input { get; set; }

    /// <summary>
    /// The type of orchestration to use for the agents.
    /// </summary>
    [Required]
    public virtual AgentOrchestrationType OrchestrationType { get; set; } = AgentOrchestrationType.Sequential;

    /// <summary>
    /// The agents to invoke
    /// </summary>
    [Required]
    public virtual IEnumerable<AgentDescriptor> Agents { get; set; } = [];

    /// <summary>
    /// Plugins and their associated context for both built-in and custom plugins.
    /// </summary>
    [Required]
    public virtual AgentPlugins Plugins { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    public virtual AgentConfigOverrides ConfigOverrides { get; set; }
}