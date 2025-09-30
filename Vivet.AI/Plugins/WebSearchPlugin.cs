using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Models.Enums;

namespace Vivet.AI.Plugins;

/// <summary>
/// Web Search Plugin.
/// </summary>
public sealed class WebSearchPlugin
{
    private readonly ITextSearch textSearchService;
    private readonly WebSearchProvider provider;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="textSearchService">The concrete search service (Bing, Google, etc.).</param>
    /// <param name="provider">The provider of the web search (Bing, Google, etc.).</param>
    public WebSearchPlugin(ITextSearch textSearchService, WebSearchProvider provider)
    {
        this.textSearchService = textSearchService ?? throw new ArgumentNullException(nameof(textSearchService));
        this.provider = provider;
    }

    /// <summary>
    /// Perform a web search with optional limit and site filtering.
    /// </summary>
    [KernelFunction("simple")]
    [Description("Perform a web search and return simple text results.")]
    public async Task<IEnumerable<string>> SearchAsync([Description("The current user question or message")]string query, int limit = 5, string site = null, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return null;
        }
            
        var filter = this.GetTextSearchFilter(site);

        var options = new TextSearchOptions
        {
            Top = limit,
            Filter = filter
        };

        var response = await this.textSearchService
            .SearchAsync(query, options, cancellationToken)
            .ConfigureAwait(false);

        var results = await response.Results
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }

    /// <summary>
    /// Retrieve detailed search results (with URLs, snippets, etc.).
    /// </summary>
    [KernelFunction("detailed")]
    [Description("Perform a web search and return structured results with titles, URLs, and snippets.")]
    public async Task<IEnumerable<object>> GetSearchResultsAsync([Description("The current user question or message")]string query, int limit = 5, string site = null, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return null;
        }

        var filter = this.GetTextSearchFilter(site);

        var options = new TextSearchOptions
        {
            Top = limit,
            Filter = filter
        };

        var response = await this.textSearchService
            .GetSearchResultsAsync(query, options, cancellationToken)
            .ConfigureAwait(false);

        var results = await response.Results
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }

    /// <summary>
    /// Retrieve text-only search results.
    /// </summary>
    [KernelFunction("text")]
    [Description("Perform a web search and return text-focused results, such as titles and snippets.")]
    public async Task<IReadOnlyList<TextSearchResult>> GetTextSearchResultsAsync([Description("The current user question or message")]string query, int limit = 5, string site = null, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return null;
        }

        var filter = this.GetTextSearchFilter(site);

        var options = new TextSearchOptions
        {
            Top = limit,
            Filter = filter
        };

        var response = await this.textSearchService
            .GetTextSearchResultsAsync(query, options, cancellationToken)
            .ConfigureAwait(false);

        var results = await response.Results
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }

    private TextSearchFilter GetTextSearchFilter(string site = null)
    {
        var filter = new TextSearchFilter();

        if (site != null)
        {
            var fieldName = this.provider switch
            {
                WebSearchProvider.Bing => "site",
                WebSearchProvider.Google => "siteSearch",
                _ => null
            };

            if (fieldName != null)
            {
                filter
                    .Equality(fieldName, site);
            }
        }

        return filter;
    }
}