using System;
using System.Reflection;
using Amazon;

namespace Vivet.AI.Extensions.Orchestration.AmazonBedrock.Helpers;

internal static class AmazonBedrockRegionEndpointHelper
{
    internal static RegionEndpoint FromPropertyName(string name)
    {
        if (name == null) 
            throw new ArgumentNullException(nameof(name));

        var type = typeof(RegionEndpoint);

        var field = type
            .GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);

        if (field == null)
        {
            throw new ArgumentOutOfRangeException(nameof(name), "Invalid region specified.");
        }

        return field
            .GetValue(null) as RegionEndpoint;
    }
}