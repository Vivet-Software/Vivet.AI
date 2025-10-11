using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents the class for built-in plugin configuration overrides.
/// </summary>
public class PluginsConfigOverrides
{
    /// <summary>
    /// Memory index config overrides.
    /// Plugins must be configured in order for the overrides to be 
    /// </summary>
    [Required]
    public virtual MemoryConfigOverrides Memory { get; internal set; } = new();

    /// <summary>
    /// Memory index config overrides.
    /// </summary>
    [Required]
    public virtual KnowledgeConfigOverrides Knowledge { get; internal set; } = new();

    /// <summary>
    /// Memory index config overrides.
    /// </summary>
    [Required]
    public virtual WebSearchConfigOverrides WebSearch { get; internal set; } = new();
}