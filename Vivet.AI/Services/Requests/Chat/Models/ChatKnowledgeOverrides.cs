namespace Vivet.AI.Services.Requests.Chat.Models;

/// <summary>
/// Represents configuration overrides specific to knowledge in chat operations.
/// </summary>
public class ChatKnowledgeOverrides
{
    /// <summary>
    /// Skips the knowledge context in the prompt for this request.
    /// </summary>
    public virtual bool SkipKnowledgeContext { get; set; } = false;
}