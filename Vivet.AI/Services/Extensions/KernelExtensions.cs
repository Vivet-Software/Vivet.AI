using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Services.Consts;
using Vivet.AI.Services.Filters;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;
using Vivet.AI.Services.Plugins.Consts;
using Vivet.AI.Services.Responses.Agent.Models;

namespace Vivet.AI.Services.Extensions;

internal static class KernelExtensions
{
    // BUG: 111: Could these methods be moved somewhere else? Check all extension methods, and clean up a bit.

    internal static IEnumerable<AutoFunctionInvocationContext> GetAutoFunctionInvocationContexts(this Kernel kernel)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        var value = kernel.Data[KernelData.FUNCTION_CALLS];

        return (IEnumerable<AutoFunctionInvocationContext>)value;
    }

    internal static void AddAutoFunctionInvocationContext(this Kernel kernel, AutoFunctionInvocationContext functionInvocationContext)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        if (functionInvocationContext == null)
            throw new ArgumentNullException(nameof(functionInvocationContext));

        var functionCalls = (IList<AutoFunctionInvocationContext>)kernel.Data[KernelData.FUNCTION_CALLS];

        functionCalls?
            .Add(functionInvocationContext);
    }

    internal static Guid? GetAgentId(this Kernel kernel)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        var value = kernel.Data[KernelData.AGENT_ID];

        var strValue = value
            .ToString();

        return strValue == null 
            ? null 
            : Guid.Parse(strValue);
    }

    internal static AgentResponseCallback GetAgentResponseCallback(this Kernel kernel)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        var value = kernel.Data[KernelData.AGENT_RESPONSE_CALLBACK];

        return (AgentResponseCallback)value;
    }




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

    internal static Kernel RemoveSkippedBuiltInPlugins(this Kernel kernel, BaseChatConfigOverrides configOverrides, BaseChatConfigOverrides parentConfigOverrides = null)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        if (parentConfigOverrides?.SkipMemoryContext is true || configOverrides.SkipMemoryContext)
        {
            kernel
                .RemovePlugin(BuiltInPluginNames.MEMORY_PLUGIN);
        }

        if (parentConfigOverrides?.SkipKnowledgeContext is true || configOverrides.SkipKnowledgeContext)
        {
            kernel
                .RemovePlugin(BuiltInPluginNames.KNOWLEDGE_PLUGIN);
        }

        if (parentConfigOverrides?.SkipWebSearchContext is true || configOverrides.SkipWebSearchContext)
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