using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Vivet.AI.Services.Extensions;

internal static class ObjectExtensions
{
    internal static T TryGetPropertyValue<T>(this object obj, string propName)
    {
        if (obj == null) 
            throw new ArgumentNullException(nameof(obj));

        var propertyInfo = obj
            .GetType()
            .GetProperty(propName);

        var value = propertyInfo?
            .GetValue(obj);

        if (value == null)
        {
            return default;
        }

        try
        {
            if (value is T tValue)
            {
                return tValue;
            }

            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            var converted = Convert.ChangeType(value, targetType);

            return (T)converted;
        }
        catch
        {
            return default;
        }
    }

    internal static IEnumerable<object> TryGetEnumerableProperty(this object obj, string propName)
    {
        if (obj == null) 
            throw new ArgumentNullException(nameof(obj));

        if (propName == null) 
            throw new ArgumentNullException(nameof(propName));

        var prop = obj
            .GetType()
            .GetProperty(propName);
        
        var value = prop?
            .GetValue(obj);

        if (value is IEnumerable enumerable and not string)
        {
            return enumerable
                .Cast<object>();
        }

        return null;
    }

    internal static void Validate(this object @object)
    {
        if (@object == null)
            throw new ArgumentNullException(nameof(@object));

        var results = new List<ValidationResult>();
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        @object
            .ValidateObjectRecursive(results, visited);

        if (results.Any())
        {
            var message = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            throw new ValidationException(message, null, results);
        }
    }


    private static void ValidateObjectRecursive(this object @object, List<ValidationResult> results, HashSet<object> visited)
    {
        if (@object == null)
            throw new ArgumentNullException(nameof(@object));

        if (results == null)
            throw new ArgumentNullException(nameof(results));

        if (!visited.Add(@object))
        {
            return;
        }

        var context = new ValidationContext(@object, null, null);

        Validator.TryValidateObject(@object, context, results, false);

        var properties = @object
            .GetType()
            .GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                continue;
            }

            var value = property
                .GetValue(@object);

            switch (value)
            {
                case null:
                    continue;

                case IEnumerable<object> enumerable:
                {
                    foreach (var item in enumerable.Where(x => x != null && ShouldRecurse(x.GetType())))
                    {
                        item
                            .ValidateObjectRecursive(results, visited);
                    }

                    break;
                }
                
                default:
                {
                    if (ShouldRecurse(property.PropertyType))
                    {
                        value
                            .ValidateObjectRecursive(results, visited);
                    }

                    break;
                }
            }
        }
    }
    private static bool ShouldRecurse(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
        
        return !type.IsPrimitive &&
               !type.IsEnum &&
               type != typeof(string) &&
               !type.IsValueType &&
               type != typeof(Type);
    }
    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        // ReSharper disable MemberHidesStaticFromOuterClass
        public new bool Equals(object x, object y) => ReferenceEquals(x, y);
        // ReSharper restore MemberHidesStaticFromOuterClass
        
        public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}