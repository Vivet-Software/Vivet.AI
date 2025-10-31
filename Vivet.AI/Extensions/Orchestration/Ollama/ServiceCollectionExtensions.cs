using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Extensions.Orchestration.Ollama.Helpers;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Extensions.Orchestration.Ollama;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Ollama AI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Ollama AI services to the specified <see cref="IServiceCollection"/> using default configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetOllama(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigOptions(out var options)
            .AddOllamaServices(options);

        return services;
    }

    /// <summary>
    /// Adds Ollama AI services to the specified <see cref="IServiceCollection"/> using custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="AiOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetOllama(this IServiceCollection services, Action<AiOptions> configureOptions)
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
            .AddOllamaServices(options);

        return services;
    }


    private static IServiceCollection AddOllamaServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options
            .Validate();

        services
            .AddOllamaChatServices(options)
            .AddOllamaEmbeddingServices(options)
            .AddOllamaMetadataServices(options)
            .AddOllamaSummarizationServices(options)
            .AddOllamaAgentsServices(options);

        services
            .AddNullTranscriptionServices(options)
            .AddNullVisionServices(options);

        return services;
    }
    private static IServiceCollection AddOllamaChatServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Chat == null)
        {
            return services;
        }

        var ollamaApiClient = OllamaApiClientFactory.GetOllamaApiClient(options.Chat.Model.Name, options.Endpoint, options.Chat.Timeout);

        services
            .AddOllamaChatClient(ollamaApiClient, serviceId: ServiceIds.CHAT_SERVICE_ID)
            .AddOllamaChatCompletion(ollamaApiClient, serviceId: ServiceIds.CHAT_SERVICE_ID);

        services
            .AddChatServices<OllamaPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddOllamaEmbeddingServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Embedding == null)
        {
            return services;
        }

        var ollamaApiClient = OllamaApiClientFactory.GetOllamaApiClient(options.Embedding.Model.Name, options.Endpoint, options.Embedding.Timeout);

        services
            .AddOllamaEmbeddingGenerator(ollamaApiClient, serviceId: ServiceIds.EMBEDDING_SERVICE_ID);

        services
            .AddEmbeddingServices(options);

        return services;
    }
    private static IServiceCollection AddOllamaMetadataServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        if (options.Metadata == null)
        {
            return services;
        }

        var ollamaApiClient = OllamaApiClientFactory.GetOllamaApiClient(options.Metadata.Model.Name, options.Endpoint, options.Metadata.Timeout);

        services
            .AddOllamaChatClient(ollamaApiClient, serviceId: ServiceIds.METADATA_SERVICE_ID)
            .AddOllamaChatCompletion(ollamaApiClient, serviceId: ServiceIds.METADATA_SERVICE_ID);

        services
            .AddMetadataServices<OllamaPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddOllamaSummarizationServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Summarization == null)
        {
            return services;
        }

        var ollamaApiClient = OllamaApiClientFactory.GetOllamaApiClient(options.Summarization.Model.Name, options.Endpoint, options.Summarization.Timeout);

        services
            .AddOllamaChatClient(ollamaApiClient, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID)
            .AddOllamaChatCompletion(ollamaApiClient, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID);

        services
            .AddSummarizationServices<OllamaPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddOllamaAgentsServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Agents == null)
        {
            return services;
        }

        var ollamaApiClient = OllamaApiClientFactory.GetOllamaApiClient(options.Agents.Model.Name, options.Endpoint, options.Agents.Timeout);

        services
            .AddOllamaChatClient(ollamaApiClient, serviceId: ServiceIds.AGENTS_SERVICE_ID)
            .AddOllamaChatCompletion(ollamaApiClient, serviceId: ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAgentsServices<OllamaPromptExecutionSettings>(options);

        return services;
    }
}