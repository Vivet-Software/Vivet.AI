using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;

/// <summary>
/// Represents the class for built-in knowledge plugin configuration overrides.
/// </summary>
public class KnowledgeConfigOverrides
{
    /// <summary>
    /// Enables or disable the knowledge plugin invocaton and context in the prompt for this request.
    /// </summary>
    [JsonIgnore]
    public virtual bool? EnableKnowledgePlugin { get; set; }

    /// <summary>
    /// Knowledge search config overrides.
    /// </summary>
    [Required]
    public virtual KnowledgeSearchConfigOverrides Search { get; internal set; } = new();
}