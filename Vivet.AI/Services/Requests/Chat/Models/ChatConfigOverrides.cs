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