using System;
using Amazon.BedrockRuntime;
using Amazon.Runtime;

namespace Vivet.AI.Extensions.Orchestration.AmazonBedrock.Helpers;

internal static class AmazonBedrockRuntimeClientFactory
{
    internal static AmazonBedrockRuntimeClient GetAmazonBedrockRuntimeClient(string modelName, string endpoint, string apiKeyId, string apiKey, TimeSpan timeout)
    {
        if (endpoint == null)
            throw new ArgumentNullException(nameof(endpoint));

        var region = AmazonBedrockRegionEndpointHelper.FromPropertyName(endpoint);
        var awsCredentials = new BasicAWSCredentials(apiKeyId, apiKey);
        var amazonBedrockRuntimeConfig = new AmazonBedrockRuntimeConfig
        {
            RegionEndpoint = region,
            Timeout = timeout
        };

        return new AmazonBedrockRuntimeClient(awsCredentials, amazonBedrockRuntimeConfig);
    }
}