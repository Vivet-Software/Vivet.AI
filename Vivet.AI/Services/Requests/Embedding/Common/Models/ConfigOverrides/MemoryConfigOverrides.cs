using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;

/// <summary>
/// Represents the class for built-in memory plugin configuration overrides.
/// </summary>
public class MemoryConfigOverrides
{
    /// <summary>
    /// Skips the memory invocaton and context in the prompt for this request.
    /// </summary>
    [JsonIgnore]
    public virtual bool SkipMemoryContext { get; set; } = false;

    /// <summary>
    /// Memory search config overrides.
    /// </summary>
    [Required]
    public virtual MemorySearchConfigOverrides Search { get; internal set; } = new();

    /// <summary>
    /// Memory index config overrides.
    /// </summary>
    [Required]
    public virtual MemoryIndexConfigOverrides Indexing { get; internal set; } = new(); 
}