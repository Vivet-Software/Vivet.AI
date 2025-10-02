using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;
using Vivet.AI.Services.Plugins.Consts;

namespace Vivet.AI.Services.Extensions;

internal static class KernelExtensions
{
    internal static Kernel AddFilters<T>(this Kernel kernel)
        where T : class
    {
        if (kernel == null) 
            throw new ArgumentNullException(nameof(kernel));
        
        var services = kernel.Services
            .GetServices<T>();

        foreach (var filter in services)
        {
            switch (filter)
            {
                case IFunctionInvocationFilter functionInvocationFilter:
                    kernel.FunctionInvocationFilters
                        .Add(functionInvocationFilter);
                    break;

                case IAutoFunctionInvocationFilter autoFunctionInvocationFilter:
                    kernel.AutoFunctionInvocationFilters
                        .Add(autoFunctionInvocationFilter);
                    break;

                case IPromptRenderFilter promptRenderFilter:
                    kernel.PromptRenderFilters
                        .Add(promptRenderFilter);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown filter type: {typeof(T)}");
            }
        }

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

    internal static Kernel AddBuiltInPluginConfigOverrides(this Kernel kernel, BuiltInPluginsConfigOverrides configOverrides, BuiltInPluginsConfigOverrides parentConfigOverrides = null)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        var skipMemoryContext = configOverrides.Memory?.SkipMemoryContext ?? parentConfigOverrides?.Memory?.SkipMemoryContext ?? false;
        if (skipMemoryContext)
        {
            kernel
                .RemovePlugin(BuiltInPluginNames.MEMORY_PLUGIN);
        }

        var skipKnowledgeContext = configOverrides.Knowledge?.SkipKnowledgeContext ?? parentConfigOverrides?.Knowledge?.SkipKnowledgeContext ?? false;
        if (skipKnowledgeContext)
        {
            kernel
                .RemovePlugin(BuiltInPluginNames.KNOWLEDGE_PLUGIN);
        }

        var skipWebSearchContext = configOverrides.WebSearch?.SkipWebSearchContext ?? parentConfigOverrides?.WebSearch?.SkipWebSearchContext ?? false;
        if (skipWebSearchContext)
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