namespace Vivet.AI.Services.Models.Plugins.BuiltIn;

// BUG: 111: use this. This is more an override, but I can't pass that to context or can i? I can in the method creating context if I pass overrides
// BUG: 111: Site uri? maybe it should be possible to add several search web plugins with different Sites. Include site in name? TESTING. Also document in readme

/// <summary>
/// Represents the context for the built-in web search plugin.
/// </summary>
public class WebSearchPluginContext
{
    /// <summary>
    /// Number of search results to return for the web search.
    /// </summary>
    public virtual int? Limit { get; init; } = 5; // BUG: 111: Maybe limit should not be configured but just a context to be set. Then default to 5 or something?

    /// <summary>
    /// The url of the site to limit the search for.
    /// </summary>
    public virtual string Site { get; init; } // BUG: 111: Implement
}