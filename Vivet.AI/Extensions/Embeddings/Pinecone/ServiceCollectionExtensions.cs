using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.Pinecone;
using Pinecone;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Extensions.Embeddings.Pinecone;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddPineconeVectorStoreOptions<T>(this IServiceCollection services)
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

                return new PineconeVectorStoreOptions
                {
                    EmbeddingGenerator = embeddingGenerator
                };
            });

        return services;
    }
    internal static IServiceCollection AddPineconeVectorStore<T>(this IServiceCollection services, VectorStoreOptions options)
        where T : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var serviceId = typeof(T).Name;
        var collectionName = typeof(T).Name;

        var clientOptions = new ClientOptions
        {
            BaseUrl = $"{options.Host}:{options.Port}",
            Timeout = options.Timeout
        };

        services
            .AddKeyedTransient(serviceId, (_, _) => new PineconeClient(options.ApiKey, clientOptions))  
            .AddKeyedPineconeVectorStore(serviceId,
                x => x.GetRequiredKeyedService<PineconeClient>(serviceId),
                x => x.GetRequiredKeyedService<PineconeVectorStoreOptions>(serviceId))
            .AddKeyedPineconeCollection<T>(serviceId, collectionName, 
                x => x.GetRequiredKeyedService<PineconeClient>(serviceId));

        return services;
    }
}