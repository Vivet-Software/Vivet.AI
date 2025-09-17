using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.PgVector;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;
using Vivet.AI.Extensions.Embeddings.Postgres.Extensions;

namespace Vivet.AI.Extensions.Embeddings.Postgres;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddPostgresVectorStoreOptions<T>(this IServiceCollection services)
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

                return new PostgresVectorStoreOptions
                {
                    EmbeddingGenerator = embeddingGenerator,
                    Schema = serviceId
                };
            });

        return services;
    }
    
    internal static IServiceCollection AddPostgresVectorStore<T>(this IServiceCollection services, VectorStoreOptions options)
        where T : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var serviceId = typeof(T).Name;
        var collectionName = typeof(T).Name;

        var connectionstring = options
            .BuildConnectionString();

        services
            .AddKeyedPostgresVectorStore(serviceId,
                _ => connectionstring,
                x => x.GetRequiredKeyedService<PostgresVectorStoreOptions>(serviceId))
            .AddKeyedPostgresCollection<Guid, T>(serviceId, collectionName, 
                _ => connectionstring, 
                _ => new PostgresCollectionOptions());

        return services;
    }
}