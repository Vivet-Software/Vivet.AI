using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAIInference;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Extensions.Helpers;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Extensions.Orchestration.AzureInferenceAi;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Azure AI Inference services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure AI Inference services to the specified <see cref="IServiceCollection"/> using default configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetAzureAiInference(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigOptions(out var options)
            .AddAzureAiInferenceServices(options);

        return services;
    }

    /// <summary>
    /// Adds Azure AI Inference services to the specified <see cref="IServiceCollection"/> using custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="AiOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetAzureAiInference(this IServiceCollection services, Action<AiOptions> configureOptions)
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
            .AddAzureAiInferenceServices(options);

        return services;
    }


    private static IServiceCollection AddAzureAiInferenceServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options
            .Validate();

        services
            .AddAzureAiInferenceChatServices(options)
            .AddAzureAiInferenceEmbeddingServices(options)
            .AddAzureAiInferenceMetadataServices(options)
            .AddAzureAiInferenceSummarizationServices(options)
            .AddAzureAiInferenceAgentsServices(options);

        services
            .AddNullTranscriptionServices(options)
            .AddNullVisionServices(options);

        return services;
    }
    private static IServiceCollection AddAzureAiInferenceChatServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureAIInferenceChatClient(options.Chat.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.CHAT_SERVICE_ID)
            .AddAzureAIInferenceChatCompletion(options.Chat.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.CHAT_SERVICE_ID);

        services
            .AddChatServices<AzureAIInferencePromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddAzureAiInferenceEmbeddingServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureAIInferenceEmbeddingGenerator(options.Embedding.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.EMBEDDING_SERVICE_ID);

        services
            .AddEmbeddingServices(options);

        return services;
    }
    private static IServiceCollection AddAzureAiInferenceMetadataServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureAIInferenceChatClient(options.Metadata.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.METADATA_SERVICE_ID)
            .AddAzureAIInferenceChatCompletion(options.Metadata.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.METADATA_SERVICE_ID);

        services
            .AddMetadataServices<AzureAIInferencePromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddAzureAiInferenceSummarizationServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureAIInferenceChatClient(options.Summarization.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.SUMMARIZATION_SERVICE_ID)
            .AddAzureAIInferenceChatCompletion(options.Summarization.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.SUMMARIZATION_SERVICE_ID);

        services
            .AddSummarizationServices<AzureAIInferencePromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddAzureAiInferenceAgentsServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureAIInferenceChatClient(options.Agents.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.AGENTS_SERVICE_ID)
            .AddAzureAIInferenceChatCompletion(options.Agents.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient, ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAgentsServices<AzureAIInferencePromptExecutionSettings>(options);

        return services;
    }
}