using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Plugins.BuiltIn;

namespace Vivet.AI.Services.Requests.Chat.Models.Plugins.BuiltIn;

/// <summary>
/// Represents the context for the built-in memory plugin.
/// </summary>
public class ChatMemoryContext : BaseMemoryContext
{
    /// <summary>
    /// Identifier of the user making the request.
    /// Used for personalizing memory lookups.
    /// </summary>
    [Required]
    public virtual string UserId { get; set; }
}