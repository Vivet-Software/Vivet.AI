using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Vivet.AI.Services.Collectors.Models;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Responses.Agent.Models;

/// <summary>
/// Represents an agent response from the model with a default generic type of <see cref="string"/>.
/// </summary>
public class AgentResult : AgentResult<string>;

/// <summary>
/// Represents an agent response.
/// </summary>
/// <typeparam name="T">The type of the answer returned by the model.</typeparam>
public class AgentResult<T> // BUG: 444: Can we use T ??? problem is for sequential for example, maybe it's the final result we want as type T, but not the intermediate.
    where T : class
{
    /// <summary>
    /// The unique identifier of the agent.
    /// </summary>
    public virtual string AgentId { get; set; }

    /// <summary>
    /// The answer produced by the model.
    /// </summary>
    [JsonIgnore]
    public virtual T Answer { get; set; }

    /// <summary>
    /// A short explanation of the model's reasoning for its answer.
    /// </summary>
    public virtual string Reasoning { get; set; }

    /// <summary>
    /// A detailed explanation of the model's thought process.
    /// Only available for certain models, e.g., DeepSeek-R1-0528.
    /// </summary>
    public virtual string Thinking { get; set; }

    /// <summary>
    /// The raw, unprocessed response returned by the model.
    /// </summary>
    public virtual string RawResponse { get; set; }
    
    /// <summary>
    /// An approximation of the instrcution prompt generated for the agent.
    /// It won’t be exactly what SK sends to the backend,
    /// because the Semantic Kernel connector may do additional formatting
    /// (e.g., JSON serialization, role tags, or system messages merged differently).
    /// </summary>
    public virtual string InstructionsPrompt { get; set; }
    
    /// <summary>
    /// The language detected of the input prompt.
    /// </summary>
    public virtual string Language { get; set; }

    /// <summary>
    /// This ID may be exposed by the underlying language model through its metadata. 
    /// Its presence is model-dependent and may not always be available.
    /// </summary>
    public virtual string ExternalId { get; set; }

    /// <summary>
    /// Information about token usage for the request, including input and output token counts.
    /// Not supported for streaming responses.
    /// </summary>
    public virtual TokenUsage TokenUsage { get; set; }

    /// <summary>
    /// The total time elapsed while processing the agent.
    /// </summary>
    public virtual TimeSpan ElapsedTime { get; set; }

    /// <summary>
    /// The function calls invoked during the request.
    /// </summary>
    public virtual IEnumerable<FunctionCall> FunctionCalls { get; set; } = [];

    /// <summary>
    /// An exception describing the failure, if one occurred.
    /// </summary>
    public virtual Exception Exception { get; set; }

    /// <summary>
    /// An error message describing the failure, if one occurred.
    /// Intended for internal use only.
    /// </summary>
    internal virtual string ErrorMessage { get; set; }
}