using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using System;
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
            .AddGoogleAIGeminiChatCompletion(options.Chat.Model.Name, options.ApiKey, serviceId: ServiceIds.CHAT_SERVICE_ID);

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
            .AddGoogleAIEmbeddingGenerator(options.Embedding.Model.Name, options.ApiKey, dimensions: options.Embedding.VectorSize, serviceId: ServiceIds.EMBEDDING_SERVICE_ID);

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
            .AddGoogleAIGeminiChatCompletion(options.Metadata.Model.Name, options.ApiKey, serviceId: ServiceIds.METADATA_SERVICE_ID);

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
            .AddGoogleAIGeminiChatCompletion(options.Summarization.Model.Name, options.ApiKey, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID);

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
            .AddGoogleAIGeminiChatCompletion(options.Agents.Model.Name, options.ApiKey, serviceId: ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAgentsServices<GeminiPromptExecutionSettings>(options);

        return services;
    }
}