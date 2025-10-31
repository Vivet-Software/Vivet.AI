using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Extensions.Orchestration.AzureOpenAi.Helpers;
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
    public static IServiceCollection AddVivetAzureOpenAi(this IServiceCollection services)
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
    public static IServiceCollection AddVivetAzureOpenAi(this IServiceCollection services, Action<AiOptions> configureOptions)
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
            .AddAzureOpenAiSummarizationServices(options)
            .AddAzureOpenAiAgentsServices(options)
            .AddAzureOpenAiTranscriptionServices(options);

        services
            .AddNullVisionServices(options);

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

        var azureOpenAiClient = AzureOpenAiClientFactory.GetAzureOpenAiClient(options.Chat.Model.Name, options.Endpoint, options.ApiKey, options.Chat.Timeout);

        services
            .AddAzureOpenAIChatClient(options.Chat.Model.Name, azureOpenAiClient, ServiceIds.CHAT_SERVICE_ID)
            .AddAzureOpenAIChatCompletion(options.Chat.Model.Name, azureOpenAiClient, ServiceIds.CHAT_SERVICE_ID);

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

        var azureOpenAiClient = AzureOpenAiClientFactory.GetAzureOpenAiClient(options.Embedding.Model.Name, options.Endpoint, options.ApiKey, options.Embedding.Timeout);

        services
            .AddAzureOpenAIEmbeddingGenerator(options.Embedding.Model.Name, azureOpenAiClient, ServiceIds.EMBEDDING_SERVICE_ID);

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

        var azureOpenAiClient = AzureOpenAiClientFactory.GetAzureOpenAiClient(options.Metadata.Model.Name, options.Endpoint, options.ApiKey, options.Metadata.Timeout);

        services
            .AddAzureOpenAIChatClient(options.Metadata.Model.Name, azureOpenAiClient, ServiceIds.METADATA_SERVICE_ID)
            .AddAzureOpenAIChatCompletion(options.Metadata.Model.Name, azureOpenAiClient, ServiceIds.METADATA_SERVICE_ID);

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

        var azureOpenAiClient = AzureOpenAiClientFactory.GetAzureOpenAiClient(options.Summarization.Model.Name, options.Endpoint, options.ApiKey, options.Summarization.Timeout);

        services
            .AddAzureOpenAIChatClient(options.Summarization.Model.Name, azureOpenAiClient, ServiceIds.SUMMARIZATION_SERVICE_ID)
            .AddAzureOpenAIChatCompletion(options.Summarization.Model.Name, azureOpenAiClient, ServiceIds.SUMMARIZATION_SERVICE_ID);

        services
            .AddSummarizationServices<AzureOpenAIPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddAzureOpenAiAgentsServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Agents == null)
        {
            return services;
        }

        var azureOpenAiClient = AzureOpenAiClientFactory.GetAzureOpenAiClient(options.Agents.Model.Name, options.Endpoint, options.ApiKey, options.Agents.Timeout);

        services
            .AddAzureOpenAIChatClient(options.Agents.Model.Name, azureOpenAiClient, ServiceIds.AGENTS_SERVICE_ID)
            .AddAzureOpenAIChatCompletion(options.Agents.Model.Name, azureOpenAiClient, ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAgentsServices<AzureOpenAIPromptExecutionSettings>(options);

        return services;
    }
    private static IServiceCollection AddAzureOpenAiTranscriptionServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Transcription == null)
        {
            return services;
        }

        var azureOpenAiClient = AzureOpenAiClientFactory.GetAzureOpenAiClient(options.Transcription.Model.Name, options.Endpoint, options.ApiKey, options.Transcription.Timeout);

        services
            .AddAzureOpenAIAudioToText(options.Transcription.Model.Name, azureOpenAiClient, ServiceIds.TRANSCRIPTION_SERVICE_ID);

        services
            .AddTranscriptionServices(options);
        
        return services;
    }
}