using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vivet.AI.Services.Models.Plugins;

namespace Vivet.AI.Services.Extensions;

/// <summary>
/// String Builder Extensions.
/// </summary>
internal static class StringBuilderExtensions
{
    internal static StringBuilder AppendBuiltInPluginContext<TContext>(this StringBuilder stringBuilder, TContext context, string name)
        where TContext : class
    {
        if (stringBuilder == null) 
            throw new ArgumentNullException(nameof(stringBuilder));

        if (name == null) 
            throw new ArgumentNullException(nameof(name));

        if (context == null)
        {
            return stringBuilder;
        }

        var contextPrompt = StringBuilderExtensions.GetBuiltInPluginContext(context, name);

        if (contextPrompt != null)
        {
            stringBuilder
                .AppendLine(contextPrompt);
        }

        return stringBuilder;
    }

    internal static StringBuilder AppendCustomPluginsContext(this StringBuilder stringBuilder, IEnumerable<CustomPlugin> customPlugins)
    {
        if (stringBuilder == null)
            throw new ArgumentNullException(nameof(stringBuilder));

        if (customPlugins == null)
            throw new ArgumentNullException(nameof(customPlugins));

        foreach (var customPlugin in customPlugins)
        {
            if (!customPlugin.Context.Any())
            {
                continue;
            }

            var contextVariables = customPlugin.Context
                .Select(x => $"{x.Key}={x.Value}")
                .ToArray();

            if (!contextVariables.Any())
            {
                continue;
            }

            var contextString = $"{customPlugin.Name} Plugin Context: {string.Join(", ", contextVariables)}";

            stringBuilder
                .AppendLine(contextString);
        }

        return stringBuilder;
    }


    private static string GetBuiltInPluginContext<TContext>(TContext context, string pluginName)
        where TContext : class
    {
        if (context == null)
        {
            return null;
        }

        var contextString = GetContextString(context);

        if (contextString == null)
        {
            return null;
        }

        return $"{pluginName}: {string.Join(", ", contextString)}";
    }
    private static string GetContextString<TContext>(TContext context)
        where TContext : class
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var values = typeof(TContext)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(x =>
            {
                var value = x.GetValue(context);

                if (value == null)
                {
                    return null;
                }

                return $"{x.Name}={value}";
            })
            .Where(x => x != null)
            .ToArray();

        if (!values.Any())
        {
            return null;
        }

        return string.Join(", ", values);
    }
}