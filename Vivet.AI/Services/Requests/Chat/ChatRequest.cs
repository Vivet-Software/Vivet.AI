using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Chat.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Chat.Models.Plugins;

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
    /// Collection of optional blobs associated with the request.
    /// These may provide additional context for answering the question.
    /// </summary>
    [Required]
    public virtual IEnumerable<BaseBlobMetadata> Blobs { get; set; } = [];

    /// <summary>
    /// Plugins and their associated context for both built-in and custom plugins.
    /// </summary>
    [Required]
    public virtual ChatPlugins Plugins { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    public virtual ChatConfigOverrides ConfigOverrides { get; set; }
}