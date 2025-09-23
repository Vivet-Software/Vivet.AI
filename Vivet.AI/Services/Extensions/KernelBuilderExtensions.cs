using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Plugins;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;

namespace Vivet.AI.Services.Extensions;

internal static class KernelBuilderExtensions
{
    internal static Kernel AddCustomPlugins(this Kernel kernel, IServiceProvider serviceProvider, IEnumerable<CustomPlugin> plugins)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        if (serviceProvider == null) 
            throw new ArgumentNullException(nameof(serviceProvider));

        if (plugins == null)
            throw new ArgumentNullException(nameof(plugins));

        foreach (var requestPlguin in plugins)
        {
            kernel.Plugins
                .AddFromType(requestPlguin, serviceProvider);
        }

        return kernel;
    }

    internal static Kernel AddPluginConfigOverridesOrDefault(this Kernel kernel, BaseChatConfigOverrides configOverrides)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        if (configOverrides?.Plugins == null)
        {
            return kernel;
        }

        if (configOverrides.Plugins.Memory is { SkipMemoryContext: true })
        {
            var memoryPlugin = kernel.Plugins
                .FirstOrDefault(x => x.Name == nameof(MemoryPlugin));

            if (memoryPlugin != null)
            {
                kernel.Plugins
                    .Remove(memoryPlugin);
            }
        }

        if (configOverrides.Plugins.Knowledge is { SkipKnowledgeContext: true })
        {
            var knowledgePlugin = kernel.Plugins
                .FirstOrDefault(x => x.Name == nameof(KnowledgePlugin));

            if (knowledgePlugin != null)
            {
                kernel.Plugins
                    .Remove(knowledgePlugin);
            }
        }

        if (configOverrides.Plugins.WebSearch is { SkipWebSearchContext: true })
        {
            var knowledgePlugin = kernel.Plugins
                .FirstOrDefault(x => x.Name == nameof(KnowledgePlugin));

            if (knowledgePlugin != null)
            {
                kernel.Plugins
                    .Remove(knowledgePlugin);
            }
        }

        return kernel;
    }
}