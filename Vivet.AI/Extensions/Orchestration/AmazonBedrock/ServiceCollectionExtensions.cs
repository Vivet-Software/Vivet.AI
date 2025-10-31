using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Amazon;
using System;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Extensions.Orchestration.AmazonBedrock.Helpers;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Extensions.Orchestration.AmazonBedrock;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Amazon Bedrock AI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Amazon Bedrock AI services to the specified <see cref="IServiceCollection"/> using default configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetAmazonBedrock(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigOptions(out var options)
            .AddAmazonBedrockAiServices(options);

        return services;
    }

    /// <summary>
    /// Adds Amazon Bedrock AI services to the specified <see cref="IServiceCollection"/> using custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="AiOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddVivetAmazonBedrock(this IServiceCollection services, Action<AiOptions> configureOptions)
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
            .AddAmazonBedrockAiServices(options);

        return services;
    }


    private static IServiceCollection AddAmazonBedrockAiServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options
            .Validate();

        services
            .AddAmazonBedrockAiChatServices(options)
            .AddAmazonBedrockAiEmbeddingServices(options)
            .AddAmazonBedrockAiMetadataServices(options)
            .AddAmazonBedrockAiSummarizationServices(options)
            .AddAmazonBedrockAiAgentsServices(options);

        services
            .AddNullTranscriptionServices(options)
            .AddNullVisionServices(options);

        return services;
    }
    private static IServiceCollection AddAmazonBedrockAiChatServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Chat == null)
        {
            return services;
        }

        var amazonBedrockRuntimeClient = AmazonBedrockRuntimeClientFactory.GetAmazonBedrockRuntimeClient(options.Chat.Model.Name, options.Endpoint, options.ApiKeyId, options.ApiKey, options.Chat.Timeout);

        services
            .AddBedrockChatClient(options.Chat.Model.Name, amazonBedrockRuntimeClient, ServiceIds.CHAT_SERVICE_ID)
            .AddBedrockChatCompletionService(options.Chat.Model.Name, amazonBedrockRuntimeClient, ServiceIds.CHAT_SERVICE_ID);

        services
            .AddAmazonChatServices(options);

        return services;
    }
    private static IServiceCollection AddAmazonBedrockAiEmbeddingServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Embedding == null)
        {
            return services;
        }

        var amazonBedrockRuntimeClient = AmazonBedrockRuntimeClientFactory.GetAmazonBedrockRuntimeClient(options.Embedding.Model.Name, options.Endpoint, options.ApiKeyId, options.ApiKey, options.Embedding.Timeout);

        services
            .AddBedrockEmbeddingGenerator(options.Embedding.Model.Name, amazonBedrockRuntimeClient, ServiceIds.EMBEDDING_SERVICE_ID);

        services
            .AddEmbeddingServices(options);

        return services;
    }
    private static IServiceCollection AddAmazonBedrockAiMetadataServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        if (options.Metadata == null)
        {
            return services;
        }

        var amazonBedrockRuntimeClient = AmazonBedrockRuntimeClientFactory.GetAmazonBedrockRuntimeClient(options.Metadata.Model.Name, options.Endpoint, options.ApiKeyId, options.ApiKey, options.Metadata.Timeout);

        services
            .AddBedrockChatClient(options.Metadata.Model.Name, amazonBedrockRuntimeClient, ServiceIds.METADATA_SERVICE_ID)
            .AddBedrockChatCompletionService(options.Metadata.Model.Name, amazonBedrockRuntimeClient, ServiceIds.METADATA_SERVICE_ID);

        services
            .AddAmazonMetadataServices(options);

        return services;
    }
    private static IServiceCollection AddAmazonBedrockAiSummarizationServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Summarization == null)
        {
            return services;
        }

        var amazonBedrockRuntimeClient = AmazonBedrockRuntimeClientFactory.GetAmazonBedrockRuntimeClient(options.Summarization.Model.Name, options.Endpoint, options.ApiKeyId, options.ApiKey, options.Summarization.Timeout);

        services
            .AddBedrockChatClient(options.Summarization.Model.Name, amazonBedrockRuntimeClient, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID)
            .AddBedrockChatCompletionService(options.Summarization.Model.Name, amazonBedrockRuntimeClient, serviceId: ServiceIds.SUMMARIZATION_SERVICE_ID);

        services
            .AddAmazonSummarizationServices(options);

        return services;
    }
    private static IServiceCollection AddAmazonBedrockAiAgentsServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Agents == null)
        {
            return services;
        }

        var amazonBedrockRuntimeClient = AmazonBedrockRuntimeClientFactory.GetAmazonBedrockRuntimeClient(options.Agents.Model.Name, options.Endpoint, options.ApiKeyId, options.ApiKey, options.Agents.Timeout);

        services
            .AddBedrockChatClient(options.Agents.Model.Name, amazonBedrockRuntimeClient, ServiceIds.AGENTS_SERVICE_ID)
            .AddBedrockChatCompletionService(options.Agents.Model.Name, amazonBedrockRuntimeClient, ServiceIds.AGENTS_SERVICE_ID);

        services
            .AddAmazonAgentsServices(options);

        return services;
    }

    private static IServiceCollection AddAmazonChatServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        var modelNameLowercase = options.Metadata.Model.Name.ToLowerInvariant();

        if (modelNameLowercase.Contains("claude"))
        {
            services
                .AddChatServices<AmazonClaudeExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command"))
        {
            services
                .AddChatServices<AmazonCommandExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command-r"))
        {
            services
                .AddChatServices<AmazonCommandRExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jamba"))
        {
            services
                .AddChatServices<AmazonJambaExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("mistral"))
        {
            services
                .AddChatServices<AmazonMistralExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("titan"))
        {
            services
                .AddChatServices<AmazonTitanExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jurassic"))
        {
            services
                .AddChatServices<AmazonJurassicExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("llama"))
        {
            services
                .AddChatServices<AmazonLlama3ExecutionSettings>(options);
        }

        throw new NotSupportedException($"Model '{options.Metadata.Model.Name}' is not supported for Amazon Bedrock");
    }
    private static IServiceCollection AddAmazonMetadataServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var modelNameLowercase = options.Chat.Model.Name.ToLowerInvariant();

        if (modelNameLowercase.Contains("claude"))
        {
            services
                .AddMetadataServices<AmazonClaudeExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command"))
        {
            services
                .AddMetadataServices<AmazonCommandExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command-r"))
        {
            services
                .AddMetadataServices<AmazonCommandRExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jamba"))
        {
            services
                .AddMetadataServices<AmazonJambaExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("mistral"))
        {
            services
                .AddMetadataServices<AmazonMistralExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("titan"))
        {
            services
                .AddMetadataServices<AmazonTitanExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jurassic"))
        {
            services
                .AddMetadataServices<AmazonJurassicExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("llama"))
        {
            services
                .AddMetadataServices<AmazonLlama3ExecutionSettings>(options);
        }

        throw new NotSupportedException($"Model '{options.Chat.Model.Name}' is not supported for Amazon Bedrock");
    }
    private static IServiceCollection AddAmazonSummarizationServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var modelNameLowercase = options.Summarization.Model.Name.ToLowerInvariant();

        if (modelNameLowercase.Contains("claude"))
        {
            services
                .AddSummarizationServices<AmazonClaudeExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command"))
        {
            services
                .AddSummarizationServices<AmazonCommandExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command-r"))
        {
            services
                .AddSummarizationServices<AmazonCommandRExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jamba"))
        {
            services
                .AddSummarizationServices<AmazonJambaExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("mistral"))
        {
            services
                .AddSummarizationServices<AmazonMistralExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("titan"))
        {
            services
                .AddSummarizationServices<AmazonTitanExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jurassic"))
        {
            services
                .AddSummarizationServices<AmazonJurassicExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("llama"))
        {
            services
                .AddSummarizationServices<AmazonLlama3ExecutionSettings>(options);
        }

        throw new NotSupportedException($"Model '{options.Summarization.Model.Name}' is not supported for Amazon Bedrock");
    }
    private static IServiceCollection AddAmazonAgentsServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var modelNameLowercase = options.Agents.Model.Name.ToLowerInvariant();

        if (modelNameLowercase.Contains("claude"))
        {
            services
                .AddAgentsServices<AmazonClaudeExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command"))
        {
            services
                .AddAgentsServices<AmazonCommandExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("command-r"))
        {
            services
                .AddAgentsServices<AmazonCommandRExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jamba"))
        {
            services
                .AddAgentsServices<AmazonJambaExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("mistral"))
        {
            services
                .AddAgentsServices<AmazonMistralExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("titan"))
        {
            services
                .AddAgentsServices<AmazonTitanExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("jurassic"))
        {
            services
                .AddAgentsServices<AmazonJurassicExecutionSettings>(options);
        }
        else if (modelNameLowercase.Contains("llama"))
        {
            services
                .AddAgentsServices<AmazonLlama3ExecutionSettings>(options);
        }

        throw new NotSupportedException($"Model '{options.Summarization.Model.Name}' is not supported for Amazon Bedrock");
    }
}