using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Hosting.HealthChecks.Extensions;

namespace IntegrationTests.Vivet.AI.Hosting.HealthChecks;

[TestClass]
public class TranscriptionModelHealthCheckTests : BaseTests
{
    private const string FAKE_TRANSCRIPTION_SERVICE_ID = "FAKE_TRANSCRIPTION_SERVICE";

    private HealthCheckService HealthCheckService => this.ServiceProvider.GetRequiredService<HealthCheckService>();

    [TestInitialize]
    public override void TestSetup()
    {
        base.TestSetup();

        this.services
            .AddKeyedSingleton<IAudioToTextService>(FAKE_TRANSCRIPTION_SERVICE_ID, new FakeAudioToTextService())
            .AddKeyedSingleton(FAKE_TRANSCRIPTION_SERVICE_ID, new PromptExecutionSettings())
            .AddHealthChecks()
            .AddTranscriptionModelCheck(FAKE_TRANSCRIPTION_SERVICE_ID, FAKE_TRANSCRIPTION_SERVICE_ID);
    }

    [TestMethod]
    public async Task CheckHealthWhenIsHealthyTest()
    {
        var healthReport = await this.HealthCheckService.CheckHealthAsync();

        var entry = healthReport.Entries[ServiceIds.TRANSCRIPTION_SERVICE_ID];
        Assert.AreEqual(HealthStatus.Healthy, entry.Status, entry.Description);
    }

    [TestMethod]
    public async Task CheckHealthkWhenIsUnhealthyTest()
    {
        var report = await this.HealthCheckService.CheckHealthAsync();
        var entry = report.Entries[FAKE_TRANSCRIPTION_SERVICE_ID];

        Assert.AreEqual(HealthStatus.Unhealthy, entry.Status);
    }


    private sealed class FakeAudioToTextService : IAudioToTextService
    {
        // ReSharper disable NotNullOrRequiredMemberIsNotInitialized
        // ReSharper disable UnassignedGetOnlyAutoProperty
        public IReadOnlyDictionary<string, object> Attributes { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty
        // ReSharper restore NotNullOrRequiredMemberIsNotInitialized

        public Task<IReadOnlyList<TextContent>> GetTextContentsAsync(AudioContent content, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}