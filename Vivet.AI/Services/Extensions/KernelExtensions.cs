using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Config;
using Vivet.AI.Services.Filters;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;
using Vivet.AI.Services.Plugins.Consts;

namespace Vivet.AI.Services.Extensions;

internal static class KernelExtensions
{
    internal static Kernel AddDefaultFilters(this Kernel kernel)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        kernel.PromptRenderFilters
            .Add(new PiiDetectionFilter());

        kernel.PromptRenderFilters
            .Add(new PromptCacheFilter());

        kernel.AutoFunctionInvocationFilters
            .Add(new ComplexObjectDeserializationFilter());

        kernel.AutoFunctionInvocationFilters
            .Add(new AutoFunctionCallCollectorFilter());

        return kernel;
    }

    internal static Kernel AddCustomPlugins(this Kernel kernel, IServiceProvider serviceProvider, IEnumerable<CustomPlugin> plugins)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        if (serviceProvider == null) 
            throw new ArgumentNullException(nameof(serviceProvider));

        if (plugins == null)
            throw new ArgumentNullException(nameof(plugins));

        foreach (var requestPlugin in plugins)
        {
            var kernelPlugin = kernel.Plugins
                .FirstOrDefault(x => x.Name == requestPlugin.Name);

            if (kernelPlugin == null)
            {
                kernel.Plugins
                    .AddFromType(requestPlugin, serviceProvider);
            }
        }

        return kernel;
    }

    internal static Kernel RemoveSkippedBuiltInPlugins(this Kernel kernel, PluginsToggleOptions toggleOptions, BaseChatConfigOverrides configOverrides, BaseChatConfigOverrides parentConfigOverrides = null)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        if (!(configOverrides.Plugins.Memory.EnableMemoryPlugin ?? parentConfigOverrides?.Plugins.Memory.EnableMemoryPlugin ?? toggleOptions.EnableMemoryPlugin))
        {
            kernel
                .RemovePlugin(BuiltInPluginNames.MEMORY_PLUGIN);
        }

        if (!(configOverrides.Plugins.Knowledge.EnableKnowledgePlugin ?? parentConfigOverrides?.Plugins.Knowledge.EnableKnowledgePlugin ?? toggleOptions.EnableKnowledgePlugin))
        {
            kernel
                .RemovePlugin(BuiltInPluginNames.KNOWLEDGE_PLUGIN);
        }

        if (!(configOverrides.Plugins.WebSearch.EnableWebSearchPlugin ?? parentConfigOverrides?.Plugins.WebSearch.EnableWebSearchPlugin ?? toggleOptions.EnableWebSearchPlugin))
        {
            kernel
                .RemovePlugin(BuiltInPluginNames.WEB_SEARCH_PLUGIN);
        }

        return kernel;
    }


    private static void RemovePlugin(this Kernel kernel, string name)
    {
        if (kernel == null) 
            throw new ArgumentNullException(nameof(kernel));
        
        if (name == null) 
            throw new ArgumentNullException(nameof(name));
        
        var webSearchPlugin = kernel.Plugins
            .FirstOrDefault(x => x.Name == name);

        if (webSearchPlugin != null)
        {
            kernel.Plugins
                .Remove(webSearchPlugin);
        }
    }
}