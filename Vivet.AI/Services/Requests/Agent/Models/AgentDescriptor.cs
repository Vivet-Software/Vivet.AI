using System;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Agent.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Agent.Models.Plugins;

namespace Vivet.AI.Services.Requests.Agent.Models;

/// <summary>
/// Represents the configuration and metadata required to create a Semantic Kernel agent.
/// </summary>
public class AgentDescriptor
{
    /// <summary>
    /// Gets the unique identifier for the agent.
    /// <para>
    /// This identifier is used to distinguish the agent during orchestration execution
    /// and when storing or retrieving the agent’s memories. 
    /// Each agent must have a unique identifier to avoid conflicts.
    /// </para>
    /// </summary>
    [Required]
    public virtual string Id { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The name of the agent.
    /// Cannot contain spaces.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// A brief description of the agent's purpose or role.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// The system message or instructions that define the agent's behavior.
    /// Typically used as the system prompt for a chat agent.
    /// </summary>
    [Required]
    public virtual string Instructions { get; set; }

    /// <summary>
    /// The role of the agent when participating in a conversation.
    /// Defaults to <see cref="AuthorRole.Assistant"/>.
    /// </summary>
    public virtual AuthorRole Role { get; set; } = AuthorRole.System;

    /// <summary>
    /// Represents plugins and their associated context, including both built-in and custom plugins.
    /// <para>
    /// Setting this property overrides the plugins defined on the parent request.
    /// </para>
    /// </summary>
    [Required]
    public virtual AgentPlugins Plugins { get; } = new();

    /// <summary>
    /// Allows overriding default configuration for the agent, such as model-specific settings
    /// or behavior adjustments..
    /// <para>
    /// Setting this property overrides the plugins defined on the parent request.
    /// </para>
    /// </summary>
    [Required]
    public virtual AgentsConfigOverrides ConfigOverrides { get; } = new();
}