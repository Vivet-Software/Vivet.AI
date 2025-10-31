using System;
using System.Net.Http;

namespace Vivet.AI.Extensions.Helpers;

internal static class HttpClientFactory
{
    internal static HttpClient GetHttpClient(string baseAddress, TimeSpan timeout)
    {
        if (baseAddress == null)
            throw new ArgumentNullException(nameof(baseAddress));

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress),
            Timeout = timeout
        };

        return httpClient;
    }
}