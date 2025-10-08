using Microsoft.SemanticKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Vivet.AI.Services.Models.Plugins;
using Vivet.AI.Services.Models.Plugins.Contexts;
using Vivet.AI.Services.Plugins.Consts;

namespace Vivet.AI.Services.Extensions;

internal static class KernelPluginCollectionExtensions
{
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

    internal static void ValidateContext<TMemory, TKnowledge, TWebSearch>(this KernelPluginCollection kernelPluginCollection, BaseBuiltInContext<TMemory, TKnowledge, TWebSearch> pluginsContext, BaseBuiltInContext<TMemory, TKnowledge, TWebSearch> parentPluginsContext = null)
        where TMemory : class
        where TKnowledge : class
        where TWebSearch : class
    {
        if (kernelPluginCollection == null)
            throw new ArgumentNullException(nameof(kernelPluginCollection));

        if (pluginsContext == null) 
            throw new ArgumentNullException(nameof(pluginsContext));

        var contextMemory = pluginsContext.Memory ?? parentPluginsContext?.Memory;
        var contextKnowledge = pluginsContext.Knowledge ?? parentPluginsContext?.Knowledge;
        var contextWebSearch = pluginsContext.WebSearch ?? parentPluginsContext?.WebSearch;

        foreach (var kernelPlugin in kernelPluginCollection)
        {
            switch (kernelPlugin.Name)
            {
                case BuiltInPluginNames.MEMORY_PLUGIN:
                    ValidateContext<TMemory>(contextMemory, kernelPlugin.Name);
                    break;

                case BuiltInPluginNames.KNOWLEDGE_PLUGIN:
                    ValidateContext<TKnowledge>(contextKnowledge, kernelPlugin.Name);
                    break;

                case BuiltInPluginNames.WEB_SEARCH_PLUGIN:
                    ValidateContext<TWebSearch>(contextWebSearch, kernelPlugin.Name);
                    break;

                default:
                    continue; 
            }
        }
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