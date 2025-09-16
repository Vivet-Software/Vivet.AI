namespace Vivet.AI.Config;

/// <summary>
/// Built-In plugin options for chat.
/// </summary>
public class ChatBuiltInPluginsOptions
{
    /// <summary>
    /// Options to configure the memory recall from past current or conversations.
    /// </summary>
    public virtual ChatMemoryPluginOptions Memory { get; set; }

    /// <summary>
    /// Options to configure persistant knowledge.
    /// </summary>
    public virtual ChatKnowledgePluginOptions Knowledge { get; set; }

    /// <summary>
    /// Web search plugin options.
    /// </summary>
    public virtual ChatWebSearchPluginOptions WebSearch { get; set; }
}