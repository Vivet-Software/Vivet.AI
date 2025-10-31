using System;
using OllamaSharp;
using Vivet.AI.Extensions.Helpers;

namespace Vivet.AI.Extensions.Orchestration.Ollama.Helpers;

internal static class OllamaApiClientFactory
{
    internal static OllamaApiClient GetOllamaApiClient(string modelName, string endpoint, TimeSpan timeout)
    {
        if (modelName == null)
            throw new ArgumentNullException(nameof(modelName));

        if (endpoint == null)
            throw new ArgumentNullException(nameof(endpoint));

        var httpClient = HttpClientFactory.GetHttpClient(endpoint, timeout);

        return new OllamaApiClient(httpClient, modelName);
    }
}