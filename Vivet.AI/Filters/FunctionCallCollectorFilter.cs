using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Vivet.AI.Filters.Models;

namespace Vivet.AI.Filters;

internal sealed class FunctionCallCollectorFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));

        if (next == null) 
            throw new ArgumentNullException(nameof(next));
        
        await next(context);
        
        var collector = FunctionCallCollectorContext.Current.Value;

        if (collector == null)
        {
            return;
        }

        if (context.Result.ValueType == typeof(string))
        {
        }
        else
        {
            //var aa = context.Result.GetValue<object>();
            //var b = JsonConvert.SerializeObject(aa);

            // BUG: 222: Can we make it typed using reflection using ValueType
            // So create a generic FunctionCallResult, hmmm the list will be the base class, so user will need to cast. Almost the same as object

            collector.Results
                .Add(new FunctionCallResult
                {
                    PluginName = context.Function.PluginName,
                    FunctionName = context.Function.Name,
                    Result = context.Result.GetValue<object>()
                    // BUG: 222: Arguments, maybe more data.
                });
        }
    }
}