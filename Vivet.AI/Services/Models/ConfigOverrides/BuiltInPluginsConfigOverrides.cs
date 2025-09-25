using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to built-in plugins.
/// </summary>
public class BuiltInPluginsConfigOverrides
{
    /// <summary>
    /// Options to configure the memory recall from past current or conversations.
    /// </summary>
    [Required]
    public virtual MemoryPluginOverrides Memory { get; } = new();

    /// <summary>
    /// Options to configure persistant knowledge.
    /// </summary>
    [Required]
    public virtual KnowledgePluginOverrides Knowledge { get; } = new();

    /// <summary>
    /// Options to configure web search.
    /// </summary>
    [Required]
    public virtual WebSearchPluginOverrides WebSearch { get; } = new();
}