using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Defines toggle options for enabling or disabling built-in plugins.
/// Each toggle only takes effect if the corresponding plugin is properly configured.
/// </summary>
public class PluginsToggleOptions
{
    /// <summary>
    /// Enables or disables the Memory plugin.
    /// The embedding memory configuration must be defined for this setting to take effect.
    /// </summary>
    [Required]
    public virtual bool EnableMemoryPlugin { get; set; } = true;

    /// <summary>
    /// Enables or disables the Knowledge plugin.
    /// The embedding knowledge configuration must be defined for this setting to take effect.
    /// </summary>
    [Required]
    public virtual bool EnableKnowledgePlugin { get; set; } = true;

    /// <summary>
    /// Enables or disables the Web Search plugin.
    /// Web search settings must be configured in <see cref="PluginsOptions"/> for this to take effect.
    /// </summary>
    [Required]
    public virtual bool EnableWebSearchPlugin { get; set; } = true;
}