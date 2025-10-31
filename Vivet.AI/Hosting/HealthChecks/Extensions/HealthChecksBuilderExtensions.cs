using System;
using System.Collections.Generic;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.ImageToText;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Hosting.HealthChecks.Extensions;

internal static class HealthChecksBuilderExtensions
{
    internal static IHealthChecksBuilder AddChatModelCheck(this IHealthChecksBuilder builder, string serviceId, string healthServiceId, HealthStatus? failureStatus = null, IEnumerable<string> tags = null)
    {
        if (builder == null) 
            throw new ArgumentNullException(nameof(builder));

        if (serviceId == null) 
            throw new ArgumentNullException(nameof(serviceId));

        var healthCheckRegistration = new HealthCheckRegistration(serviceId,
            x =>
            {
                var chatCompletionService = x
                    .GetRequiredKeyedService<IChatCompletionService>(serviceId);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(healthServiceId);

                return new ChatModelHealthCheck(chatCompletionService, promptExecutionSettings);
            },
            failureStatus, tags);

        builder
            .Add(healthCheckRegistration);

        return builder;
    }

    internal static IHealthChecksBuilder AddEmbeddingModelCheck(this IHealthChecksBuilder builder, string serviceId, HealthStatus? failureStatus = null, IEnumerable<string> tags = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        var healthCheckRegistration = new HealthCheckRegistration(serviceId,
            x =>
            {
                var embeddingGenerator = x
                    .GetRequiredKeyedService<IEmbeddingGenerator<string, Embedding<float>>>(serviceKey: serviceId);

                return new EmbeddingModelHealthCheck(embeddingGenerator);
            },
            failureStatus, tags);

        builder
            .Add(healthCheckRegistration);

        return builder;
    }

    internal static IHealthChecksBuilder AddTranscriptionModelCheck(this IHealthChecksBuilder builder, string serviceId, string healthServiceId, HealthStatus? failureStatus = null, IEnumerable<string> tags = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        var healthCheckRegistration = new HealthCheckRegistration(serviceId,
            x =>
            {
                var audioToTextService = x
                    .GetRequiredKeyedService<IAudioToTextService>(serviceId);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(healthServiceId);

                return new TranscriptionModelHealthCheck(audioToTextService, promptExecutionSettings);
            },
            failureStatus, tags);

        builder
            .Add(healthCheckRegistration);

        return builder;
    }

    internal static IHealthChecksBuilder AddImageExtractionModelCheck(this IHealthChecksBuilder builder, string serviceId, string healthServiceId, HealthStatus? failureStatus = null, IEnumerable<string> tags = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceId == null)
            throw new ArgumentNullException(nameof(serviceId));

        var healthCheckRegistration = new HealthCheckRegistration(serviceId,
            x =>
            {
                var audioToTextService = x
                    .GetRequiredKeyedService<IImageToTextService>(serviceId);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(healthServiceId);

                return new ImageExtractionModelHealthCheck(audioToTextService, promptExecutionSettings);
            },
            failureStatus, tags);

        builder
            .Add(healthCheckRegistration);

        return builder;
    }

    internal static IHealthChecksBuilder AddVectorStoreCheck<TCollection>(this IHealthChecksBuilder builder, HealthStatus? failureStatus = null, IEnumerable<string> tags = null)
        where TCollection : BaseEmbedding
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var serviceId = typeof(TCollection).Name;
        var name = $"{typeof(TCollection).Name}_vector_store";

        var healthCheckRegistration = new HealthCheckRegistration(name,
            x =>
            {
                var vectorStoreCollection = x
                    .GetRequiredKeyedService<VectorStoreCollection<Guid, TCollection>>(serviceId);

                return new VectorStoreHealthCheck<TCollection>(vectorStoreCollection);
            },
            failureStatus, tags);

        builder
            .Add(healthCheckRegistration);

        return builder;
    }
}