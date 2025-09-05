using System;
using Azure;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Extensions.Embeddings.AzureAiSearch;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddAzureAiSearchVectorStoreOptions<T>(this IServiceCollection services)
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

                return new AzureAISearchVectorStoreOptions
                {
                    EmbeddingGenerator = embeddingGenerator
                };
            });

        return services;
    }
  
    internal static IServiceCollection AddAzureAiSearchVectorStore<T>(this IServiceCollection services, VectorStoreOptions options)
        where T : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var serviceId = typeof(T).Name;
        var collectionName = typeof(T).Name;

        services
            .AddKeyedTransient(serviceId, (_, _) =>
            {
                var uri = new Uri(options.Host);
                var azureKeyCredential = new AzureKeyCredential(options.ApiKey);
                var searchClientOptions = new SearchClientOptions
                {
                    Serializer = new JsonObjectSerializer()
                };

                return new SearchIndexClient(uri, azureKeyCredential, searchClientOptions);
            })
            .AddKeyedAzureAISearchVectorStore(serviceId,
                x => x.GetRequiredKeyedService<SearchIndexClient>(serviceId),
                x => x.GetRequiredKeyedService<AzureAISearchVectorStoreOptions>(serviceId))
            .AddKeyedAzureAISearchCollection<T>(serviceId, collectionName,
                x => x.GetRequiredKeyedService<SearchIndexClient>(serviceId));

        return services;
    }
}