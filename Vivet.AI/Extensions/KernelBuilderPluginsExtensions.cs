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

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (type == null)
            throw new ArgumentNullException(nameof(type));

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

        object instance;
        try
        {
            var parameters = constructorInfo
                .GetParameters()
                .Select(x => serviceProvider
                    .GetService(x.ParameterType))
                .ToArray();

            instance = constructorInfo
                .Invoke(parameters);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Plugin type {type.FullName} constructor can't be resolved. See inner exception for details.", ex);
        }

        kernelBuilderPlugins
            .AddFromObject(instance, name ?? type.Name);
    }
}
