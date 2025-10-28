using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaSharp;
using System;
using System.Net.Http;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
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
            .AddNullImageExtractionServices(options);

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

        services
            .AddOllamaApiClient(nameof(options.Chat), options.Endpoint, options.Chat.Model.Name, options.Chat.Timeout, out var ollamaApiClient)
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

        services
            .AddOllamaApiClient(nameof(options.Embedding), options.Endpoint, options.Embedding.Model.Name, options.Embedding.Timeout, out var ollamaApiClient)
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

        services
            .AddOllamaApiClient(nameof(options.Metadata), options.Endpoint, options.Metadata.Model.Name, options.Metadata.Timeout, out var ollamaApiClient)
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

        services
            .AddOllamaApiClient(nameof(options.Summarization), options.Endpoint, options.Summarization.Model.Name, options.Summarization.Timeout, out var ollamaApiClient)
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

        services
            .AddOllamaApiClient(nameof(options.Agents), options.Endpoint, options.Agents.Model.Name, options.Agents.Timeout, out var ollamaApiClient)
            .AddOllamaChatClient(ollamaApiClient, serviceId: ServiceIds.AGENTS_SERVICE_ID)
            .AddOllamaChatCompletion(ollamaApiClient, serviceId: ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAgentsServices<OllamaPromptExecutionSettings>(options);

        return services;
    }

    private static IServiceCollection AddOllamaApiClient(this IServiceCollection services, string name, string baseAddress, string modelName, TimeSpan timeout, out OllamaApiClient ollamaApiClient)
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

        services
            .AddScoped(x =>
            {
                var httpClient = x
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(name);

                return new OllamaApiClient(httpClient, modelName);
            });

        ollamaApiClient = services
            .BuildServiceProvider()
            .GetRequiredService<OllamaApiClient>();

        return services;
    }
}