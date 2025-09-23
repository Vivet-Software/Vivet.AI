namespace Vivet.AI.Config;

/// <summary>
/// Built-In plugin options for chat.
/// </summary>
public class BuiltInPluginsOptions
{
    /// <summary>
    /// Options to configure the memory recall from past current or conversations.
    /// </summary>
    public virtual MemoryPluginOptions Memory { get; set; }

    /// <summary>
    /// Options to configure persistant knowledge.
    /// </summary>
    public virtual KnowledgePluginOptions Knowledge { get; set; }

    /// <summary>
    /// Web search plugin options.
    /// </summary>
    public virtual WebSearchPluginOptions WebSearch { get; set; }
}