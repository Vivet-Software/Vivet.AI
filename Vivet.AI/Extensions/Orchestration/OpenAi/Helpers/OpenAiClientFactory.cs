using OpenAI;
using System;
using System.ClientModel;

namespace Vivet.AI.Extensions.Orchestration.OpenAi.Helpers;

internal static class OpenAiClientFactory
{
    internal static OpenAIClient GetOpenAiClient(string modelName, string endpoint, string apiKey, TimeSpan timeout)
    {
        if (endpoint == null)
            throw new ArgumentNullException(nameof(endpoint));

        var openAiClientOptions = new OpenAIClientOptions
        {
            NetworkTimeout = timeout,
            Endpoint = new Uri(endpoint)
        };
        var apiKeyCredential = new ApiKeyCredential(apiKey);

        return new OpenAIClient(apiKeyCredential, openAiClientOptions);
    }
}