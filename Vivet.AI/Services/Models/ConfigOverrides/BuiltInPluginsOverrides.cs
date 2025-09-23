namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to built-in plugins.
/// </summary>
public class BuiltInPluginsOverrides
{
    /// <summary>
    /// Options to configure the memory recall from past current or conversations.
    /// </summary>
    public virtual MemoryPluginOverrides Memory { get; set; }

    /// <summary>
    /// Options to configure persistant knowledge.
    /// </summary>
    public virtual KnowledgePluginOverrides Knowledge { get; set; }

    /// <summary>
    /// Options to configure web search.
    /// </summary>
    public virtual WebSearchPluginOverrides WebSearch { get; set; }
}