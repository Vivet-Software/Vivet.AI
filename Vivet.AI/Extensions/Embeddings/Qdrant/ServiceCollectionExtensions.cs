using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Extensions.Embeddings.Qdrant;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddQdrantVectorStoreOptions<T>(this IServiceCollection services)
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

                return new QdrantVectorStoreOptions
                {
                    EmbeddingGenerator = embeddingGenerator,
                    HasNamedVectors = false
                };
            });

        return services;
    }
    
    internal static IServiceCollection AddQdrantVectorStore<T>(this IServiceCollection services, VectorStoreOptions options)
        where T : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var serviceId = typeof(T).Name;
        var collectionName = typeof(T).Name;

        services
            .AddKeyedTransient(serviceId, (_, _) => new QdrantClient(options.Host, options.Port, options.UseSsl, options.ApiKey, options.Timeout))
            .AddKeyedQdrantVectorStore(serviceId, 
                x => x.GetRequiredKeyedService<QdrantClient>(serviceId), 
                x => x.GetRequiredKeyedService<QdrantVectorStoreOptions>(serviceId))
            .AddKeyedQdrantCollection<Guid, T>(serviceId, collectionName, 
                x => x.GetRequiredKeyedService<QdrantClient>(serviceId));

        return services;
    }
}