using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Linq;
using Vivet.AI.Config;

namespace Vivet.AI.Extensions;

internal static class KernelBuilderExtensions
{
    internal static IKernelBuilder AddChatPluginsFromConfiguration(this IKernelBuilder builder, IServiceCollection services, ChatOptions chatOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        if (chatOptions == null)
            throw new ArgumentNullException(nameof(chatOptions));

        var serviceProvider = services
            .BuildServiceProvider();

        var types = chatOptions.Plugins
            .Select(x => Type.GetType(x, true)); 

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }
    internal static IKernelBuilder AddMetadataPluginsFromConfiguration(this IKernelBuilder builder, IServiceCollection services, MetadataOptions metadataOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (metadataOptions == null)
            throw new ArgumentNullException(nameof(metadataOptions));

        var serviceProvider = services
            .BuildServiceProvider();

        var types = metadataOptions.Plugins
            .Select(x => Type.GetType(x, true));

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }
    internal static IKernelBuilder AddSummarizationPluginsFromConfiguration(this IKernelBuilder builder, IServiceCollection services, SummarizationOptions summarizationOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (summarizationOptions == null) 
            throw new ArgumentNullException(nameof(summarizationOptions));

        var serviceProvider = services
            .BuildServiceProvider();

        var types = summarizationOptions.Plugins
            .Select(x => Type.GetType(x, true));

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }


    private static void AddFromType(this IKernelBuilder kernelBuilder, Type type, IServiceProvider serviceProvider)
    {
        if (kernelBuilder == null) 
            throw new ArgumentNullException(nameof(kernelBuilder));

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
            .OrderByDescending(x => x.GetParameters().Length)
            .FirstOrDefault();

        if (constructorInfo == null)
        {
            throw new InvalidOperationException($"Plugin type {type.FullName} has no public constructor.");
        }

        var parameters = constructorInfo
            .GetParameters()
            .Select(x => serviceProvider.GetService(x.ParameterType) ?? throw new InvalidOperationException($"Cannot resolve constructor parameter {x.ParameterType.FullName} for plugin {type.FullName}"))
            .ToArray();

        var instance = constructorInfo
            .Invoke(parameters);

        kernelBuilder.Plugins
            .AddFromObject(instance, type.Name);
    }
}