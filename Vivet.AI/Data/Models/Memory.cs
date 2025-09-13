using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.VectorData;

namespace Vivet.AI.Data.Models;

/// <summary>
/// Represents a memory entry used for embedding and vector search,
/// including metadata such as user, agent, thread, and question/answer context.
/// </summary>
public class Memory : BaseEmbedding
{
    /// <summary>
    /// Gets or sets the identifier of the agent associated with this memory.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string AgentId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user associated with this memory.
    /// This property is required and indexed for text search.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual string UserId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the conversation thread this memory belongs to.
    /// This property is required and indexed.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual string ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the question/answer pair in this memory.
    /// This property is required and indexed.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual string QuestionAnswerId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this memory entry represents a question.
    /// This property is required and indexed. Defaults to <c>false</c>.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual bool IsQuestion { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this memory entry represents an answer.
    /// This property is required and indexed. Defaults to <c>false</c>.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual bool IsAnswer { get; set; } = false;

    /// <summary>
    /// Gets or sets the array of related context strings from the counterpart
    /// (question or answer) for this memory entry.
    /// This property is required but not indexed by default.
    /// </summary>
    [Required]
    [VectorStoreData]
    public virtual string[] CounterpartContext { get; set; } = [];
}