using System.ComponentModel.DataAnnotations;
using Vivet.AI.Models.Enums;

namespace Vivet.AI.Config;

/// <summary>
/// Web Search Plugin Options.
/// </summary>
public class WebSearchPluginOptions
{
    /// <summary>
    /// The provider for the plugin to use when searching the web.
    /// </summary>
    public virtual WebSearchProvider Provider { get; set; }

    /// <summary>
    /// The identifier used for web search.
    /// <list type="bullet">
    /// <item>
    /// <b>Google:</b> The Search Engine ID.
    /// </item>
    /// <item>
    /// <b>Bing:</b> Not applicable.
    /// </item>
    /// </list>
    /// </summary>
    public virtual string Id { get; set; }

    /// <summary>
    /// The api-key of the web search provider.
    /// </summary>
    [Required]
    public virtual string ApiKey { get; set; }
}