using System;
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
}