using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Vivet.AI.Hosting.HealthChecks;

/// <summary>
/// Embedding Model Health Check.
/// </summary>
/// <param name="embeddingGenerator">The <see cref="IEmbeddingGenerator"/>.</param>
public class EmbeddingModelHealthCheck(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : IHealthCheck
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

    /// <inheritdoc cref="IHealthCheck"/>
    public virtual async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        try
        {
            var embeddingGenerationOptions = new EmbeddingGenerationOptions
            {
                Dimensions = 1
            };

            var embeddings = await this.embeddingGenerator
                .GenerateAsync(["ping"], embeddingGenerationOptions, cancellationToken)
                .ConfigureAwait(false);

            if (!embeddings.Any())
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