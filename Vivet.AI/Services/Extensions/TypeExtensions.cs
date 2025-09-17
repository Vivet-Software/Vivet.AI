using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vivet.AI.Services.Models.Blobs;

namespace Vivet.AI.Services.Extensions;

internal static class TypeExtensions
{
    internal static Type GetMetadataType(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BaseBlobAdditionalMetadata<,>))
            {
                return type
                    .GetGenericArguments()[1];
            }

            type = type.BaseType;
        }

        return null;
    }

    internal static Dictionary<string, object> GenerateJsonMap(this Type type)
    {
        if (type == null) 
            throw new ArgumentNullException(nameof(type));
        
        var jsonMap = new Dictionary<string, object>();

        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var prop in properties)
        {
            jsonMap[prop.Name] = prop.PropertyType
                .GetTypeDefinition();
        }

        return jsonMap;
    }


    private static object GetTypeDefinition(this Type type)
    {
        if (type == null) 
            throw new ArgumentNullException(nameof(type));
        
        var jsonType = type
            .GetJsonType();

        switch (jsonType)
        {
            case "object":
                return GenerateJsonMap(type);

            case "array":
            {
                var elementType = type.IsArray
                    ? type
                        .GetElementType()
                    : type
                        .GetGenericArguments()
                        .FirstOrDefault();

                return new List<object>
                {
                    elementType != null 
                        ? elementType
                            .GetTypeDefinition() 
                        : "object"
                };
            }

            default:
                return jsonType;
        }
    }
    private static string GetJsonType(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type == typeof(string))
        {
            return "string";
        }

        if (type == typeof(bool))
        {
            return "boolean";
        }

        if (type.IsPrimitive || type == typeof(decimal) || type == typeof(double) || type == typeof(float))
        {
            return "number";
        }

        if (type == typeof(int) || type == typeof(long) || type == typeof(short))
        {
            return "integer";
        }

        if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
        {
            return "string";
        }

        if (type.IsArray || (type.IsGenericType && type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))))
        {
            return "array";
        }

        if (type.IsClass || type.IsValueType)
        {
            return "object";
        }
        
        return "string";
    }
}