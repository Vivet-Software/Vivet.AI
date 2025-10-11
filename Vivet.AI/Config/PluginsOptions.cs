namespace Vivet.AI.Config;

/// <summary>
/// Plugin options for chat.
/// </summary>
public class PluginsOptions
{
    /// <summary>
    /// Memory plugin options.
    /// </summary>
    public virtual MemoryPluginOptions Memory { get; set; }

    /// <summary>
    /// Knowledge plugin options.
    /// </summary>
    public virtual KnowledgePluginOptions Knowledge { get; set; }

    /// <summary>
    /// Web search plugin options.
    /// </summary>
    public virtual WebSearchPluginOptions WebSearch { get; set; }
}