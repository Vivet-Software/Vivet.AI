using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Plugin options for chat model.
/// </summary>
public class PluginsOptions
{
    /// <summary>
    /// Built-in plugins that can be enabled for the chat model.
    /// </summary>
    [Required]
    public virtual BuiltInPluginsOptions BuiltInPlugins { get; set; } = new();

    /// <summary>
    /// A collection of custom plugins.
    /// </summary>
    public virtual IEnumerable<CustomPluginOptions> CustomPlugins { get; set; } = [];
}