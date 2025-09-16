using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Net.Http;
using Vivet.AI.Config;
using Vivet.AI.Config.Enums;
using Vivet.AI.Data.Definitions;
using Vivet.AI.Data.Models;
using Vivet.AI.Data.Stores;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Extensions.Embeddings.AzureAiSearch;
using Vivet.AI.Extensions.Embeddings.Pinecone;
using Vivet.AI.Extensions.Embeddings.Postgres;
using Vivet.AI.Extensions.Embeddings.Qdrant;
using Vivet.AI.Extensions.Embeddings.Weaviate;
using Vivet.AI.Models;
using Vivet.AI.Services;
using Vivet.AI.Services.Interfaces;
using ChatOptions = Vivet.AI.Config.ChatOptions;

namespace Vivet.AI.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddOptions(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton(options);

        return services;
    }
    internal static IServiceCollection AddConfigOptions(this IServiceCollection services, out AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection(AiOptions.SectionName);

        options = section.Get<AiOptions>() ?? new AiOptions();

        services
            .AddSingleton(options)
            .Configure<AiOptions>(section);

        return services;
    }

    internal static IServiceCollection AddChatServices<T>(this IServiceCollection services, AiOptions options)
        where T : PromptExecutionSettings, new()
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton(options.Chat);

        services
            .AddKeyedSingleton(ServiceIds.CHAT_SERVICE_ID, (x, _) =>
            {
                var builder = Kernel.CreateBuilder();

                builder
                    .AddChatPluginsFromConfiguration(x);

                return builder;
            });

        services
            .AddPromptExecutionSettings<T>(options.Chat.Model.Parameters, ServiceIds.CHAT_SERVICE_ID)
            .AddScoped<IChatService>(x =>
            {
                var chatOptions = x
                    .GetRequiredService<ChatOptions>();

                var chatCompletionService = x
                    .GetRequiredKeyedService<IChatCompletionService>(ServiceIds.CHAT_SERVICE_ID);

                var kernelBuilder = x
                    .GetRequiredKeyedService<IKernelBuilder>(ServiceIds.CHAT_SERVICE_ID);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(ServiceIds.CHAT_SERVICE_ID);

                var embeddingMemoryService = x
                    .GetService<IEmbeddingMemoryService>();

                return new ChatService(chatOptions, chatCompletionService, kernelBuilder, promptExecutionSettings, embeddingMemoryService);
            });

        services
            .AddHealthCheckPromptExecutionSettings<T>(ServiceIds.HEALTH_CHAT_SERVICE_ID)
            .AddHealthChecks()
            .AddChatModelCheck(ServiceIds.CHAT_SERVICE_ID, ServiceIds.HEALTH_CHAT_SERVICE_ID);

        return services;
    }
    internal static IServiceCollection AddEmbeddingServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Embedding == null)
        {
            return services;
        }

        services
            .AddSingleton(options.Embedding);

        services
            .AddScoped<IEmbeddingGenerator>(x => x
                .GetRequiredKeyedService<IEmbeddingGenerator<string, Embedding<float>>>(serviceKey: ServiceIds.EMBEDDING_SERVICE_ID));

        services
            .AddSingleton(_ => EmbeddingVectorStoreCollectionDefinition.GetVectorStoreCollectionDefinition(options.Embedding.VectorSize));

        if (options.Embedding.Memory?.VectorStore != null)
        {
            services
                .AddSingleton(options.Embedding.Memory)
                .AddMemoryVectorStore(options.Embedding.Memory)
                .AddScoped<IEmbeddingMemoryService>(x =>
                {
                    var embeddingOptions = x
                        .GetRequiredService<EmbeddingOptions>();

                    var embeddingGenerator = x
                        .GetRequiredKeyedService<IEmbeddingGenerator<string, Embedding<float>>>(serviceKey: ServiceIds.EMBEDDING_SERVICE_ID);

                    var memoryVectorStore = x
                        .GetRequiredService<MemoryVectorStore>();

                    var metadataService = x
                        .GetKeyedService<IMetadataService>(ServiceIds.METADATA_SERVICE_ID);

                    var summarizationService = x
                        .GetKeyedService<ISummarizationService>(ServiceIds.SUMMARIZATION_SERVICE_ID);

                    return new EmbeddingMemoryService(embeddingOptions, embeddingGenerator, memoryVectorStore, metadataService, summarizationService);
                });
        }

        if (options.Embedding.Knowledge?.VectorStore != null)
        {
            services
                .AddSingleton(options.Embedding.Knowledge)
                .AddKnowledgeVectorStore(options.Embedding.Knowledge)
                .AddScoped<IEmbeddingKnowledgeService>(x =>
                {
                    var embeddingOptions = x
                        .GetRequiredService<EmbeddingOptions>();

                    var embeddingGenerator = x
                        .GetRequiredKeyedService<IEmbeddingGenerator<string, Embedding<float>>>(serviceKey: ServiceIds.EMBEDDING_SERVICE_ID);

                    var memoryVectorStore = x
                        .GetRequiredService<KnowledgeVectorStore>();

                    var metadataService = x
                        .GetKeyedService<IMetadataService>(ServiceIds.METADATA_SERVICE_ID);

                    return new EmbeddingKnowledgeService(embeddingOptions, embeddingGenerator, memoryVectorStore, metadataService);
                });
        }

        services
            .AddHealthChecks()
            .AddEmbeddingModelCheck(ServiceIds.EMBEDDING_SERVICE_ID);

        return services;
    }
    internal static IServiceCollection AddMetadataServices<T>(this IServiceCollection services, AiOptions options)
        where T : PromptExecutionSettings, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton(options.Metadata);

        services
            .AddKeyedSingleton(ServiceIds.METADATA_SERVICE_ID, (_, _) =>
            {
                var builder = Kernel.CreateBuilder();

                return builder;
            });

        services
            .AddPromptExecutionSettings<T>(options.Metadata.Model.Parameters, ServiceIds.METADATA_SERVICE_ID)
            .AddScoped<IMetadataService>(x =>
            {
                var metadataOptions = x
                    .GetRequiredService<MetadataOptions>();

                var chatCompletionService = x
                    .GetRequiredKeyedService<IChatCompletionService>(ServiceIds.METADATA_SERVICE_ID);

                var kernelBuilder = x
                    .GetRequiredKeyedService<IKernelBuilder>(ServiceIds.METADATA_SERVICE_ID);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(ServiceIds.METADATA_SERVICE_ID);

                return new MetadataService(metadataOptions, chatCompletionService, kernelBuilder, promptExecutionSettings);
            });

        services
            .AddHealthCheckPromptExecutionSettings<T>(ServiceIds.HEALTH_METADATA_SERVICE_ID)
            .AddHealthChecks()
            .AddChatModelCheck(ServiceIds.METADATA_SERVICE_ID, ServiceIds.HEALTH_METADATA_SERVICE_ID);

        return services;
    }
    internal static IServiceCollection AddSummarizationServices<T>(this IServiceCollection services, AiOptions options)
        where T : PromptExecutionSettings, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        services
            .AddSingleton(options.Summarization);

        services
            .AddKeyedSingleton(ServiceIds.SUMMARIZATION_SERVICE_ID, (_, _) =>
            {
                var builder = Kernel.CreateBuilder();

                return builder;
            });

        services
            .AddPromptExecutionSettings<T>(options.Summarization.Model.Parameters, ServiceIds.SUMMARIZATION_SERVICE_ID)
            .AddScoped<ISummarizationService>(x =>
            {
                var summarizationOptions = x
                    .GetRequiredService<SummarizationOptions>();

                var chatCompletionService = x
                    .GetRequiredKeyedService<IChatCompletionService>(ServiceIds.SUMMARIZATION_SERVICE_ID);

                var kernelBuilder = x
                    .GetRequiredKeyedService<IKernelBuilder>(ServiceIds.SUMMARIZATION_SERVICE_ID);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(ServiceIds.SUMMARIZATION_SERVICE_ID);

                return new SummarizationService(summarizationOptions, chatCompletionService, kernelBuilder, promptExecutionSettings);
            });

        services
            .AddHealthCheckPromptExecutionSettings<T>(ServiceIds.HEALTH_SUMMARIZATION_SERVICE_ID)
            .AddHealthChecks()
            .AddChatModelCheck(ServiceIds.SUMMARIZATION_SERVICE_ID, ServiceIds.HEALTH_SUMMARIZATION_SERVICE_ID);

        return services;
    }

    internal static IServiceCollection AddHttpClient(this IServiceCollection services, string name, string baseAddress, TimeSpan timeout, out HttpClient httpClient)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (baseAddress == null) 
            throw new ArgumentNullException(nameof(baseAddress));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        services
            .AddHttpClient(name, x =>
            {
                x.BaseAddress = new Uri(baseAddress);
                x.Timeout = timeout;
            });

        httpClient = services
            .BuildServiceProvider()
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient(name);

        return services;
    }
    internal static IServiceCollection AddPromptExecutionSettings<T>(this IServiceCollection services, ChatModelParameters chatModelParameters, string serviceId)
        where T : PromptExecutionSettings, new()
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));
        
        if (chatModelParameters == null) 
            throw new ArgumentNullException(nameof(chatModelParameters));
        
        if (serviceId == null) 
            throw new ArgumentNullException(nameof(serviceId));
        
        services
            .AddKeyedTransient(serviceId, (_, _) =>
            {
                var promptExecutionSettings = chatModelParameters
                    .GetPromptExecutionSettings<T>();

                return promptExecutionSettings;
            });

        return services;
    }
    internal static IServiceCollection AddHealthCheckPromptExecutionSettings<T>(this IServiceCollection services, string serviceId)
        where T : PromptExecutionSettings, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        services
            .AddKeyedSingleton(serviceId, (_, _) =>
            {
                var promptExecutionSettings = ChatModelParameters.GetHealthPromptExecutionSettings<T>();

                promptExecutionSettings
                    .Freeze();

                return promptExecutionSettings;
            });

        return services;
    }


    private static IServiceCollection AddMemoryVectorStore(this IServiceCollection services, EmbeddingOptions.MemoryOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddVectorStore<Memory>(options.VectorStore);

        services
            .AddScoped(x =>
            {
                const string SERVICE_ID = nameof(Memory);

                var vectorStore = x
                    .GetRequiredKeyedService<VectorStore>(SERVICE_ID);

                var vectorStoreCollectionDefinition = x
                    .GetRequiredService<VectorStoreCollectionDefinition>();

                return new MemoryVectorStore(vectorStore, vectorStoreCollectionDefinition);
            });

        services
            .AddVectorStoreHealthCheck<Memory>(options.VectorStore);

        services
            .EnsureCreated<Memory>();

        return services;
    }
    private static IServiceCollection AddKnowledgeVectorStore(this IServiceCollection services, EmbeddingOptions.KnowledgeOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddVectorStore<Knowledge>(options.VectorStore);

        services
            .AddScoped(x =>
            {
                const string SERVICE_ID = nameof(Knowledge);

                var vectorStore = x
                    .GetRequiredKeyedService<VectorStore>(SERVICE_ID);

                var vectorStoreCollectionDefinition = x
                    .GetRequiredService<VectorStoreCollectionDefinition>();

                return new KnowledgeVectorStore(vectorStore, vectorStoreCollectionDefinition);
            });

        services
            .AddVectorStoreHealthCheck<Knowledge>(options.VectorStore);

        services
            .EnsureCreated<Knowledge>();

        return services;
    }
    private static IServiceCollection AddVectorStore<T>(this IServiceCollection services, VectorStoreOptions options = null)
        where T : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        switch (options.Provider)
        {
            case VectorProvider.None:
                return services;

            case VectorProvider.Qdrant:
                services
                    .AddQdrantVectorStoreOptions<T>()
                    .AddQdrantVectorStore<T>(options);

                break;

            case VectorProvider.AzureAiSearch:
                services
                    .AddAzureAiSearchVectorStoreOptions<T>()
                    .AddAzureAiSearchVectorStore<T>(options);

                break;

            case VectorProvider.Weaviate:
                services
                    .AddWeaviateVectorStoreOptions<T>()
                    .AddWeaviateVectorStore<T>(options);
                break;

            case VectorProvider.Postgres:
                services
                    .AddPostgresVectorStoreOptions<T>()
                    .AddPostgresVectorStore<T>(options);
                break;

            case VectorProvider.Pinecone:
                services
                    .AddPineconeVectorStoreOptions<T>()
                    .AddPineconeVectorStore<T>(options);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(options.Provider));
        }

        return services;
    }
    private static IServiceCollection AddVectorStoreHealthCheck<TCollection>(this IServiceCollection services, VectorStoreOptions options)
        where TCollection : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        if (!options.UseHealthCheck)
        {
            return services;
        }

        services
            .AddHealthChecks()
            .AddVectorStoreCheck<TCollection>();

        return services;
    }
    private static void EnsureCreated<TCollection>(this IServiceCollection services)
        where TCollection : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var serviceId = typeof(TCollection).Name;
        var collectionName = typeof(TCollection).Name;

        var serviceProvider = services
            .BuildServiceProvider();

        var vectorStoreCollection = serviceProvider
            .GetRequiredKeyedService<VectorStore>(serviceId);

        var vectorStoreCollectionDefinition = serviceProvider
            .GetRequiredService<VectorStoreCollectionDefinition>();

        var collection = vectorStoreCollection
            .GetCollection<Guid, TCollection>(collectionName, vectorStoreCollectionDefinition);

        collection
            .EnsureCollectionExistsAsync()
            .Wait();
    }
}