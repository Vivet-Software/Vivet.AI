using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.VectorData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Data.Models;
using Vivet.AI.Hosting.HealthChecks;

namespace IntegrationTests.Vivet.AI.Hosting.HealthChecks;

[TestClass]
public class VectorStoreHealthCheckTests : BaseTests
{
    private const string FAKE_VECTOR_STORE = "FAKE_VECTOR_STORE";

    private HealthCheckService HealthCheckService => this.ServiceProvider.GetRequiredService<HealthCheckService>();

    [TestMethod]
    public async Task CheckHealthWhenMemoryHealthyTest()
    {
        var entryName = $"{nameof(Memory).ToLower()}_vector_store";

        var healthReport = await this.HealthCheckService.CheckHealthAsync();

        var entry = healthReport.Entries[entryName];
        Assert.AreEqual(HealthStatus.Healthy, entry.Status, entry.Description);
    }

    [TestMethod]
    public async Task CheckHealthWhenKnowledgeHealthyTest()
    {
        var entryName = $"{nameof(Knowledge).ToLower()}_vector_store";

        var healthReport = await this.HealthCheckService.CheckHealthAsync();

        var entry = healthReport.Entries[entryName];
        Assert.AreEqual(HealthStatus.Healthy, entry.Status, entry.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_ReturnsUnhealthy_WhenCollectionDoesNotExist()
    {
        var fakeCollection = new FakeVectorStoreCollection();
        var check = new VectorStoreHealthCheck<FakeEmbedding>(fakeCollection);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(FAKE_VECTOR_STORE, check, null, null)
        };

        var result = await check.CheckHealthAsync(context, CancellationToken.None);
        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }


    // ReSharper disable ClassNeverInstantiated.Local
    private sealed class FakeEmbedding : BaseEmbedding;
    // ReSharper restore ClassNeverInstantiated.Local
    
    private sealed class FakeVectorStoreCollection : VectorStoreCollection<Guid, FakeEmbedding>
    {
        // ReSharper disable NotNullOrRequiredMemberIsNotInitialized
        // ReSharper disable UnassignedGetOnlyAutoProperty
        public override string Name { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty
        // ReSharper restore NotNullOrRequiredMemberIsNotInitialized

        public override Task<bool> CollectionExistsAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Simulated failure");
        }

        public override Task EnsureCollectionExistsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task EnsureCollectionDeletedAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync(Guid key, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task UpsertAsync(FakeEmbedding record, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task UpsertAsync(IEnumerable<FakeEmbedding> records, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override IAsyncEnumerable<VectorSearchResult<FakeEmbedding>> SearchAsync<TInput>(TInput searchValue, int top, VectorSearchOptions<FakeEmbedding> options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override object GetService(Type serviceType, object serviceKey = null)
        {
            throw new NotImplementedException();
        }

        public override Task<FakeEmbedding> GetAsync(Guid key, RecordRetrievalOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override IAsyncEnumerable<FakeEmbedding> GetAsync(Expression<Func<FakeEmbedding, bool>> filter, int top, FilteredRecordRetrievalOptions<FakeEmbedding> options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}