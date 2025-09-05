using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Chat.Models;

/// <summary>
/// Represents configuration overrides specific to memory in chat operations.
/// </summary>
public class ChatMemoryOverrides
{
    /// <summary>
    /// Skips the memory context in the prompt for this request.
    /// </summary>
    public virtual bool SkipMemoryContext { get; set; } = false;

    /// <summary>
    /// Skips saving memory context for the request.
    /// </summary>
    public virtual bool SkipSaveMemoryContext { get; set; } = false;

    /// <summary>
    /// Metadata retrieval overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingMetadataConfigOverrides Metadata { get; set; } = new();

    /// <summary>
    /// Summarization overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingSummarizationConfigOverrides Summarization { get; set; } = new();
}