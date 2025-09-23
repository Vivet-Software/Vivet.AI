namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to knowledge in chat operations.
/// </summary>
public class KnowledgePluginOverrides
{
    /// <summary>
    /// Skips the knowledge plugin invocaton and context in the prompt for this request.
    /// </summary>
    public virtual bool SkipKnowledgeContext { get; set; } = false;
}