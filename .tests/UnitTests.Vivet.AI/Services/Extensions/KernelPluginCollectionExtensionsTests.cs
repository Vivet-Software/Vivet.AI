using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models.Plugins;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class KernelPluginCollectionExtensionsTests
{
    private sealed class TestBuiltInPluginsContext : BaseBuiltInPluginsContext<object, object, object>;

    [TestMethod]
    public void ValidateContextTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void ValidateContextWhenKernelPluginCollectionIsNullThrowsArgumentNullExceptionTest()
    {
        KernelPluginCollection kernelPluginCollection = null;
        var pluginsContext = new TestBuiltInPluginsContext();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernelPluginCollection.ValidateContext(pluginsContext));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void ValidateContextWhenPluginsContextIsNullThrowsArgumentNullExceptionTest()
    {
        var kernelPluginCollection = new KernelPluginCollection();
        TestBuiltInPluginsContext pluginsContext = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernelPluginCollection.ValidateContext(pluginsContext));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddFromTypeTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddFromTypeWhenKernelPluginCollectionIsNullThrowsArgumentNullExceptionTest()
    {
        KernelPluginCollection kernelPluginCollection = null;
        var customPlugin = new CustomPlugin();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernelPluginCollection.AddFromType(customPlugin, serviceProvider));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddFromTypeWhenCustomPluginIsNullThrowsArgumentNullExceptionTest()
    {
        var kernelPluginCollection = new KernelPluginCollection();
        CustomPlugin customPlugin = null;
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernelPluginCollection.AddFromType(customPlugin, serviceProvider));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddFromTypeWhenServiceProviderIsNullThrowsArgumentNullExceptionTest()
    {
        var kernelPluginCollection = new KernelPluginCollection();
        var customPlugin = new CustomPlugin();
        IServiceProvider serviceProvider = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => kernelPluginCollection.AddFromType(customPlugin, serviceProvider));
        // ReSharper restore ExpressionIsAlwaysNull
    }
}