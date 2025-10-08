using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;

namespace Vivet.AI.Services.Extensions;

/// <summary>
/// String Builder Extensions.
/// </summary>
internal static class StringBuilderExtensions
{
    internal static StringBuilder AppendBuiltInPluginContext<TContext>(this StringBuilder stringBuilder, string name, TContext context = null)
        where TContext : class
    {
        if (stringBuilder == null)
            throw new ArgumentNullException(nameof(stringBuilder));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var contextValues = GetComplexValue(nameof(context), context);

        if (contextValues == null)
        {
            return stringBuilder;
        }

        var value = $"{name}: {contextValues}";

        stringBuilder
            .AppendLine(value);

        return stringBuilder;
    }

    internal static StringBuilder AppendBuiltInPluginContext<TContext, TOverride>(this StringBuilder stringBuilder, string name, TContext context = null, TOverride configOverrides = null)
        where TContext : class
        where TOverride : BaseConfigOverrides
    {
        if (stringBuilder == null)
            throw new ArgumentNullException(nameof(stringBuilder));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var contextValues = GetComplexValue(nameof(context), context);
        var configOverridesValue = GetComplexValue(nameof(configOverrides), configOverrides);

        if (contextValues == null && configOverridesValue == null)
        {
            return stringBuilder;
        }

        var valueBuilder = new StringBuilder()
            .Append(name)
            .Append(": ");

        var parts = new[]
            {
                contextValues,
                configOverridesValue
            }
            .Where(x => x != null);

        valueBuilder
            .Append(string.Join(", ", parts));

        stringBuilder
            .AppendLine(valueBuilder.ToString());

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

            var contextValues = customPlugin.Context
                .Select(x =>
                {
                    var value = x.Value.GetType().IsSimple()
                        ? GetSimpleValue(x.Key, x.Value)
                        : GetComplexValue(x.Key, x.Value);

                    return value;
                })
                .Where(x => x != null)
                .ToArray();

            if (!contextValues.Any())
            {
                continue;
            }

            stringBuilder
                .AppendLine($"{customPlugin.Name}: {string.Join(", ", contextValues)}");
        }

        return stringBuilder;
    }


    private static string GetSimpleValue(string name, object value = null)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (value == null)
        {
            return null;
        }

        return $"{name}={value}";
    }
    private static string GetComplexValue(string name, object value = null)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (value == null)
        {
            return null;
        }

        var jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        var serializedObject = JsonConvert.SerializeObject(value, jsonSerializerSettings);

        var serializedValue = serializedObject
            .Replace("\"", "\\\"");

        return $"{name}={serializedValue}";
    }
}