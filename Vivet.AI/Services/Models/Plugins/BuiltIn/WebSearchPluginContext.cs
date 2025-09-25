namespace Vivet.AI.Services.Models.Plugins.BuiltIn;

/// <summary>
/// Represents the context for the built-in web search plugin.
/// </summary>
public class WebSearchPluginContext
{
    /// <summary>
    /// Number of search results to return for the web search.
    /// </summary>
    public virtual int Limit { get; init; } = 5;

    /// <summary>
    /// The url of the site to limit the search for.
    /// </summary>
    public virtual string Site { get; init; }
}