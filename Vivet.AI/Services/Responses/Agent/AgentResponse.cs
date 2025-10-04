using System.Collections.Generic;
using Vivet.AI.Services.Responses.Agent.Models;

namespace Vivet.AI.Services.Responses.Agent;

/// <summary>
/// Represents a chat response from the model with a default generic type of <see cref="string"/>.
/// </summary>
public class AgentResponse : AgentResponse<string>;

/// <summary>
/// Represents an agent response.
/// </summary>
/// <typeparam name="T">The type of the answer returned by the model.</typeparam>
public class AgentResponse<T> : BaseResponse
    where T : class
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