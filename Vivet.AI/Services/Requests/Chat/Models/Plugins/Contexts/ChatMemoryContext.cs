using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Plugins.Contexts;

namespace Vivet.AI.Services.Requests.Chat.Models.Plugins.Contexts;

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
    public virtual Guid UserId { get; set; } 
}