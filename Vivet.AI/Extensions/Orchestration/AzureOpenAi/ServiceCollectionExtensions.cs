using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Extensions.Orchestration.AzureOpenAi;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Azure OpenAi AI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure OpenAi AI services to the specified <see cref="IServiceCollection"/> using default configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAzureOpenAi(this IServiceCollection services)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigOptions(out var options)
            .AddAzureOpenAiServices(options);

        return services;
    }

    /// <summary>
    /// Adds Azure OpenAi AI services to the specified <see cref="IServiceCollection"/> using custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="AiOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAzureOpenAi(this IServiceCollection services, Action<AiOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var options = new AiOptions();

        configureOptions
            .Invoke(options);

        services
            .AddOptions(options)
            .AddAzureOpenAiServices(options);

        return services;
    }


    private static IServiceCollection AddAzureOpenAiServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options
            .Validate();

        services
            .AddAzureOpenAiChatServices(options)
            .AddAzureOpenAiEmbeddingServices(options)
            .AddAzureOpenAiMetadataServices(options)
            .AddAzureOpenAiSummarizationServices(options);

        // TODO: Kernel, Check this
        services
            .AddTransient(x =>
            {
                var builder = Kernel.CreateBuilder();

                if (options.Chat != null)
                {
                    builder
                        .AddAzureOpenAIChatClient(options.Chat.Model.Name, options.Endpoint, options.ApiKey)
                        .AddAzureOpenAIChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey);
                }

                if (options.Embedding != null)
                {
                    builder
                        .AddAzureOpenAIEmbeddingGenerator(options.Embedding.Model.Name, options.Endpoint, options.ApiKey)
                        .AddVectorStoreSearches(x);
                }

                return builder
                    .Build();
            });

        return services;
    }
    private static IServiceCollection AddAzureOpenAiChatServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureOpenAIChatClient(options.Chat.Model.Name, options.Endpoint, options.ApiKey, httpClient: httpClient, serviceId: ServiceIds.CHAT_SERVICE_ID)
            .AddAzureOpenAIChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey, httpClient: httpClient, serviceId: ServiceIds.CHAT_SERVICE_ID);

        services
            .AddChatServices<AzureOpenAIPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddAzureOpenAiEmbeddingServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureOpenAIEmbeddingGenerator(options.Embedding.Model.Name, options.Endpoint, options.ApiKey, httpClient: httpClient, serviceId: ServiceIds.EMBEDDING_SERVICE_ID);

        services
            .AddEmbeddingServices(options);

        return services;
    }
    private static IServiceCollection AddAzureOpenAiMetadataServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureOpenAIChatClient(options.Metadata.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.METADATA_SERVICE_ID, httpClient: httpClient)
            .AddAzureOpenAIChatCompletion(options.Metadata.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.METADATA_SERVICE_ID, httpClient: httpClient);

        services
            .AddMetadataServices<AzureOpenAIPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddAzureOpenAiSummarizationServices(this IServiceCollection services, AiOptions options)
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
            .AddAzureOpenAIChatClient(options.Summarization.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID, httpClient: httpClient)
            .AddAzureOpenAIChatCompletion(options.Summarization.Model.Name, options.Endpoint, options.ApiKey, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID, httpClient: httpClient);

        services
            .AddSummarizationServices<AzureOpenAIPromptExecutionSettings>(options);

        return services;
    }
}