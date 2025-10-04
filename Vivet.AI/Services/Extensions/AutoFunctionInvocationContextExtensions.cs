using System;
using System.Linq;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Extensions;

internal static class AutoFunctionInvocationContextExtensions
{
    internal static FunctionCall GetFunctionCall(this AutoFunctionInvocationContext context)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));

        var id = context.ToolCallId;
        var pluginName = context.Function.PluginName;
        var functionName = context.Function.Name;
        var valueType = context.Result.ValueType;

        var value = context.Result
            .GetValue<object>();

        var arguments = context.Arguments?
            .ToDictionary() ?? [];

        var serializedValue = JsonConvert.SerializeObject(value);

        var renderedPrompt = @$"{nameof(AuthorRole.Assistant)}
[{nameof(FunctionCallContent)}]
{nameof(AuthorRole.Tool)}
{serializedValue}
[{nameof(FunctionResultContent)}]";

        return new FunctionCall
        {
            Id = id,
            PluginName = pluginName,
            FunctionName = functionName,
            Result = new FunctionCallResult
            {
                Type = valueType,
                Result = value
            },
            RenderedPrompt = renderedPrompt,
            Arguments = arguments,
            Exception = null
        };
    }
}