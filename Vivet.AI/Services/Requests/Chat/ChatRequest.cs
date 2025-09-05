using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Chat.Models;

namespace Vivet.AI.Services.Requests.Chat;

/// <summary>
/// Represents a chat request, including system message, user question,
/// conversation context, and optional blob data for reference.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// A system message that provides context or instructions to the chat model.
    /// </summary>
    public virtual string SystemMessage { get; set; }

    /// <summary>
    /// The user's question to be answered in the chat.
    /// </summary>
    [Required]
    public virtual string Question { get; set; }

    /// <summary>
    /// Identifier of the tenant associated with this request.
    /// Used when looking up relevant knowledge entries.
    /// </summary>
    public virtual string TenantId { get; set; }

    /// <summary>
    /// Identifier of the sub-tenant associated with this request.
    /// Used when looking up relevant knowledge entries.
    /// </summary>
    public virtual string SubTenantId { get; set; }

    /// <summary>
    /// Scope identifier for the request.
    /// Used when looking up knowledge entries or memories.
    /// </summary>
    public virtual string ScopeId { get; set; }

    /// <summary>
    /// Identifier of the agent processing the request.
    /// Used for memory retrieval and context matching.
    /// </summary>
    public virtual string AgentId { get; set; }

    /// <summary>
    /// Identifier of the user making the request.
    /// Used for personalizing memory and knowledge lookups.
    /// </summary>
    [Required]
    public virtual string UserId { get; set; }

    /// <summary>
    /// Identifier of the current conversation thread.
    /// Used to boost the relevance of memory entries in the same thread.
    /// </summary>
    [Required]
    public virtual string CurrentThreadId { get; set; }

    /// <summary>
    /// The language of the request, typically for localization or model selection.
    /// </summary>
    public virtual string Language { get; set; }

    /// <summary>
    /// Collection of optional blobs associated with the request.
    /// These may provide additional context for answering the question.
    /// </summary>
    [Required]
    public virtual IEnumerable<BaseBlobMetadata> Blobs { get; set; } = [];

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual ChatConfigOverrides ConfigOverrides { get; set; } = new();
}