using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
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
            .AddOpenAiSummarizationServices(options);

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

        services
            .AddHttpClient(nameof(options.Chat), options.Endpoint, options.Chat.Timeout, out var httpClient)
            .AddOpenAIChatClient(options.Chat.Model.Name, options.Endpoint, options.ApiKey, httpClient: httpClient, serviceId: ServiceIds.CHAT_SERVICE_ID)
            .AddOpenAIChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.CHAT_SERVICE_ID);

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

        services
            .AddHttpClient(nameof(options.Embedding), options.Endpoint, options.Embedding.Timeout, out var httpClient)
            .AddOpenAIEmbeddingGenerator(options.Embedding.Model.Name, options.Endpoint, options.ApiKey, httpClient: httpClient, serviceId: ServiceIds.EMBEDDING_SERVICE_ID);

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

        services
            .AddHttpClient(nameof(options.Metadata), options.Endpoint, options.Metadata.Timeout, out var httpClient)
            .AddOpenAIChatClient(options.Metadata.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.METADATA_SERVICE_ID, httpClient: httpClient)
            .AddOpenAIChatCompletion(options.Metadata.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.METADATA_SERVICE_ID);

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

        services
            .AddHttpClient(nameof(options.Summarization), options.Endpoint, options.Summarization.Timeout, out var httpClient)
            .AddOpenAIChatClient(options.Summarization.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID, httpClient: httpClient)
            .AddOpenAIChatCompletion(options.Summarization.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID);

        services
            .AddSummarizationServices<OpenAIPromptExecutionSettings>(options);

        return services;
    }
}