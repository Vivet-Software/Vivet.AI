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
}