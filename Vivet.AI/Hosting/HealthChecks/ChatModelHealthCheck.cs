using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Hosting.HealthChecks;

/// <summary>
/// Chat Model Health Check.
/// </summary>
/// <param name="chatCompletionService">The <see cref="IChatCompletionService"/>.</param>
/// <param name="promptExecutionSettings">The <see cref="PromptExecutionSettings"/>.</param>
public class ChatModelHealthCheck(IChatCompletionService chatCompletionService, PromptExecutionSettings promptExecutionSettings) : IHealthCheck
{
    private readonly IChatCompletionService chatCompletionService = chatCompletionService ?? throw new ArgumentNullException(nameof(chatCompletionService));
    private readonly PromptExecutionSettings promptExecutionSettings = promptExecutionSettings ?? throw new ArgumentNullException(nameof(promptExecutionSettings));

    /// <inheritdoc cref="IHealthCheck"/>
    public virtual async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));

        try
        {
            var chatMessageContent = await this.chatCompletionService
                .GetChatMessageContentAsync("ping", this.promptExecutionSettings, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(chatMessageContent.Content))
            {
                return HealthCheckResult.Unhealthy("No content");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }

        return HealthCheckResult.Healthy("Success");
    }
}