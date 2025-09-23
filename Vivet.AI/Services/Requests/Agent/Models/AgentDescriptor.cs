using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.SemanticKernel.ChatCompletion;
using Vivet.AI.Services.Requests.Agent.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Agent.Models.Plugins;

namespace Vivet.AI.Services.Requests.Agent.Models;

/// <summary>
/// Represents the configuration and metadata required to create a Semantic Kernel agent.
/// </summary>
public class AgentDescriptor
{
    /// <summary>
    /// A unique identifier for the agent. Defaults to a new GUID if not provided.
    /// </summary>
    [Required]
    public virtual string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The display name of the agent.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// A brief description of the agent's purpose or role.
    /// </summary>
    [Required]
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
    public virtual AuthorRole Role { get; set; } = AuthorRole.Assistant;

    /// <summary>
    /// Represents plugins and their associated context, including both built-in and custom plugins.
    /// Setting this property overrides the plugins defined on the parent request or kernel.
    /// </summary>
    [Required]
    public virtual AgentPlugins Plugins { get; set; }

    /// <summary>
    /// Allows overriding default configuration for the agent, such as model-specific settings
    /// or behavior adjustments. This works in conjunction with the kernel and plugins.
    /// </summary>
    [Required]
    public virtual AgentConfigOverrides ConfigOverrides { get; set; }
}