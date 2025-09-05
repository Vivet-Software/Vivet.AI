using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Vivet.AI.Hosting.HealthChecks;

namespace Vivet.AI.Extensions;

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
                var chatService = x
                    .GetRequiredKeyedService<IChatCompletionService>(serviceId);

                var promptExecutionSettings = x
                    .GetRequiredKeyedService<PromptExecutionSettings>(healthServiceId);

                return new ChatModelHealthCheck(chatService, promptExecutionSettings);
            },
            failureStatus, tags);

        builder
            .Add(healthCheckRegistration);

        return builder;
    }
}