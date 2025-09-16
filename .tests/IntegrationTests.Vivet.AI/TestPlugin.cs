using System;
using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Vivet.AI;

internal sealed class TestPlugin(TestPluginService knowledgeService)
{
    private readonly TestPluginService testPluginService = knowledgeService ?? throw new ArgumentNullException(nameof(knowledgeService));

    [KernelFunction]
    [Description("Test Plugin.")]
    public void TestAsync()
    {
    }
}