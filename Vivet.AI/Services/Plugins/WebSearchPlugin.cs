using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Models.Enums;
using Vivet.AI.Services.Models.Plugins.Contexts;

namespace Vivet.AI.Services.Plugins;

/// <summary>
/// Web Search Plugin.
/// </summary>
public sealed class WebSearchPlugin
{
    private readonly WebSearchProvider provider;
    private readonly ITextSearch textSearchService;

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
    /// <param name="query">The current user question or message.</param>
    /// <param name="context">The context for the web search.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The web serarch results.</returns>
    [KernelFunction("simple")]
    [Description("Perform a web search and return simple text results.")]
    public async Task<IEnumerable<string>> SearchAsync([Description("The current user question or message")]string query, WebSearchContext context, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return null;
        }

        try
        {
            var filter = this.GetTextSearchFilter(context.Site);

            var options = new TextSearchOptions
            {
                Top = context.Limit,
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
        catch (Exception ex)
        {
            return [$"An error occurred. {ex.Message}"];
        }
    }

    /// <summary>
    /// Retrieve detailed search results (with URLs, snippets, etc.).
    /// </summary>
    /// <param name="query">The current user question or message.</param>
    /// <param name="context">The context for the web search.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The web serarch results.</returns>
    [KernelFunction("detailed")]
    [Description("Perform a web search and return structured results with titles, URLs, and snippets.")]
    public async Task<IEnumerable<object>> GetSearchResultsAsync([Description("The current user question or message")]string query, WebSearchContext context, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return null;
        }

        try
        {
            var filter = this.GetTextSearchFilter(context.Site);

            var options = new TextSearchOptions
            {
                Top = context.Limit,
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
        catch (Exception ex)
        {
            return [$"An error occurred. {ex.Message}"];
        }
    }

    /// <summary>
    /// Retrieve text-only search results.
    /// </summary>
    /// <param name="query">The current user question or message.</param>
    /// <param name="context">The context for the web search.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The web serarch results.</returns>
    [KernelFunction("text")]
    [Description("Perform a web search and return text-focused results, such as titles and snippets.")]
    public async Task<IReadOnlyList<TextSearchResult>> GetTextSearchResultsAsync([Description("The current user question or message")]string query, WebSearchContext context, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return null;
        }

        try
        {
            var filter = this.GetTextSearchFilter(context.Site);

            var options = new TextSearchOptions
            {
                Top = context.Limit,
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
        catch (Exception ex)
        {
            return [new TextSearchResult($"An error occurred. {ex.Message}")];
        }
    }


    private TextSearchFilter GetTextSearchFilter(string site = null)
    {
        var filter = new TextSearchFilter();

        if (site == null)
        {
            return filter;
        }

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

        return filter;
    }
}