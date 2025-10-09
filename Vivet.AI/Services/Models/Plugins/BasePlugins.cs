using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Plugins.Contexts;

namespace Vivet.AI.Services.Models.Plugins;

/// <summary>
/// Represents the base class for plugins and their associated context for both built-in and custom plugins.
/// </summary>
public abstract class BasePlugins<TContext>
    where TContext : BaseBuiltInContext, new()
{
    /// <summary>
    /// Provides the execution context required by built-in plugins.
    /// <para>
    /// If a built-in plugin is enabled and configured, the corresponding 
    /// context variables must be set on the request. Otherwise, the plugin's 
    /// behavior is not guaranteed.
    /// </para>
    /// </summary>
    [Required]
    public virtual TContext Context { get; internal set; } = new();

    /// <summary>
    /// A collection of custom plugins and their associated context 
    /// that should be included in the request.
    /// </summary>
    [Required]
    public virtual IEnumerable<CustomPlugin> CustomPlugins { get; set; } = [];
}