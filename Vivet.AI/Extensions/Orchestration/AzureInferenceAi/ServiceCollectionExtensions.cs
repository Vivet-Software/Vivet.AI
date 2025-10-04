using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAIInference;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
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

        services
            .AddHttpClient(nameof(options.Chat), options.Endpoint, options.Chat.Timeout, out var httpClient)
            .AddAzureAIInferenceChatClient(options.Chat.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.CHAT_SERVICE_ID, httpClient: httpClient)
            .AddAzureAIInferenceChatCompletion(options.Chat.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.CHAT_SERVICE_ID, httpClient: httpClient);

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

        services
            .AddHttpClient(nameof(options.Embedding), options.Endpoint, options.Embedding.Timeout, out var httpClient)
            .AddAzureAIInferenceEmbeddingGenerator(options.Embedding.Model.Name, options.ApiKey, new Uri(options.Endpoint), httpClient: httpClient, serviceId: ServiceIds.EMBEDDING_SERVICE_ID);

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

        services
            .AddHttpClient(nameof(options.Metadata), options.Endpoint, options.Metadata.Timeout, out var httpClient)
            .AddAzureAIInferenceChatClient(options.Metadata.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.METADATA_SERVICE_ID, httpClient: httpClient)
            .AddAzureAIInferenceChatCompletion(options.Metadata.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.METADATA_SERVICE_ID, httpClient: httpClient);

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

        services
            .AddHttpClient(nameof(options.Summarization), options.Endpoint, options.Summarization.Timeout, out var httpClient)
            .AddAzureAIInferenceChatClient(options.Summarization.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID, httpClient: httpClient)
            .AddAzureAIInferenceChatCompletion(options.Summarization.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID, httpClient: httpClient);

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

        services
            .AddHttpClient(nameof(options.Agents), options.Endpoint, options.Agents.Timeout, out var httpClient)
            .AddAzureAIInferenceChatClient(options.Agents.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.AGENT_SERVICE_ID, httpClient: httpClient)
            .AddAzureAIInferenceChatCompletion(options.Agents.Model.Name, options.ApiKey, new Uri(options.Endpoint), serviceId: ServiceIds.AGENT_SERVICE_ID, httpClient: httpClient);

        services
            .AddAgentsServices<AzureAIInferencePromptExecutionSettings>(options);

        return services;
    }
}