using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class KernelExtensionsTests
{
    [TestMethod]
    public void GetAutoFunctionInvocationContextsTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void GetAutoFunctionInvocationContextsWhenKernelIsNullThrowsArgumentNullExceptionTest()
    {
        Kernel kernel = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(kernel.GetAutoFunctionInvocationContexts);
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddAutoFunctionInvocationContextTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddAutoFunctionInvocationContextWhenKernelIsNullThrowsArgumentNullExceptionTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddAutoFunctionInvocationContextWhenContextIsNullThrowsArgumentNullExceptionTest()
    {
        Assert.Inconclusive();
    }


    [TestMethod]
    public void GetAgentIdTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void GetAgentIdWhenKernelIsNullThrowsArgumentNullExceptionTest()
    {
        Kernel kernel = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernel.GetAgentId());
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void GetAgentResponseCallbackTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void GetAgentResponseCallbackWhenKernelIsNullThrowsArgumentNullExceptionTest()
    {
        Kernel kernel = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(kernel.GetAgentResponseCallback);
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddFiltersTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddFiltersWhenKernelIsNullThrowsArgumentNullExceptionTest()
    {
        Kernel kernel = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(kernel.AddFilters);
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddCustomPluginsTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddCustomPluginsWhenKernelIsNullThrowsArgumentNullExceptionTest()
    {
        Kernel kernel = null;
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        // ReSharper disable CollectionNeverUpdated.Local
        var customPlugins = new List<CustomPlugin>();
        // ReSharper restore CollectionNeverUpdated.Local

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernel.AddCustomPlugins(serviceProvider, customPlugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddCustomPluginsWhenServiceProviderIsNullThrowsArgumentNullExceptionTest()
    {
        var kernel = new Kernel();
        IServiceProvider serviceProvider = null;
        // ReSharper disable CollectionNeverUpdated.Local
        var customPlugins = new List<CustomPlugin>();
        // ReSharper restore CollectionNeverUpdated.Local

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernel.AddCustomPlugins(serviceProvider, customPlugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddCustomPluginsWhenCustomPluginsIsNullThrowsArgumentNullExceptionTest()
    {
        Kernel kernel = null;
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        List<CustomPlugin> customPlugins = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernel.AddCustomPlugins(serviceProvider, customPlugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddPluginConfigOverridesTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenKernelIsNullThrowsArgumentNullExceptionTest()
    {
        Kernel kernel = null;
        var configOverrides = new BuiltInPluginsConfigOverrides();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernel.AddBuiltInPluginConfigOverrides(configOverrides));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenConfigOverridesIsNullTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenConfigOverridesMemoryTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenConfigOverridesMemoryIsNullTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenConfigOverridesKnowledgeTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenConfigOverridesKnowledgeIsNullTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenConfigOverridesWebSearchTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddPluginConfigOverridesWhenConfigOverridesWebSearchIsNullTest()
    {
        Assert.Inconclusive();
    }
}