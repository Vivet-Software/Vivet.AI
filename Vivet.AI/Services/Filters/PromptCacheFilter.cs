using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Services.Filters;

/// <summary>
/// Enables semantic caching for prompts.
/// </summary>
public sealed class PromptCacheFilter : IPromptRenderFilter
{
    /// <inheritdoc />
    public Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // TODO: Prompt Render Filter: Prompt Caching
        // https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Caching/SemanticCachingWithFilters.cs

        return next(context);
    }
}