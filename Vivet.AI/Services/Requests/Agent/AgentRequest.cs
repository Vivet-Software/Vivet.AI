using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Agent.Enums;

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
    /// The description of the agentic orchestration.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// The input to pass to the agents.
    /// </summary>
    [Required]
    public virtual string Input { get; set; }

    /// <summary>
    /// The type of orchestration to use for the agents.
    /// </summary>
    public virtual AgentOrchestrationType OrchestrationType { get; set; } = AgentOrchestrationType.Sequential;

    /// <summary>
    /// The agents to invoke
    /// </summary>
    public virtual IEnumerable<Agent2> Agents { get; set; } = [];
}

/// <summary>
/// 
/// </summary>
public class Agent2
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// The system message for the agent.
    /// </summary>
    [Required]
    public virtual string Instructions { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual AuthorRole Role { get; set; } = AuthorRole.Assistant;
}