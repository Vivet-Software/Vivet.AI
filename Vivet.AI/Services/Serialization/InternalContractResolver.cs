using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Vivet.AI.Services.Serialization;

internal class InternalContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        if (type == null) 
            throw new ArgumentNullException(nameof(type));

        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(x => base.CreateProperty(x, memberSerialization))
            .ToList();

        foreach (var property in properties)
        {
            property.Writable = true;
            property.Readable = true;
        }

        return properties;
    }
}