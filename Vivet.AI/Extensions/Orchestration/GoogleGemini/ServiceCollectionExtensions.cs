using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System;
using System.Net.Http;
using Microsoft.Extensions.AI;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Extensions.Orchestration.GoogleGemini;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Google Gemini AI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Google Gemini AI services to the specified <see cref="IServiceCollection"/> using default configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetGoogleGemini(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigOptions(out var options)
            .AddGoogleGeminiAiServices(options);

        return services;
    }

    /// <summary>
    /// Adds Google Gemini AI services to the specified <see cref="IServiceCollection"/> using custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="AiOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetGoogleGemini(this IServiceCollection services, Action<AiOptions> configureOptions)
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
            .AddGoogleGeminiAiServices(options);

        return services;
    }


    private static IServiceCollection AddGoogleGeminiAiServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options
            .Validate();

        services
            .AddGoogleGeminiAiChatServices(options)
            .AddGoogleGeminiAiEmbeddingServices(options)
            .AddGoogleGeminiAiMetadataServices(options)
            .AddGoogleGeminiAiSummarizationServices(options)
            .AddGoogleGeminiAiAgentsServices(options);

        services
            .AddNullTranscriptionServices(options)
            .AddNullVisionServices(options);

        return services;
    }
    private static IServiceCollection AddGoogleGeminiAiChatServices(this IServiceCollection services, AiOptions options)
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
            .AddGoogleAiGeminiChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey, options.Chat.Timeout, ServiceIds.CHAT_SERVICE_ID);

        services
            .AddChatServices<GeminiPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddGoogleGeminiAiEmbeddingServices(this IServiceCollection services, AiOptions options)
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
            .AddGoogleAiEmbeddingGenerator(options.Embedding.Model.Name, options.Endpoint, options.ApiKey, options.Embedding.VectorSize, options.Embedding.Timeout, ServiceIds.EMBEDDING_SERVICE_ID);

        services
            .AddEmbeddingServices(options);

        return services;
    }
    private static IServiceCollection AddGoogleGeminiAiMetadataServices(this IServiceCollection services, AiOptions options)
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
            .AddGoogleAiGeminiChatCompletion(options.Metadata.Model.Name, options.Endpoint, options.ApiKey, options.Metadata.Timeout, ServiceIds.METADATA_SERVICE_ID);

        services
            .AddMetadataServices<GeminiPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddGoogleGeminiAiSummarizationServices(this IServiceCollection services, AiOptions options)
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
            .AddGoogleAiGeminiChatCompletion(options.Summarization.Model.Name, options.Endpoint, options.ApiKey, options.Summarization.Timeout, ServiceIds.SUMMARIZATION_SERVICE_ID);

        services
            .AddSummarizationServices<GeminiPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddGoogleGeminiAiAgentsServices(this IServiceCollection services, AiOptions options)
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
            .AddGoogleAiGeminiChatCompletion(options.Agents.Model.Name, options.Endpoint, options.ApiKey, options.Agents.Timeout, ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAgentsServices<GeminiPromptExecutionSettings>(options);

        return services;
    }

    private static IServiceCollection AddGoogleAiGeminiChatCompletion(this IServiceCollection services, string modelId, string endpoint, string apiKey, TimeSpan timeout, string serviceId)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (modelId == null)
            throw new ArgumentNullException(nameof(modelId));

        if (endpoint == null)
            throw new ArgumentNullException(nameof(endpoint));

        if (apiKey == null)
            throw new ArgumentNullException(nameof(apiKey));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        services
            .AddHttpClient(serviceId, x =>
            {
                x.BaseAddress = new Uri(endpoint);
                x.Timeout = timeout;
            });

        services
            .AddKeyedSingleton<IChatCompletionService>(serviceId, (x, _) =>
            {
                var httpClientFactory = x
                    .GetService<IHttpClientFactory>();

                var httpClient = httpClientFactory
                    .CreateClient(serviceId);

                var loggerFactory = x
                    .GetService<ILoggerFactory>();

                return new GoogleAIGeminiChatCompletionService(modelId, apiKey, GoogleAIVersion.V1, httpClient, loggerFactory);
            });

        return services;
    }
    private static IServiceCollection AddGoogleAiEmbeddingGenerator(this IServiceCollection services, string modelId, string endpoint, string apiKey, int dimensions, TimeSpan timeout, string serviceId)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (modelId == null)
            throw new ArgumentNullException(nameof(modelId));

        if (endpoint == null)
            throw new ArgumentNullException(nameof(endpoint));

        if (apiKey == null)
            throw new ArgumentNullException(nameof(apiKey));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        services
            .AddHttpClient(serviceId, x =>
            {
                x.BaseAddress = new Uri(endpoint);
                x.Timeout = timeout;
            });

        services
            .AddKeyedSingleton<IEmbeddingGenerator<string, Embedding<float>>>(serviceId, (x, _) =>
            {
                var httpClientFactory = x
                    .GetService<IHttpClientFactory>();

                var httpClient = httpClientFactory
                    .CreateClient(serviceId);

                var loggerFactory = x
                    .GetService<ILoggerFactory>();

                return new GoogleAIEmbeddingGenerator(modelId, apiKey, GoogleAIVersion.V1, httpClient, loggerFactory, dimensions);
            });

        return services;
    }
}