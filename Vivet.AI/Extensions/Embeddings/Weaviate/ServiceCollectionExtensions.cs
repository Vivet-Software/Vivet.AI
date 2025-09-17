using System;
using System.Net.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.Weaviate;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Extensions.Embeddings.Weaviate;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddWeaviateVectorStoreOptions<T>(this IServiceCollection services)
        where T : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var serviceId = typeof(T).Name;

        services
            .AddKeyedSingleton(serviceId, (x, _) =>
            {
                var embeddingGenerator = x
                    .GetRequiredService<IEmbeddingGenerator>();

                return new WeaviateVectorStoreOptions
                {
                    EmbeddingGenerator = embeddingGenerator,
                    HasNamedVectors = false
                };
            });

        return services;
    }

    internal static IServiceCollection AddWeaviateVectorStore<T>(this IServiceCollection services, VectorStoreOptions options)
        where T : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var serviceId = typeof(T).Name;
        var collectionName = typeof(T).Name;

        services
            .AddHttpClient(serviceId, (_, httpClient) =>
            {
                var uriBuilder = new UriBuilder(options.Host)
                {
                    Port = options.Port
                };

                httpClient.BaseAddress = uriBuilder.Uri;
                httpClient.Timeout = options.Timeout;

                if (options.ApiKey == null)
                {
                    httpClient.DefaultRequestHeaders
                        .Add("Authorization", $"Bearer {options.ApiKey}");
                }
            });

        services
            .AddKeyedWeaviateVectorStore(serviceId,
                x => x.GetRequiredService<IHttpClientFactory>()
                    .CreateClient(serviceId),
                x => x.GetRequiredKeyedService<WeaviateVectorStoreOptions>(serviceId))
            .AddKeyedWeaviateCollection<T>(serviceId, collectionName);

        return services;
    }
}
