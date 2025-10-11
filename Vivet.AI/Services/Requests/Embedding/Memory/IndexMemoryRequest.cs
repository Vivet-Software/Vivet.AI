using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Attributes;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Memory;

/// <inheritdoc />
public class IndexMemoryRequest : IndexMemoryRequest<string>;

/// <summary>
/// Represents a request to index a memory entry with optional configuration overrides.
/// </summary>
public class IndexMemoryRequest<T> : BaseIndexRequest<MemoryIndexConfigOverrides>
    where T : class
{
    /// <summary>
    /// The question associated with the memory entry.
    /// </summary>
    [Required]
    public virtual string Question { get; set; }

    /// <summary>
    /// The answer corresponding to the question.
    /// </summary>
    [Required]
    public virtual T Answer { get; set; }

    /// <summary>
    /// The ID of the user creating the memory entry.
    /// </summary>
    [RequiredOneOf(nameof(this.UserId))]
    public virtual Guid? UserId { get; set; }

    /// <summary>
    /// The ID of the user creating the memory entry.
    /// </summary>
    [RequiredOneOf(nameof(this.AgentId))]
    public virtual Guid? AgentId { get; set; }

    /// <summary>
    /// The ID of the thread or conversation.
    /// </summary>
    [Required]
    public virtual Guid ThreadId { get; set; }

    /// <summary>
    /// The collection of blobs associated with the memory entry.
    /// </summary>
    [Required]
    public virtual IEnumerable<BaseBlobMetadata> Blobs { get; set; } = [];
}