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

    // BUG: 888: CHAT Config Overrides - How can we combine Index with Query here.

    /// <summary>
    /// Memory search config overrides.
    /// </summary>
    public virtual EmbeddingMemorySearchConfigOverrides MemoryConfigOverrides { get; set; }

    /// <summary>
    /// Knowledge search config overrides.
    /// </summary>
    public virtual EmbeddingKnowledgeSearchConfigOverrides KnowledgeCongigOverrides { get; set; }

    /// <summary>
    /// Memory config overrides.
    /// </summary>
    [Required]
    public virtual EmbeddingMemoryIndexConfigOverrides Memory { get; internal set; } = new();
}