using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.ImageToText;
using System;
using System.Collections.Generic;
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
using Vivet.AI.Hosting.HealthChecks.Extensions;
using Vivet.AI.Models;
using Vivet.AI.Models.Enums;
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
            .AddSingleton(options)
            .AddSingleton(options.Plugins);

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
            .AddSingleton(options.Plugins)
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
                var pluginsOptions = x
                    .GetService<PluginsOptions>();

                var chatCompletionService = x
                    .GetRequiredKeyedService<IChatCompletionService>(ServiceIds.CHAT_SERVICE_ID);

                var builder = Kernel.CreateBuilder();

                builder.Services
                    .AddScoped(_ => chatCompletionService);

                builder
                    .AddLoggerFactory(x)
                    .AddChatBuiltInPlugins(x, pluginsOptions);

                return builder;
            });

        services
            .AddTextSearch(ServiceIds.CHAT_SERVICE_ID, options.Plugins.WebSearch)
            .AddChatModelPromptExecutionSettings<T>(options.Chat.Model.Parameters, ServiceIds.CHAT_SERVICE_ID)
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

                return new ChatService(chatOptions, chatCompletionService, kernelBuilder, x, promptExecutionSettings, embeddingMemoryService);
            });

        if (options.Chat.Model.UseHealthCheck)
        {
            services
                .AddChatModelHealthCheckPromptExecutionSettings<T>(ServiceIds.HEALTH_CHAT_SERVICE_ID)
                .AddHealthChecks()
                .AddChatModelCheck(ServiceIds.CHAT_SERVICE_ID, ServiceIds.HEALTH_CHAT_SERVICE_ID);
        }

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
            .AddSingleton<IEmbeddingGenerator>(x => x
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
                        .GetService<IMetadataService>();

                    var summarizationService = x
                        .GetService<ISummarizationService>();

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
                        .GetService<IMetadataService>();

                    return new EmbeddingKnowledgeService(embeddingOptions, embeddingGenerator, memoryVectorStore, metadataService);
                });
        }

        if (options.Embedding.Model.UseHealthCheck)
        {
            services
                .AddHealthChecks()
                .AddEmbeddingModelCheck(ServiceIds.EMBEDDING_SERVICE_ID);
        }

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
            .AddKeyedSingleton(ServiceIds.METADATA_SERVICE_ID, (x, _) =>
            {
                var builder = Kernel.CreateBuilder();

                builder
                    .AddLoggerFactory(x);

                return builder;
            });

        services
            .AddChatModelPromptExecutionSettings<T>(options.Metadata.Model.Parameters, ServiceIds.METADATA_SERVICE_ID)
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

        if (options.Metadata.Model.UseHealthCheck)
        {
            services
                .AddChatModelHealthCheckPromptExecutionSettings<T>(ServiceIds.HEALTH_METADATA_SERVICE_ID)
                .AddHealthChecks()
                .AddChatModelCheck(ServiceIds.METADATA_SERVICE_ID, ServiceIds.HEALTH_METADATA_SERVICE_ID);
        }

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
            .AddKeyedSingleton(ServiceIds.SUMMARIZATION_SERVICE_ID, (x, _) =>
            {
                var builder = Kernel.CreateBuilder();

                builder
                    .AddLoggerFactory(x);

                return builder;
            });

        services
            .AddChatModelPromptExecutionSettings<T>(options.Summarization.Model.Parameters, ServiceIds.SUMMARIZATION_SERVICE_ID)
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

        if (options.Summarization.Model.UseHealthCheck)
        {
            services
                .AddChatModelHealthCheckPromptExecutionSettings<T>(ServiceIds.HEALTH_SUMMARIZATION_SERVICE_ID)
                .AddHealthChecks()
                .AddChatModelCheck(ServiceIds.SUMMARIZATION_SERVICE_ID, ServiceIds.HEALTH_SUMMARIZATION_SERVICE_ID);
        }

        return services;
    }

    internal static IServiceCollection AddAgentsServices<T>(this IServiceCollection services, AiOptions options)
        where T : PromptExecutionSettings, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton(options.Agents);

        services
            .AddKeyedSingleton(ServiceIds.AGENTS_SERVICE_ID, (x, _) =>
            {
                var pluginsOptions = x
                    .GetService<PluginsOptions>();

                var chatCompletionService = x
                    .GetRequiredKeyedService<IChatCompletionService>(ServiceIds.AGENTS_SERVICE_ID);

                var builder = Kernel.CreateBuilder();

                builder.Services
                    .AddScoped(_ => chatCompletionService);

                builder
                    .AddLoggerFactory(x)
                    .AddAgentsBuiltInPlugins(x, pluginsOptions);

                return builder;
            });

        services
            .AddTextSearch(ServiceIds.AGENTS_SERVICE_ID, options.Plugins.WebSearch)
            .AddChatModelPromptExecutionSettings<T>(options.Agents.Model.Parameters, ServiceIds.AGENTS_SERVICE_ID)
            .AddScoped<IAgentsService>(x =>
            {
                var agentOptions = x
                    .GetRequiredService<AgentsOptions>();

                var kernelBuilder = x
                    .GetRequiredKeyedService<IKernelBuilder>(ServiceIds.AGENTS_SERVICE_ID);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(ServiceIds.AGENTS_SERVICE_ID);

                var embeddingMemoryService = x
                    .GetService<IEmbeddingMemoryService>();

                return new AgentsService(agentOptions, x, kernelBuilder, promptExecutionSettings, embeddingMemoryService);
            });

        if (options.Agents.Model.UseHealthCheck)
        {
            services
                .AddChatModelHealthCheckPromptExecutionSettings<T>(ServiceIds.HEALTH_AGENTS_SERVICE_ID)
                .AddHealthChecks()
                .AddChatModelCheck(ServiceIds.AGENTS_SERVICE_ID, ServiceIds.HEALTH_AGENTS_SERVICE_ID);
        }

        return services;
    }

    internal static IServiceCollection AddTranscriptionServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton(options.Transcription);

        services
            .AddTranscriptionModelPromptExecutionSettings(options.Transcription, ServiceIds.TRANSCRIPTION_SERVICE_ID)
            .AddScoped<ITranscriptionService>(x =>
            {
                var audioToTextService = x
                    .GetRequiredKeyedService<IAudioToTextService>(ServiceIds.TRANSCRIPTION_SERVICE_ID);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(ServiceIds.TRANSCRIPTION_SERVICE_ID);

                return new TranscriptionService(audioToTextService, promptExecutionSettings);
            });

        if (options.Transcription.Model.UseHealthCheck)
        {
            services
                .AddTranscriptionModelHealthCheckPromptExecutionSettings(ServiceIds.HEALTH_TRANSCRIPTION_SERVICE_ID)
                .AddHealthChecks()
                .AddTranscriptionModelCheck(ServiceIds.TRANSCRIPTION_SERVICE_ID, ServiceIds.HEALTH_TRANSCRIPTION_SERVICE_ID);
        }

        return services;
    }

    internal static IServiceCollection AddVisionServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton(options.Vision);

        services
            .AddImageExtractionModelPromptExecutionSettings(ServiceIds.VISION_SERVICE_ID)
            .AddScoped<IVisionService>(x =>
            {
                var imageToTextService = x
                    .GetRequiredKeyedService<IImageToTextService>(ServiceIds.VISION_SERVICE_ID);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(ServiceIds.VISION_SERVICE_ID);

                return new VisionService(imageToTextService, promptExecutionSettings);
            });

        if (options.Vision.Model.UseHealthCheck)
        {
            services
                .AddImageExtractionModelHealthCheckPromptExecutionSettings(ServiceIds.HEALTH_VISION_SERVICE_ID)
                .AddHealthChecks()
                .AddImageExtractionModelCheck(ServiceIds.VISION_SERVICE_ID, ServiceIds.HEALTH_VISION_SERVICE_ID);
        }

        return services;
    }


    private static IServiceCollection AddChatModelPromptExecutionSettings<T>(this IServiceCollection services, ChatModelParameters chatModelParameters, string serviceId)
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
    private static IServiceCollection AddChatModelHealthCheckPromptExecutionSettings<T>(this IServiceCollection services, string serviceId)
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
    private static IServiceCollection AddTranscriptionModelPromptExecutionSettings(this IServiceCollection services, TranscriptionOptions options, string serviceId)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        services
            .AddKeyedTransient(serviceId, (_, _) =>
            {
                var timestampGranularities = new List<string>
                {
                    "segment"
                };

                if (options.IncludeWordGranularity)
                {
                    timestampGranularities
                        .Add("word");
                }

                return new PromptExecutionSettings
                {
                    ExtensionData = new Dictionary<string, object>
                    {
                        ["response_format"] = "verbose_json",
                        ["timestamp_granularities"] = timestampGranularities
                    }
                };
            });

        return services;
    }
    private static IServiceCollection AddTranscriptionModelHealthCheckPromptExecutionSettings(this IServiceCollection services, string serviceId)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        services
            .AddKeyedSingleton(serviceId, (_, _) =>
            {
                var promptExecutionSettings = new PromptExecutionSettings();

                promptExecutionSettings
                    .Freeze();

                return promptExecutionSettings;
            });

        return services;
    }
    private static IServiceCollection AddImageExtractionModelPromptExecutionSettings(this IServiceCollection services, string serviceId)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        services
            .AddKeyedTransient(serviceId, (_, _) =>
            {
                var promptExecutionSettings = new PromptExecutionSettings();

                return promptExecutionSettings;
            });

        return services;
    }
    private static IServiceCollection AddImageExtractionModelHealthCheckPromptExecutionSettings(this IServiceCollection services, string serviceId)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        services
            .AddKeyedSingleton(serviceId, (_, _) =>
            {
                var promptExecutionSettings = new PromptExecutionSettings();

                promptExecutionSettings
                    .Freeze();

                return promptExecutionSettings;
            });

        return services;
    }
    private static IServiceCollection AddTextSearch(this IServiceCollection services, string serviceId, WebSearchPluginOptions webSearchPluginOptions = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        if (webSearchPluginOptions == null)
        {
            return services;
        }

        switch (webSearchPluginOptions.Provider)
        {
            case WebSearchProvider.Google:
                services
                    .AddGoogleTextSearch(webSearchPluginOptions.Id, webSearchPluginOptions.ApiKey, serviceId: serviceId);
                break;

            case WebSearchProvider.Bing:
                services
                    .AddBingTextSearch(webSearchPluginOptions.ApiKey, serviceId: serviceId);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(webSearchPluginOptions.Provider), webSearchPluginOptions.Provider, $"The provider '{webSearchPluginOptions.Provider}' is not suppoprted.");
        }

        return services;
    }
    private static IServiceCollection AddMemoryVectorStore(this IServiceCollection services, EmbeddingMemoryOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return null;
        }

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
    private static IServiceCollection AddKnowledgeVectorStore(this IServiceCollection services, EmbeddingKnowledgeOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return null;
        }

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