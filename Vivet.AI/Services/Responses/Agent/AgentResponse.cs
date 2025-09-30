using System.Collections.Generic;
using Vivet.AI.Services.Responses.Agent.Models;

namespace Vivet.AI.Services.Responses.Agent;

/// <summary>
/// Represents an agent response.
/// </summary>
public class AgentResponse : BaseResponse
{
    /// <summary>
    /// An approximation of the agent input prompt generated. The user role message.
    /// It won’t be exactly what SK sends to the backend,
    /// because the Semantic Kernel connector may do additional formatting
    /// (e.g., JSON serialization, role tags, or system messages merged differently).
    /// </summary>
    public virtual string InputPrompt { get; set; }

    /// <summary>
    /// The results for each agent invoked in the request.
    /// </summary>
    public virtual IEnumerable<AgentResult> Results { get; set; } = [];
}