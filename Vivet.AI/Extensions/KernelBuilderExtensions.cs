using Google.Apis.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Config;
using Vivet.AI.Config.Enums;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Plugins;
using Vivet.AI.Services.Interfaces;

namespace Vivet.AI.Extensions;

internal static class KernelBuilderExtensions
{
    internal static IKernelBuilder AddChatPluginsFromConfiguration(this IKernelBuilder builder, IServiceProvider serviceProvider)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null) 
            throw new ArgumentNullException(nameof(serviceProvider));

        var chatOptions = serviceProvider
            .GetRequiredService<ChatOptions>();

        builder
            .AddMemoryPlugin(serviceProvider, chatOptions.Plugins.BuiltInPlugins.Memory)
            .AddKnowledgePlugin(serviceProvider, chatOptions.Plugins.BuiltInPlugins.Knowledge)
            .AddWebSearchPlugin(chatOptions.Plugins.BuiltInPlugins.WebSearch);

        var types = chatOptions.Plugins.CustomPlugins
            .Select(x => Type.GetType(x, true)); 

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }
    
    internal static IKernelBuilder AddFilters<TFilter>(this IKernelBuilder builder, IServiceCollection services)
        where TFilter : class
    {
        if (builder == null) 
            throw new ArgumentNullException(nameof(builder));
        
        if (services == null) 
            throw new ArgumentNullException(nameof(services));
        
        var filterDescriptors = services.Where(sd => sd.ServiceType == typeof(TFilter));

        foreach (var filterDescriptor in filterDescriptors)
        {
            builder.Services
                .Add(filterDescriptor);
        }

        return builder;
    }


    private static IKernelBuilder AddMemoryPlugin(this IKernelBuilder builder, IServiceProvider serviceProvider, ChatMemoryPluginOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return builder;
        }

        var embeddingMemoryService = serviceProvider
            .GetService<IEmbeddingMemoryService>();

        if (embeddingMemoryService == null)
        {
            return builder;
        }

        var chatMemoryPlugin = new ChatMemoryPlugin(options, embeddingMemoryService);

        builder.Plugins
            .AddFromObject(chatMemoryPlugin);

        return builder;
    }
    private static IKernelBuilder AddKnowledgePlugin(this IKernelBuilder builder, IServiceProvider serviceProvider, ChatKnowledgePluginOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return builder;
        }

        var embeddingKnowledgeService = serviceProvider
            .GetService<IEmbeddingKnowledgeService>();

        if (embeddingKnowledgeService == null)
        {
            return builder;
        }

        var chatMemoryPlugin = new ChatKnowledgePlugin(options, embeddingKnowledgeService);

        builder.Plugins
            .AddFromObject(chatMemoryPlugin);

        return builder;
    }
    private static IKernelBuilder AddWebSearchPlugin(this IKernelBuilder builder, ChatWebSearchPluginOptions options = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return null;
        }

        var textSearchOptions = new TextSearchOptions
        {
            Top = options.Limit
        };

        List<KernelFunction> webSearchFunctions;

        switch (options.Provider)
        {
            case WebSearchProvider.Bing:
                var bingTextSearch = new BingTextSearch(options.ApiKey);

                var bingSearchFunc = bingTextSearch
                    .CreateSearch(searchOptions: textSearchOptions);

                var bingGetResultsFunc = bingTextSearch
                    .CreateGetSearchResults(searchOptions: textSearchOptions);

                var bingGetTextResultsFunc = bingTextSearch
                    .CreateGetTextSearchResults(searchOptions: textSearchOptions);

                webSearchFunctions =
                [
                    bingSearchFunc,
                    bingGetResultsFunc,
                    bingGetTextResultsFunc
                ];

                break;

            case WebSearchProvider.Google:
                var initializer = new BaseClientService.Initializer
                {
                    ApiKey = options.ApiKey
                };
                var googleTextSearch = new GoogleTextSearch(initializer, options.Id);

                var googleSearchFunc = googleTextSearch
                    .CreateSearch(searchOptions: textSearchOptions);

                var googleGetResultsFunc = googleTextSearch
                    .CreateGetSearchResults(searchOptions: textSearchOptions);

                var googleGetTextResultsFunc = googleTextSearch
                    .CreateGetTextSearchResults(searchOptions: textSearchOptions);

                webSearchFunctions =
                [
                    googleSearchFunc,
                    googleGetResultsFunc,
                    googleGetTextResultsFunc
                ];

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(options.Provider));
        }

        builder.Plugins
            .AddFromFunctions(BuiltInPluginNames.WEB_SEARCH_PLUGIN, webSearchFunctions);

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

        kernelBuilder.Plugins
            .AddFromObject(instance, type.Name);
    }
}