using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Data;
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

// BUG: Functions / Plugins
// - Plugins: https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/?pivots=programming-language-csharp 
// - Text Search Plugins: https://learn.microsoft.com/en-us/semantic-kernel/concepts/text-search/?pivots=programming-language-csharp
// - Function Filters https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/filters?pivots=programming-language-csharp
// - Planning: https://learn.microsoft.com/en-us/semantic-kernel/concepts/planning?pivots=programming-language-csharp

// BUG: Integration Testing Plugins (config and requests)

// BUG: Make Built-in Plugins: Google, Bing, etc online search (chat, metadata, summarization)
// https://learn.microsoft.com/en-us/semantic-kernel/concepts/text-search/out-of-the-box-textsearch/google-textsearch?pivots=programming-language-csharp

// TODO: Agent Framework: https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/?pivots=programming-language-csharp
// TODO: Process Framework: https://learn.microsoft.com/en-us/semantic-kernel/frameworks/process/process-framework

// TODO: Observability: https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/?pivots=programming-language-csharp

// TODO: Check in Azure AI Foundry which types of models that can be deployed (when deploying a model there is a list to filter model types)
// https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-models/how-to/use-image-embeddings?pivots=programming-language-csharp
// - Check common services (Azure, HuggingFace) and consider whether we should integrate them into the library

// TODO: Handle Blobs better. (after AI Services, e.g. Docuemnt Intelligence)
// SemanticKernel Services: https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/integrations
// - SK: Image to Text (Experimental) ???
// - SK: Audio to Text (Experimental) ???
// - SK: Text to Audio (Experimental) ???
// - Azure.AI.DocumentIntelligence + There was a package also to store files available to the LLM on blob. Check it out.

// TODO: All Azure AI services: 
// - https://portal.azure.com/#view/Microsoft_Azure_ProjectOxford/CognitiveServicesHub/~/overview (Azure Document Intelligence)
//   https://learn.microsoft.com/en-us/azure/search/tutorial-document-extraction-image-verbalization
// - Text Analytics: Azure Cognitive Services Text Analytics is a cloud service that provides advanced natural language processing over raw text,
//   and features like Language Detection, Sentiment Analysis, Key Phrase Extraction, Named Entity Recognition, Personally Identifiable Information (PII) Recognition,
//   Linked Entity Recognition, Text Analytics for Health, and more.


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
            .AddSingleton(x =>
            {
                var chatOptions = x
                    .GetRequiredService<ChatOptions>();

                var builder = Kernel.CreateBuilder();

                builder
                    .AddChatPluginsFromConfiguration(services, chatOptions);

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

                var embeddingKnowledgeService = x
                    .GetService<IEmbeddingKnowledgeService>();

                return new ChatService(chatOptions, chatCompletionService, kernelBuilder, promptExecutionSettings, embeddingMemoryService, embeddingKnowledgeService);
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
            .AddSingleton(x =>
            {
                var metadataOptions = x
                    .GetRequiredService<MetadataOptions>();

                var builder = Kernel.CreateBuilder();

                builder
                    .AddMetadataPluginsFromConfiguration(services, metadataOptions);

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
            .AddSingleton(x =>
            {
                var summarizationOptions = x
                    .GetRequiredService<SummarizationOptions>();

                var builder = Kernel.CreateBuilder();

                builder
                    .AddSummarizationPluginsFromConfiguration(services,summarizationOptions);

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
            .AddKeyedSingleton(serviceId, (_, _) =>
            {
                var promptExecutionSettings = chatModelParameters
                    .GetPromptExecutionSettings<T>();

                promptExecutionSettings
                    .Freeze();

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
            .AddVectorStore<Memory>(options.VectorStore)
            .AddTextSearchServices<Memory>();

        services
            .AddTransient(x =>
            {
                const string SERVICE_ID = nameof(Memory);

                var vectorStore = x
                    .GetRequiredKeyedService<VectorStore>(SERVICE_ID);

                var vectorStoreTextSearch = x
                    .GetRequiredKeyedService<VectorStoreTextSearch<Memory>>(SERVICE_ID);

                var vectorStoreCollectionDefinition = x
                    .GetRequiredService<VectorStoreCollectionDefinition>();

                return new MemoryVectorStore(vectorStore, vectorStoreTextSearch, vectorStoreCollectionDefinition);
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
            .AddVectorStore<Knowledge>(options.VectorStore)
            .AddTextSearchServices<Knowledge>();

        services
            .AddTransient(x =>
            {
                const string SERVICE_ID = nameof(Knowledge);

                var vectorStore = x
                    .GetRequiredKeyedService<VectorStore>(SERVICE_ID);

                var vectorStoreTextSearch = x
                    .GetRequiredKeyedService<VectorStoreTextSearch<Knowledge>>(SERVICE_ID);

                var vectorStoreCollectionDefinition = x
                    .GetRequiredService<VectorStoreCollectionDefinition>();

                return new KnowledgeVectorStore(vectorStore, vectorStoreTextSearch, vectorStoreCollectionDefinition);
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
    private static IServiceCollection AddTextSearchServices<TCollection>(this IServiceCollection services)
        where TCollection : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var serviceId = typeof(TCollection).Name;

        services
            .AddKeyedTransient(serviceId, (x, y) =>
            {
                var vectorStoreCollection = x
                    .GetRequiredKeyedService<VectorStoreCollection<Guid, TCollection>>(y);

                var embeddingGenerator = x
                    .GetRequiredKeyedService<IEmbeddingGenerator<string, Embedding<float>>>(serviceKey: ServiceIds.EMBEDDING_SERVICE_ID);

                return new VectorStoreTextSearch<TCollection>(vectorStoreCollection, embeddingGenerator);
            })
            .AddKeyedTransient<ITextSearch>(serviceId,
                (x, y) => x.GetRequiredKeyedService<VectorStoreTextSearch<TCollection>>(y));

        return services;
    }
    private static void EnsureCreated<TCollection>(this IServiceCollection services)
        where TCollection : BaseEmbedding
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var serviceId = typeof(TCollection).Name;
        var collectionName = typeof(TCollection).Name;

        var vectorStoreCollection = services
            .BuildServiceProvider()
            .GetRequiredKeyedService<VectorStore>(serviceId);

        var vectorStoreCollectionDefinition = services
            .BuildServiceProvider()
            .GetRequiredService<VectorStoreCollectionDefinition>();

        var collection = vectorStoreCollection
            .GetCollection<Guid, TCollection>(collectionName, vectorStoreCollectionDefinition);

        collection
            .EnsureCollectionExistsAsync()
            .Wait();
    }
}