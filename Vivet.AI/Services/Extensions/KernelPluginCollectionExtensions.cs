using Microsoft.SemanticKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Vivet.AI.Plugins.Consts;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;

namespace Vivet.AI.Services.Extensions;

internal static class KernelPluginCollectionExtensions
{
    internal static void ValidateContext<TMemory, TKnowledge, TWebSearch>(this KernelPluginCollection kernelPluginCollection, BaseBuiltInPluginsContext<TMemory, TKnowledge, TWebSearch> pluginsContext, BuiltInPluginsConfigOverrides configOverrides)
        where TMemory : class
        where TKnowledge : class
        where TWebSearch : class
    {
        if (kernelPluginCollection == null)
            throw new ArgumentNullException(nameof(kernelPluginCollection));

        if (pluginsContext == null) 
            throw new ArgumentNullException(nameof(pluginsContext));

        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        foreach (var kernelPlugin in kernelPluginCollection)
        {
            switch (kernelPlugin.Name)
            {
                case BuiltInPluginNames.MEMORY_PLUGIN:
                    if (!configOverrides.Memory.SkipMemoryContext)
                    {
                        ValidateContext<TMemory>(pluginsContext.Memory, kernelPlugin.Name);
                    }
                    break;

                case BuiltInPluginNames.KNOWLEDGE_PLUGIN:
                    if (!configOverrides.Knowledge.SkipKnowledgeContext)
                    {
                        ValidateContext<TKnowledge>(pluginsContext.Knowledge, kernelPlugin.Name);
                    }
                    break;

                case BuiltInPluginNames.WEB_SEARCH_PLUGIN:
                    if (!configOverrides.WebSearch.SkipWebSearchContext)
                    {
                        ValidateContext<TWebSearch>(pluginsContext.WebSearch, kernelPlugin.Name);
                    }
                    break;

                default:
                    continue; 
            }
        }
    }

    internal static void AddFromType(this KernelPluginCollection kernelPluginCollection, CustomPlugin customPlugin, IServiceProvider serviceProvider)
    {
        if (kernelPluginCollection == null)
            throw new ArgumentNullException(nameof(kernelPluginCollection));

        if (customPlugin == null)
            throw new ArgumentNullException(nameof(customPlugin));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (!typeof(object).IsAssignableFrom(customPlugin.Type))
        {
            throw new InvalidOperationException($"Plugin type {customPlugin.Type.FullName} is invalid.");
        }

        var constructorInfo = customPlugin.Type
            .GetConstructors()
            .OrderByDescending(x => x
                .GetParameters().Length)
            .FirstOrDefault();

        if (constructorInfo == null)
        {
            throw new InvalidOperationException($"Plugin type {customPlugin.Type.FullName} has no public constructor.");
        }

        var parameters = constructorInfo
            .GetParameters()
            .Select(x => serviceProvider
                .GetService(x.ParameterType))
            .ToArray();

        var instance = constructorInfo
            .Invoke(parameters);

        kernelPluginCollection
            .AddFromObject(instance, customPlugin.Name ?? customPlugin.Type.Name);
    }


    private static void ValidateContext<TContext>(object contextObj, string name)
    {
        if (contextObj != null)
        {
            return;
        }

        var hasRequiredProperties = typeof(TContext)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(x => x.GetCustomAttribute<RequiredAttribute>(true) != null);

        if (hasRequiredProperties)
        {
            throw new InvalidOperationException($"The context for plugin '{name}' is null. Context must be included in the request when the built-in plugin is enabled.");
        }
    }
}