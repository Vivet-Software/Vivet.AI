using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Linq;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;
using Vivet.AI.Plugins.TextSearch.Mappers;

namespace Vivet.AI.Extensions;

internal static class KernelBuilderExtensions
{
    internal static IKernelBuilder AddChatPluginsFromConfiguration(this IKernelBuilder builder, IServiceProvider serviceProvider, ChatOptions chatOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null) 
            throw new ArgumentNullException(nameof(serviceProvider));

        if (chatOptions == null)
            throw new ArgumentNullException(nameof(chatOptions));

        var types = chatOptions.Plugins
            .Select(x => Type.GetType(x, true)); 

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }
    internal static IKernelBuilder AddMetadataPluginsFromConfiguration(this IKernelBuilder builder, IServiceProvider serviceProvider, MetadataOptions metadataOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (metadataOptions == null)
            throw new ArgumentNullException(nameof(metadataOptions));

        var types = metadataOptions.Plugins
            .Select(x => Type.GetType(x, true));

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }
    internal static IKernelBuilder AddSummarizationPluginsFromConfiguration(this IKernelBuilder builder, IServiceProvider serviceProvider, SummarizationOptions summarizationOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (summarizationOptions == null) 
            throw new ArgumentNullException(nameof(summarizationOptions));

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



    // BUG: Memory / Knowledge

    internal static string VectorStoreSearchPluginNameTemplate = "Search{0}Plugin";
    internal static string VectorStoreSearchFunctionNameTemplate = "Search{0}";


    internal static IKernelBuilder AddVectorStoreSearches(this IKernelBuilder builder, IServiceProvider serviceProvider)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        builder
            .AddVectorStoreSearch<Knowledge>(serviceProvider)
            .AddVectorStoreSearch<Memory>(serviceProvider);

        return builder;
    }


    private static IKernelBuilder AddVectorStoreSearch<TEmbedding>(this IKernelBuilder builder, IServiceProvider serviceProvider)
        where TEmbedding : BaseEmbedding
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var serviceId = typeof(TEmbedding).Name;

        var textSearchStringMapper = new EmbeddingTextSearchStringMapper();
        var textSearchResultMapper = new EmbeddingTextSearchResultMapper();
        var textSearchOptions = new VectorStoreTextSearchOptions();

        builder
            .AddVectorStoreTextSearch<TEmbedding>(textSearchStringMapper, textSearchResultMapper, textSearchOptions);

        var textSearch = serviceProvider
            .GetRequiredKeyedService<VectorStoreTextSearch<TEmbedding>>(serviceId);

        var chatOptions = serviceProvider
            .GetRequiredService<ChatOptions>();

        var kernelFunction = textSearch
            .CreateSearchFunction(chatOptions);

        var pluginName = string.Format(KernelBuilderExtensions.VectorStoreSearchPluginNameTemplate, typeof(TEmbedding).Name);

        var searchPlugin = KernelPluginFactory.CreateFromFunctions(pluginName, null, [kernelFunction]);

        builder.Plugins
            .Add(searchPlugin);

        return builder;
    }
}