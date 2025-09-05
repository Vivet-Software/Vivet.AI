using System.Linq;
using System.Reflection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Config;
using Vivet.AI.Data.Annotations;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Extensions;

internal static class VectorStoreTextSearchExtensions
{
    internal static KernelFunction CreateSearchFunction<T>(this VectorStoreTextSearch<T> textSearch, ChatOptions chatOptions)
        where T : BaseEmbedding
    {
        var functionName = string.Format(KernelBuilderExtensions.VectorStoreSearchFunctionNameTemplate, typeof(T).Name);

        var parameters = typeof(T)
            .GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
            .Select(x =>
            {
                var parameterAttribute = x
                    .GetCustomAttribute<TextSearchParameterAttribute>();

                if (parameterAttribute == null)
                {
                    return null;
                }

                return new KernelParameterMetadata(x.Name)
                {
                    Description = parameterAttribute.Description,
                    DefaultValue = parameterAttribute.DefaultValue,
                    IsRequired = parameterAttribute.IsRequired
                };
            })
            .ToList();

        var limit = typeof(T) switch
        {
            var x when x == typeof(Memory) => chatOptions.Memory.ContextQueryLimit,
            var x when x == typeof(Knowledge) => chatOptions.Knowledge.ContextQueryLimit,
            _ => 5
        };

        parameters
            .AddRange(
            [
                new KernelParameterMetadata("skip") { IsRequired = true, DefaultValue = 0 },
                new KernelParameterMetadata("limit") { IsRequired = true, DefaultValue = limit },
                new KernelParameterMetadata("TenantId") { IsRequired = false },
                new KernelParameterMetadata("SubTenantId") { IsRequired = false },
                new KernelParameterMetadata("UserId") { IsRequired = false }
            ]);

        var options = new KernelFunctionFromMethodOptions
        {
            FunctionName = functionName,
            Description = null,
            Parameters = parameters,
            ReturnParameter = new KernelReturnParameterMetadata
            {
                ParameterType = typeof(KernelSearchResults<string>)
            }
        };

        return textSearch
            .CreateGetTextSearchResults(options);
    }
}