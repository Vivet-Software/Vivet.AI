using Microsoft.SemanticKernel;
using System;
using System.Threading.Tasks;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Services.Filters;

/// <summary>
/// Auto function filter used to collect function calls and add it to the kernel data.
/// </summary>
public sealed class AutoFunctionCallCollectorFilter : IAutoFunctionInvocationFilter
{
    /// <inheritdoc />
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));
        
        if (next == null) 
            throw new ArgumentNullException(nameof(next));

        await next(context);

        context.Kernel
            .AddAutoFunctionInvocationContext(context);
    }
}