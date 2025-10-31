using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.HuggingFace;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Extensions.Helpers;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Extensions.Orchestration.HuggingFace;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Hugging Face AI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Hugging Face AI services to the specified <see cref="IServiceCollection"/> using default configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetHuggingFace(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigOptions(out var options)
            .AddHuggingFaceServices(options);

        return services;
    }

    /// <summary>
    /// Adds Hugging Face AI services to the specified <see cref="IServiceCollection"/> using custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="AiOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetHuggingFace(this IServiceCollection services, Action<AiOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        var options = new AiOptions();

        configureOptions
            .Invoke(options);

        services
            .AddOptions(options)
            .AddHuggingFaceServices(options);

        return services;
    }


    private static IServiceCollection AddHuggingFaceServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options
            .Validate();

        services
            .AddHuggingFaceChatServices(options)
            .AddHuggingFaceEmbeddingServices(options)
            .AddHuggingFaceMetadataServices(options)
            .AddHuggingFaceSummarizationServices(options)
            .AddHuggingFaceAgentsServices(options)
            .AddHuggingFaceVisionServices(options);

        services
            .AddNullTranscriptionServices(options);
        
        return services;
    }
    private static IServiceCollection AddHuggingFaceChatServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Chat == null)
        {
            return services;
        }

        var httpClient = HttpClientFactory.GetHttpClient(options.Endpoint, options.Chat.Timeout);

        services
            .AddHuggingFaceChatCompletion(options.Chat.Model.Name, new Uri(options.Endpoint), options.ApiKey, ServiceIds.CHAT_SERVICE_ID, httpClient);
        
        services
            .AddChatServices<HuggingFacePromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddHuggingFaceEmbeddingServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Embedding == null)
        {
            return services;
        }

        var httpClient = HttpClientFactory.GetHttpClient(options.Endpoint, options.Embedding.Timeout);

        services
            .AddHuggingFaceEmbeddingGenerator(options.Embedding.Model.Name, new Uri(options.Endpoint), options.ApiKey, httpClient: httpClient, serviceId: ServiceIds.EMBEDDING_SERVICE_ID);

        services
            .AddEmbeddingServices(options);

        return services;
    }
    private static IServiceCollection AddHuggingFaceMetadataServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        if (options.Metadata == null)
        {
            return services;
        }

        var httpClient = HttpClientFactory.GetHttpClient(options.Endpoint, options.Metadata.Timeout);

        services
            .AddHuggingFaceChatCompletion(options.Metadata.Model.Name, new Uri(options.Endpoint), options.ApiKey, ServiceIds.METADATA_SERVICE_ID, httpClient);

        services
            .AddMetadataServices<HuggingFacePromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddHuggingFaceSummarizationServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Summarization == null)
        {
            return services;
        }

        var httpClient = HttpClientFactory.GetHttpClient(options.Endpoint, options.Summarization.Timeout);

        services
            .AddHuggingFaceChatCompletion(options.Summarization.Model.Name, new Uri(options.Endpoint), options.ApiKey, ServiceIds.SUMMARIZATION_SERVICE_ID, httpClient);

        services
            .AddSummarizationServices<HuggingFacePromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddHuggingFaceAgentsServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Agents == null)
        {
            return services;
        }

        var httpClient = HttpClientFactory.GetHttpClient(options.Endpoint, options.Agents.Timeout);

        services
            .AddHuggingFaceChatCompletion(options.Agents.Model.Name, new Uri(options.Endpoint), options.ApiKey, ServiceIds.AGENTS_SERVICE_ID, httpClient);

        services
            .AddAgentsServices<HuggingFacePromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddHuggingFaceVisionServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Vision == null)
        {
            return services;
        }

        var httpClient = HttpClientFactory.GetHttpClient(options.Endpoint, options.Vision.Timeout);

        services
            .AddHuggingFaceImageToText(options.Vision.Model.Name, new Uri(options.Endpoint), options.ApiKey, ServiceIds.VISION_SERVICE_ID, httpClient);

        services
            .AddVisionServices(options);

        return services;
    }
}