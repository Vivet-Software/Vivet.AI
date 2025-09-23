using System;
using System.Linq;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Extensions;

internal static class KernelBuilderPluginsExtensions
{
    internal static void AddFromType(this IKernelBuilderPlugins kernelBuilderPlugins, IServiceProvider serviceProvider, Type type, string name = null)
    {
        if (kernelBuilderPlugins == null)
            throw new ArgumentNullException(nameof(kernelBuilderPlugins));

        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (!typeof(object).IsAssignableFrom(type))
        {
            throw new InvalidOperationException($"Plugin type {type.FullName} is invalid.");
        }

        var constructorInfo = type
            .GetConstructors()
            .OrderByDescending(x => x
                .GetParameters().Length)
            .FirstOrDefault();

        if (constructorInfo == null)
        {
            throw new InvalidOperationException($"Plugin type {type.FullName} has no public constructor.");
        }

        var parameters = constructorInfo
            .GetParameters()
            .Select(x => serviceProvider
                .GetService(x.ParameterType))
            .ToArray();

        var instance = constructorInfo
            .Invoke(parameters);

        kernelBuilderPlugins
            .AddFromObject(instance, name ?? type.Name);
    }
}