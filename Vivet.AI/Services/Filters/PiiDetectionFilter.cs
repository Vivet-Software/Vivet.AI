using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Services.Filters;

/// <summary>
/// Detects Personal Identifiable Information (PII)
/// and automatically redacts it before content is sent to the chat model.
/// </summary>
public sealed class PiiDetectionFilter : IPromptRenderFilter
{
    /// <inheritdoc />
    public Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // TODO: Prompt Render Filter: PII Identification
        // http://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PIIDetection.cs

        return next(context);
    }
}