using System.ComponentModel.DataAnnotations;
using Vivet.AI.Models;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Chat.Models;

/// <summary>
/// Represents configuration overrides specific to chat operations.
/// </summary>
public class ChatConfigOverrides : BaseConfigOverrides
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
    /// Memory config overrides.
    /// </summary>
    [Required]
    public virtual ChatMemoryOverrides Memory { get; set; } = new();

    /// <summary>
    /// Knowledge config overrides.
    /// </summary>
    [Required]
    public virtual ChatKnowledgeOverrides Knowledge { get; set; } = new();
}