using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Agent.Models;
using Vivet.AI.Services.Requests.Agent.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Agent.Models.Plugins;

namespace Vivet.AI.Services.Requests.Agent;

/// <summary>
/// Represents an agent request,
/// defining the orchestration type and the agents to invoke.
/// </summary>
public abstract class BaseAgentsRequest
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
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// The input to pass to the agents.
    /// The orchestration takes your input and routes it through the selected agents in sequence, parallel. etc.,
    /// </summary>
    [Required]
    public virtual string Input { get; set; }

    /// <summary>
    /// Collection of optional blobs associated with the request.
    /// These may provide additional context for answering the question.
    /// </summary>
    [Required]
    public virtual IEnumerable<BaseBlobMetadata> Blobs { get; set; } = [];

    /// <summary>
    /// The agents to invoke.
    /// </summary>
    [Required]
    [MinLength(1)]
    public virtual IEnumerable<AgentDescriptor> Agents { get; set; } = [];

    /// <summary>
    /// Plugins and their associated context for both built-in and custom plugins.
    /// </summary>
    [Required]
    public virtual AgentPlugins Plugins { get; } = new();

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual AgentConfigOverrides ConfigOverrides { get; } = new(); 
}