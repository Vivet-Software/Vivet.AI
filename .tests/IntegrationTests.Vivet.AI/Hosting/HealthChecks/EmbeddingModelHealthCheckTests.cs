using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Hosting.HealthChecks.Extensions;

namespace IntegrationTests.Vivet.AI.Hosting.HealthChecks;

[TestClass]
public class EmbeddingModelHealthCheckTests : BaseTests
{
    private const string FAKE_EMBEDDING_SERVICE_ID = "FAKE_EMBEDDING_SERVICE";

    private HealthCheckService HealthCheckService => this.ServiceProvider.GetRequiredService<HealthCheckService>();

    [TestInitialize]
    public override void TestSetup()
    {
        base.TestSetup();

        this.services
            .AddKeyedSingleton<IEmbeddingGenerator<string, Embedding<float>>>(FAKE_EMBEDDING_SERVICE_ID, new FakeEmbeddingGenerator())
            .AddHealthChecks()
            .AddEmbeddingModelCheck(FAKE_EMBEDDING_SERVICE_ID);
    }

    [TestMethod]
    public async Task CheckHealthAsyncWhenIsHealthyTest()
    {
        var healthReport = await this.HealthCheckService.CheckHealthAsync();

        var entry = healthReport.Entries[ServiceIds.EMBEDDING_SERVICE_ID];
        Assert.AreEqual(HealthStatus.Healthy, entry.Status, entry.Description);
    }

    [TestMethod]
    public async Task CheckHealthWhenUnhealthyTest()
    {
        var report = await this.HealthCheckService.CheckHealthAsync();
        var entry = report.Entries[FAKE_EMBEDDING_SERVICE_ID];

        Assert.AreEqual(HealthStatus.Unhealthy, entry.Status);
    }


    private sealed class FakeEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
    {
        public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> data, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new GeneratedEmbeddings<Embedding<float>>());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType, object serviceKey = null)
        {
            throw new NotImplementedException();
        }
    }
}