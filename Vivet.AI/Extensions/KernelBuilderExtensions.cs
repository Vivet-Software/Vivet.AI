using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Plugins;
using Vivet.AI.Services.Plugins.Consts;

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

    internal static IKernelBuilder AddBuiltInPlugins(this IKernelBuilder builder, IServiceProvider serviceProvider, PluginsOptions pluginsOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (pluginsOptions == null)
            throw new ArgumentNullException(nameof(pluginsOptions));

        builder
            .AddMemoryPlugin(serviceProvider)
            .AddKnowledgePlugin(serviceProvider)
            .AddWebSearchPlugin(serviceProvider, pluginsOptions.WebSearch);

        return builder;
    }


    private static IKernelBuilder AddMemoryPlugin(this IKernelBuilder builder, IServiceProvider serviceProvider)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var embeddingMemoryService = serviceProvider
            .GetService<IEmbeddingMemoryService>();

        if (embeddingMemoryService == null)
        {
            return builder;
        }

        var memoryPlugin = new MemoryPlugin(embeddingMemoryService);

        builder.Plugins
            .AddFromObject(memoryPlugin, BuiltInPluginNames.MEMORY_PLUGIN);

        return builder;
    }
    private static IKernelBuilder AddKnowledgePlugin(this IKernelBuilder builder, IServiceProvider serviceProvider)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var embeddingKnowledgeService = serviceProvider
            .GetService<IEmbeddingKnowledgeService>();

        if (embeddingKnowledgeService == null)
        {
            return builder;
        }

        var knowledgePlugin = new KnowledgePlugin(embeddingKnowledgeService);

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