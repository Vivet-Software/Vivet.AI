using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Chat Options.
/// </summary>
public class ChatOptions
{
    /// <summary>
    /// The chat model name.
    /// Make sure the model is configured in the choosen AI provider (e.g. Azure AI, Azure OpenAU, Ollama, etc).
    /// </summary>
    [Required]
    public virtual ChatModel Model { get; set; } = new();

    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Options for enabling or disabling built-in plugins.
    /// </summary>
    [Required]
    public virtual PluginsToggleOptions Plugins { get; set; } = new();
}