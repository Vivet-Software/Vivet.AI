namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to web search in chat operations.
/// </summary>
public class WebSearchPluginOverrides
{
    /// <summary>
    /// Skips the web search invocaton and context in the prompt for this request.
    /// </summary>
    public virtual bool SkipWebSearchContext { get; set; } = false;
}