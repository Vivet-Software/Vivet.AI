using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Vivet.AI.Services.Collectors;

namespace Vivet.AI.Services.Filters;

/// <summary>
/// Fuction call collector invocation filter.
/// Collects all function calls and returns them in the response of the different requests.
/// </summary>
public sealed class FunctionCallCollectorFilter : IFunctionInvocationFilter
{
    /// <inheritdoc />
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));

        if (next == null) 
            throw new ArgumentNullException(nameof(next));
        
        await next(context);

        FunctionsCollectorContext.Functions
            .Add(context);
    }
}