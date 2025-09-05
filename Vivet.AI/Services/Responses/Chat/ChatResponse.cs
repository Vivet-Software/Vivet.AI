using Newtonsoft.Json;

namespace Vivet.AI.Services.Responses.Chat;

/// <summary>
/// Represents a chat response from the model with a default generic type of <see cref="object"/>.
/// </summary>
public class ChatResponse : ChatResponse<string>;

/// <summary>
/// Represents a chat response from the model with a generic answer type.
/// </summary>
/// <typeparam name="T">The type of the answer returned by the model.</typeparam>
public class ChatResponse<T> : BaseResponse
    where T : class
{
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
    /// An approximation of the chat prompt generated.
    /// It won’t be exactly what SK sends to the backend,
    /// because the Semantic Kernel connector may do additional formatting (e.g., JSON serialization, role tags, or system messages merged differently).
    /// </summary>
    public virtual string InputPrompt { get; set; }
}