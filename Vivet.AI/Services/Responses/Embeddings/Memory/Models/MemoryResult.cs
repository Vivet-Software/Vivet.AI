using System;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Responses.Embeddings.Memory.Models;

/// <summary>
/// Represents the result of a memory entry, including associated metadata, blob, and context.
/// </summary>
public class MemoryResult : BaseResult
{
    /// <summary>
    /// The scope identifier of the memory entry.
    /// </summary>
    public Guid? ScopeId { get; set; }

    /// <summary>
    /// The agent identifier associated with the memory entry.
    /// </summary>
    public Guid? AgentId { get; set; }

    /// <summary>
    /// The user identifier associated with the memory entry.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The thread identifier for the memory entry.
    /// </summary>
    public Guid? ThreadId { get; set; }

    /// <summary>
    /// The unique identifier for the question-answer pair.
    /// </summary>
    public Guid? QuestionAnswerId { get; set; }

    /// <summary>
    /// Indicates whether this entry is a question.
    /// </summary>
    public bool IsQuestion { get; set; }

    /// <summary>
    /// Indicates whether this entry is an answer.
    /// </summary>
    public bool IsAnswer { get; set; }

    /// <summary>
    /// The blob associated with the memory entry.
    /// </summary>
    public BlobResponse Blob { get; set; }

    /// <summary>
    /// The counterpart context for the memory entry.
    /// </summary>
    public string[] CounterpartContext { get; set; } = [];

    /// <summary>
    /// Default constructor.
    /// </summary>
    public MemoryResult()
    {
    }

    /// <summary>
    /// Constructs a <see cref="MemoryResult"/> from a <see cref="Data.Models.Memory"/> instance.
    /// </summary>
    /// <param name="memory">The memory model to map from.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="memory"/> is null.</exception>
    public MemoryResult(Data.Models.Memory memory)
        : base(memory)
    {
        if (memory == null)
            throw new ArgumentNullException(nameof(memory));

        this.ScopeId = memory.ScopeId == null 
            ? null 
            : Guid.Parse(memory.ScopeId);
        this.AgentId = memory.AgentId == null 
            ? null 
            : Guid.Parse(memory.AgentId);
        this.UserId = memory.UserId == null 
            ? null 
            : Guid.Parse(memory.UserId);
        this.ThreadId = memory.ThreadId == null 
            ? null 
            : Guid.Parse(memory.ThreadId);
        this.QuestionAnswerId = memory.QuestionAnswerId == null 
            ? null 
            : Guid.Parse(memory.QuestionAnswerId);
        this.IsQuestion = memory.IsQuestion;
        this.IsAnswer = memory.IsAnswer;
        this.Blob = memory.BlobBase64 == null
            ? null
            : new BlobResponse
            {
                MimeType = MimeType.FromValue(memory.BlobMimeType),
                Base64 = memory.BlobBase64,
                Hash = memory.ContentHash
            };
        this.CounterpartContext = memory.CounterpartContext;
    }
}