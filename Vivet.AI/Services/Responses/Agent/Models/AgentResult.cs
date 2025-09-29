using Newtonsoft.Json;
using System;
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
public class AgentResult<T> // BUG: 111: Can we use T ??? problem is for sequential for example, maybe it's the final result we want as type T, but not the intermediate.
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
    /// An error message describing the failure, if one occurred.
    /// Intended for internal use only.
    /// </summary>
    internal virtual string ErrorMessage { get; set; }
}