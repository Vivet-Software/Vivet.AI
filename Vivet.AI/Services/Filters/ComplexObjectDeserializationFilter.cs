using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Vivet.AI.Services.Filters;

/// <summary>
/// AutoFunctionInvocationFilter that automatically deserializes string arguments into their corresponding complex object types using Newtonsoft.Json.
/// This allows plugin functions to accept complex parameters like nested classes even when the arguments are passed as JSON strings.
/// </summary>
public sealed class ComplexObjectDeserializationFilter : IAutoFunctionInvocationFilter
{
    private static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// Called by Semantic Kernel before a plugin function is invoked.
    /// Iterates over all arguments and deserializes any string arguments into their target complex type, replacing the argument in context.
    /// </summary>
    /// <param name="context">The auto-function invocation context containing the function and arguments.</param>
    /// <param name="next">The delegate to invoke the next filter or the function itself.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));
        
        if (next == null) 
            throw new ArgumentNullException(nameof(next));

        if (context.Arguments == null)
        {
            return Task.CompletedTask;
        }

        foreach (var argument in context.Arguments)
        {
            var parameter = context.Function.Metadata.Parameters
                .FirstOrDefault(x => x.Name == argument.Key);

            // BUG: 000: What about Guid, DateTime, DateTimeOffset, Nullable, TimeSpan, TimeOnly, DateOnly, etc
            if (parameter?.ParameterType == null || parameter.ParameterType.IsPrimitive || parameter.ParameterType == typeof(string))
            {
                continue;
            }

            if (argument.Value is string s)
            {
                var deserializedObject = JsonConvert.DeserializeObject(s, parameter.ParameterType, ComplexObjectDeserializationFilter.jsonSerializerSettings);

                context.Arguments[argument.Key] = deserializedObject;
            }
        }

        return next(context);
    }
}