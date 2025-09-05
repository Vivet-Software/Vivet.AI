using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.VectorData;

namespace Vivet.AI.Hosting.HealthChecks;

/// <summary>
/// Vector Store Health Check.
/// </summary>
/// <typeparam name="TCollection">the collection type.</typeparam>
/// <param name="collection">The <see cref="VectorStoreCollection{Guid, TCollection}"/>.</param>
public class VectorStoreHealthCheck<TCollection>(VectorStoreCollection<Guid, TCollection> collection) : IHealthCheck
    where TCollection : class
{
    private readonly VectorStoreCollection<Guid, TCollection> collection = collection ?? throw new ArgumentNullException(nameof(collection));

    /// <inheritdoc cref="IHealthCheck"/>
    public virtual async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));
        
        try
        {
            var exists = await this.collection
                .CollectionExistsAsync(cancellationToken)
                .ConfigureAwait(false);

            if (exists)
            {
                return HealthCheckResult.Healthy();
            }

            return HealthCheckResult.Unhealthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}