using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using System;
using System.Linq;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Plugins;
using Vivet.AI.Plugins.Consts;
using Vivet.AI.Services.Interfaces;

namespace Vivet.AI.Extensions;

internal static class KernelBuilderExtensions
{
    internal static IKernelBuilder AddLoggerFactory(this IKernelBuilder builder, IServiceProvider serviceProvider)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var loggerFactory = serviceProvider
            .GetService<ILoggerFactory>();

        if (loggerFactory != null)
        {
            builder.Services
                .AddSingleton(loggerFactory);
        }

        return builder;
    }

    internal static IKernelBuilder AddFilters<T>(this IKernelBuilder builder, IServiceProvider serviceProvider)
        where T : class
    {
        if (builder == null) 
            throw new ArgumentNullException(nameof(builder));
        
        var filters = serviceProvider
            .GetServices<T>();

        foreach (var filter in filters)
        {
            builder.Services
                .AddSingleton(filter);
        }

        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => x
                .GetTypes()
                .Where(y => !y.IsAbstract && y.IsClass && typeof(T).IsAssignableFrom(y)))
            .SelectMany(x => x);

        foreach (var type in types)
        {
            var service = serviceProvider
                .GetService(type);

            if (service is T instance)
            {
                builder.Services
                    .AddSingleton(instance);
            }
        }

        return builder;
    }

    internal static IKernelBuilder AddPlugins(this IKernelBuilder builder, IServiceProvider serviceProvider, PluginsOptions pluginsOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (pluginsOptions == null)
            throw new ArgumentNullException(nameof(pluginsOptions));

        builder
            .AddMemoryPlugin(serviceProvider, pluginsOptions.BuiltInPlugins.Memory)
            .AddKnowledgePlugin(serviceProvider, pluginsOptions.BuiltInPlugins.Knowledge)
            .AddWebSearchPlugin(serviceProvider, pluginsOptions.BuiltInPlugins.WebSearch);

        var typeAndNames = pluginsOptions.CustomPlugins
            .Select(x => new
            {
                x.Name,
                Type = Type.GetType(x.Type, true)
            });

        foreach (var typeAndName in typeAndNames)
        {
            builder.Plugins
                .AddFromType(serviceProvider, typeAndName.Type, typeAndName.Name);
        }

        return builder;
    }


    private static IKernelBuilder AddMemoryPlugin(this IKernelBuilder builder, IServiceProvider serviceProvider, MemoryPluginOptions options = null)
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

        var memoryPlugin = new MemoryPlugin(options, embeddingMemoryService);

        builder.Plugins
            .AddFromObject(memoryPlugin, BuiltInPluginNames.MEMORY_PLUGIN);

        return builder;
    }
    private static IKernelBuilder AddKnowledgePlugin(this IKernelBuilder builder, IServiceProvider serviceProvider, KnowledgePluginOptions options = null)
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

        var knowledgePlugin = new KnowledgePlugin(options, embeddingKnowledgeService);

        builder.Plugins
            .AddFromObject(knowledgePlugin, BuiltInPluginNames.KNOWLEDGE_PLUGIN);

        return builder;
    }
    private static IKernelBuilder AddWebSearchPlugin(this IKernelBuilder builder, IServiceProvider serviceProvider, WebSearchPluginOptions options = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return null;
        }

        var textSearch = serviceProvider
            .GetKeyedService<ITextSearch>(ServiceIds.CHAT_SERVICE_ID);
        
        if (textSearch == null)
        {
            return null;
        }

        var webSearchPlugin = new WebSearchPlugin(textSearch, options.Provider);

        builder.Plugins
            .AddFromObject(webSearchPlugin, BuiltInPluginNames.WEB_SEARCH_PLUGIN);

        return builder;
    }
}