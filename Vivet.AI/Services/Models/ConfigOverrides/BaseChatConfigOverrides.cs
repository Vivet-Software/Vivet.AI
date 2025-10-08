using System.ComponentModel.DataAnnotations;
using Vivet.AI.Models;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents the base class for chat configuration overrides.
/// </summary>
public abstract class BaseChatConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the name of the model to use for this request, overriding the default configured model.
    /// The specified model must be supported by the registered orchestration; otherwise, the request may fail.
    /// </summary>
    public virtual string ModelName { get; set; }

    /// <summary>
    /// Optional parameters for configuring the behavior of the chat model.
    /// </summary>
    public virtual ChatModelParameters ModelParameters { get; set; }


    // BUG: 000: COnsider all the Skip and maybe auto-skip if context isn't provided.
    /// <summary>
    /// Skips the memory invocaton and context in the prompt for this request.
    /// </summary>
    public virtual bool SkipMemoryContext { get; set; } = false;

    /// <summary>
    /// Skips the knowledge plugin invocaton and context in the prompt for this request.
    /// </summary>
    public virtual bool SkipKnowledgeContext { get; set; } = false;

    /// <summary>
    /// Skips the web search plugin invocaton and context in the prompt for this request.
    /// </summary>
    public virtual bool SkipWebSearchContext { get; set; } = false;

    // BUG: 000: CHAT Config Overrides 
    // better triple-slash

    /// <summary>
    /// Memory search config overrides.
    /// </summary>
    [Required]
    public virtual MemorySearchConfigOverrides MemoryConfigOverrides { get; internal set; } = new();

    /// <summary>
    /// Knowledge search config overrides.
    /// </summary>
    [Required]
    public virtual KnowledgeSearchConfigOverrides KnowledgeConfigOverrides { get; internal set; } = new();

    /// <summary>
    /// Memory index config overrides.
    /// </summary>
    [Required]
    public virtual MemoryIndexConfigOverrides Memory { get; internal set; } = new();
}

/// <summary>
/// 
/// </summary>
public class PluginsConfigOverrides
{

}

// BUG: Where to place
// We have Models/ConfigOverrides and Models/Plugins
// The ones above, e.g. MemorySearchConfigOverrides, are places under embedding. move?
// - We don't have overrides for WebSearch, but if we get it we have nowhere to place it. Why we should move all Memory/Knowledge overrides to Plugins folder
// - But we also have memory indexing, which does not belong in plugins.

/// <summary>
/// 
/// </summary>
public class MemoryConfigOverrides
{

}

/// <summary>
/// 
/// </summary>
public class KnowledgeConfigOverrides
{

}

// BUG: Should we add this just empty? Then we could reuse the same stringbuilder extension
/// <summary>
/// 
/// </summary>
public class WebSearchConfigOverrides
{

}