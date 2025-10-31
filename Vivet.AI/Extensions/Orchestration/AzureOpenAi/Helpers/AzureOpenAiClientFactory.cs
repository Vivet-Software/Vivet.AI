using System;
using System.ClientModel;
using Azure.AI.OpenAI;

namespace Vivet.AI.Extensions.Orchestration.AzureOpenAi.Helpers;

internal static class AzureOpenAiClientFactory
{
    internal static AzureOpenAIClient GetAzureOpenAiClient(string modelName, string endpoint, string apiKey, TimeSpan timeout)
    {
        if (endpoint == null)
            throw new ArgumentNullException(nameof(endpoint));

        var azureOpenAiClientOptions = new AzureOpenAIClientOptions
        {
            NetworkTimeout = timeout
        };
        var apiKeyCredential = new ApiKeyCredential(apiKey);
        
        return new AzureOpenAIClient(new Uri(endpoint), apiKeyCredential, azureOpenAiClientOptions);
    }
}