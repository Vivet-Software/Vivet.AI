using System;
using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Vivet.AI;

internal sealed class TestPlugin(TestPluginService testPluginService)
{
    // ReSharper disable UnusedMember.Local
    private readonly TestPluginService testPluginService = testPluginService ?? throw new ArgumentNullException(nameof(testPluginService));
    // ReSharper restore UnusedMember.Local

    [KernelFunction]
    [Description("Test Plugin.")]
    public void TestAsync()
    {
    }
}