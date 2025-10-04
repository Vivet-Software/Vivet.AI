using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models.Plugins;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class StringBuilderExtensionsTests
{
    [TestMethod]
    public void AppendBuiltInPluginContextTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AppendBuiltInPluginContextWhenStringBuilderIsNullThrowsArgumentNullExceptionTest()
    {
        StringBuilder stringBuilder = null;
        var context = new object();
        const string NAME = "name";

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => stringBuilder.AppendBuiltInPluginContext(context, NAME));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AppendBuiltInPluginContextWhenContextIsNullTest()
    {
        var stringBuilder = new StringBuilder();
        object context = null;
        const string NAME = "name";

        // ReSharper disable ExpressionIsAlwaysNull
        stringBuilder.AppendBuiltInPluginContext(context, NAME);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AppendBuiltInPluginContextWhenNameIsNullThrowsArgumentNullExceptionTest()
    {
        StringBuilder stringBuilder = null;
        var context = new object();
        const string NAME = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => stringBuilder.AppendBuiltInPluginContext(context, NAME));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AppendCustomPluginsContextTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AppendCustomPluginsContextWhenStringBuilderIsNullThrowsArgumentNullExceptionTest()
    {
        StringBuilder stringBuilder = null;
        CustomPlugin[] plugins = [];

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => stringBuilder.AppendCustomPluginsContext(plugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AppendCustomPluginsContextWhenContextIsNullThrowsArgumentNullExceptionTest()
    {
        var stringBuilder = new StringBuilder();
        CustomPlugin[] plugins = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => stringBuilder.AppendCustomPluginsContext(plugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }
}