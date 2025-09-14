using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Plugin options for chat model.
/// </summary>
public class ChatPluginsOptions : BasePluginsOptions
{
    /// <summary>
    /// Built-in plugins that can be enabled for the chat model.
    /// </summary>
    [Required]
    public virtual ChatBuiltInPluginsOptions BuiltInPlugins { get; set; } = new();
}