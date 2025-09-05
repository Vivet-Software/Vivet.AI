using System.ComponentModel.DataAnnotations;
using Vivet.AI.Models;

namespace Vivet.AI.Config.Models;

/// <summary>
/// Chat Model.
/// </summary>
public class ChatModel : BaseModel
{
    /// <summary>
    /// Paramters for configuring options of the chat model.
    /// </summary>
    [Required]
    public virtual ChatModelParameters Parameters { get; set; } = new();
}