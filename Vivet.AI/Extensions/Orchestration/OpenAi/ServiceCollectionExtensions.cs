using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Extensions.Orchestration.OpenAi.Helpers;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Extensions.Orchestration.OpenAi;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register OpenAi AI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenAi AI services to the specified <see cref="IServiceCollection"/> using default configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetOpenAi(this IServiceCollection services)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigOptions(out var options)
            .AddOpenAiServices(options);

        return services;
    }

    /// <summary>
    /// Adds OpenAi AI services to the specified <see cref="IServiceCollection"/> using custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="AiOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetOpenAi(this IServiceCollection services, Action<AiOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var options = new AiOptions();

        configureOptions
            .Invoke(options);

        services
            .AddOptions(options)
            .AddOpenAiServices(options);

        return services;
    }


    private static IServiceCollection AddOpenAiServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options
            .Validate();

        services
            .AddOpenAiChatServices(options)
            .AddOpenAiEmbeddingServices(options)
            .AddOpenAiMetadataServices(options)
            .AddOpenAiSummarizationServices(options)
            .AddOpenAiAgentServices(options);

        services
            .AddNullTranscriptionServices(options)
            .AddNullVisionServices(options);

        return services;
    }
    private static IServiceCollection AddOpenAiChatServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        
        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        if (options.Chat == null)
        {
            return services;
        }

        var openAiClient = OpenAiClientFactory.GetOpenAiClient(options.Chat.Model.Name, options.Endpoint, options.ApiKey, options.Chat.Timeout);

        services
            .AddOpenAIChatClient(options.Chat.Model.Name, openAiClient, ServiceIds.CHAT_SERVICE_ID)
            .AddOpenAIChatCompletion(options.Chat.Model.Name, openAiClient, ServiceIds.CHAT_SERVICE_ID);

        services
            .AddChatServices<OpenAIPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddOpenAiEmbeddingServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Embedding == null)
        {
            return services;
        }

        var openAiClient = OpenAiClientFactory.GetOpenAiClient(options.Embedding.Model.Name, options.Endpoint, options.ApiKey, options.Embedding.Timeout);

        services
            .AddOpenAIEmbeddingGenerator(options.Embedding.Model.Name, openAiClient, serviceId: ServiceIds.EMBEDDING_SERVICE_ID);

        services
            .AddEmbeddingServices(options);

        return services;
    }
    private static IServiceCollection AddOpenAiMetadataServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Metadata == null)
        {
            return services;
        }

        var openAiClient = OpenAiClientFactory.GetOpenAiClient(options.Metadata.Model.Name, options.Endpoint, options.ApiKey, options.Metadata.Timeout);

        services
            .AddOpenAIChatClient(options.Metadata.Model.Name, openAiClient, ServiceIds.METADATA_SERVICE_ID)
            .AddOpenAIChatCompletion(options.Metadata.Model.Name, openAiClient, ServiceIds.METADATA_SERVICE_ID);

        services
            .AddMetadataServices<OpenAIPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddOpenAiSummarizationServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Summarization == null)
        {
            return services;
        }

        var openAiClient = OpenAiClientFactory.GetOpenAiClient(options.Summarization.Model.Name, options.Endpoint, options.ApiKey, options.Summarization.Timeout);

        services
            .AddOpenAIChatClient(options.Summarization.Model.Name, openAiClient, ServiceIds.SUMMARIZATION_SERVICE_ID)
            .AddOpenAIChatCompletion(options.Summarization.Model.Name, openAiClient, ServiceIds.SUMMARIZATION_SERVICE_ID);

        services
            .AddSummarizationServices<OpenAIPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddOpenAiAgentServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Agents == null)
        {
            return services;
        }

        var openAiClient = OpenAiClientFactory.GetOpenAiClient(options.Agents.Model.Name, options.Endpoint, options.ApiKey, options.Agents.Timeout);

        services
            .AddOpenAIChatClient(options.Agents.Model.Name, openAiClient, ServiceIds.AGENTS_SERVICE_ID)
            .AddOpenAIChatCompletion(options.Agents.Model.Name, openAiClient, ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAgentsServices<OpenAIPromptExecutionSettings>(options);

        return services;
    }
}