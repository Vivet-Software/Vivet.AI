using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Extensions;
using Vivet.AI.Extensions.Consts;

namespace IntegrationTests.Vivet.AI.Hosting.HealthChecks;

[TestClass]
public class ChatModelHealthCheckTests : BaseTests
{
    private const string FAKE_CHAT_SERVICE_ID = "FAKE_CHAT_SERVICE";

    private HealthCheckService HealthCheckService => this.ServiceProvider.GetRequiredService<HealthCheckService>();

    [TestInitialize]
    public override void TestSetup()
    {
        base.TestSetup();

        this.services
            .AddKeyedSingleton<IChatCompletionService>(FAKE_CHAT_SERVICE_ID, new FakeChatCompletionService())
            .AddKeyedSingleton(FAKE_CHAT_SERVICE_ID, new PromptExecutionSettings())
            .AddHealthChecks()
            .AddChatModelCheck(FAKE_CHAT_SERVICE_ID, FAKE_CHAT_SERVICE_ID);
    }

    [TestMethod]
    public async Task ChatCheckHealthWhenIsHealthyTest()
    {
        var healthReport = await this.HealthCheckService.CheckHealthAsync();

        var entry = healthReport.Entries[ServiceIds.CHAT_SERVICE_ID];
        Assert.AreEqual(HealthStatus.Healthy, entry.Status, entry.Description);
    }

    [TestMethod]
    public async Task MetadataCheckHealthWhenIsHealthyTest()
    {
        var healthReport = await this.HealthCheckService.CheckHealthAsync();

        var entry = healthReport.Entries[ServiceIds.METADATA_SERVICE_ID];
        Assert.AreEqual(HealthStatus.Healthy, entry.Status, entry.Description);
    }

    [TestMethod]
    public async Task SummarizationCheckHealthWhenIsHealthyTest()
    {
        var healthReport = await this.HealthCheckService.CheckHealthAsync();

        var entry = healthReport.Entries[ServiceIds.SUMMARIZATION_SERVICE_ID];
        Assert.AreEqual(HealthStatus.Healthy, entry.Status, entry.Description);
    }

    [TestMethod]
    public async Task CheckHealthkWhenIsUnhealthyTest()
    {
        var report = await this.HealthCheckService.CheckHealthAsync();
        var entry = report.Entries[FAKE_CHAT_SERVICE_ID];

        Assert.AreEqual(HealthStatus.Unhealthy, entry.Status);
    }


    private sealed class FakeChatCompletionService : IChatCompletionService
    {
        // ReSharper disable NotNullOrRequiredMemberIsNotInitialized
        // ReSharper disable UnassignedGetOnlyAutoProperty
        public IReadOnlyDictionary<string, object> Attributes { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty
        // ReSharper restore NotNullOrRequiredMemberIsNotInitialized

        public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}