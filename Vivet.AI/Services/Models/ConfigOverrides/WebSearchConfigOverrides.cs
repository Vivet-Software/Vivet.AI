using Newtonsoft.Json;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents the class for built-in web search plugin configuration overrides.
/// </summary>
public class WebSearchConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Skips the web search plugin invocaton and context in the prompt for this request.
    /// </summary>
    [JsonIgnore]
    public virtual bool SkipWebSearchContext { get; set; } = false;
}