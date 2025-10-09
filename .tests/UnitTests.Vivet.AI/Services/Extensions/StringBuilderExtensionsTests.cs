using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.Plugins;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class StringBuilderExtensionsTests
{
    private sealed class DummyContext
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public string Key { get; set; } = "Value";
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
    private sealed class DummyOverride : BaseConfigOverrides
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public string Mode { get; set; } = "TestMode";
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
    private sealed class TestCustomPlugin : CustomPlugin
    {
        public TestCustomPlugin(string name, Dictionary<string, object> context)
        {
            Name = name;
            Context = context;
        }
    }

    [TestMethod]
    public void AppendBuiltInPluginContextTest()
    {
        var stringBuilder = new StringBuilder();
        var ctx = new DummyContext
        {
            Key = "ABC"
        };

        stringBuilder.AppendBuiltInPluginContext("PluginA", ctx);

        var output = stringBuilder.ToString();
        Assert.IsTrue(output.Contains("PluginA: context="));
        Assert.IsTrue(output.Contains("Key"));
        Assert.IsTrue(output.Contains("ABC"));
    }

    [TestMethod]
    public void AppendBuiltInPluginContextWhenStringBuilderIsNullTest()
    {
        StringBuilder stringBuilder = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => stringBuilder.AppendBuiltInPluginContext("Test", new DummyContext()));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AppendBuiltInPluginContextWhenNameIsNullTest()
    {
        var stringBuilder = new StringBuilder();
        Assert.ThrowsException<ArgumentNullException>(() => stringBuilder.AppendBuiltInPluginContext<DummyContext>(null));
    }

    [TestMethod]
    public void AppendBuiltInPluginContextWhenContextIsNullTest()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendBuiltInPluginContext<DummyContext>("PluginA");

        Assert.AreEqual(0, stringBuilder.Length);
    }


    [TestMethod]
    public void AppendBuiltInPluginContextConfigOverrideTest()
    {
        var builder = new StringBuilder();
        var context = new DummyContext { Key = "XYZ" };
        var configOverrides = new DummyOverride { Mode = "Active" };

        builder.AppendBuiltInPluginContext("PluginB", context, configOverrides);

        var result = builder.ToString();
        Assert.IsTrue(result.Contains("PluginB:"));
        Assert.IsTrue(result.Contains("context="));
        Assert.IsTrue(result.Contains("configOverrides="));
    }

    [TestMethod]
    public void AppendBuiltInPluginContextConfigOverrideWhenStringBuilderIsNullTest()
    {
        StringBuilder stringBuilder = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => stringBuilder.AppendBuiltInPluginContext("Test", new DummyContext(), new DummyOverride()));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AppendBuiltInPluginContextConfigOverrideWhenNameIsNullTest()
    {
        var stringBuilder = new StringBuilder();
        Assert.ThrowsException<ArgumentNullException>(() => stringBuilder.AppendBuiltInPluginContext<DummyContext, DummyOverride>(null));
    }

    [TestMethod]
    public void AppendBuiltInPluginContextConfigOverrideWhenContextIsNullTest()
    {
        var sb = new StringBuilder();
        sb.AppendBuiltInPluginContext<DummyContext, DummyOverride>("PluginB");
        Assert.AreEqual(0, sb.Length);
    }


    [TestMethod]
    public void AppendCustomPluginsContextWhenStringBuilderIsNullTest()
    {
        StringBuilder stringBuilder = null;
        // ReSharper disable CollectionNeverUpdated.Local
        var plugins = new List<CustomPlugin>();
        // ReSharper restore CollectionNeverUpdated.Local

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => stringBuilder.AppendCustomPluginsContext(plugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AppendCustomPluginsContextWhenPluginsIsNullTest()
    {
        var stringBuilder = new StringBuilder();
        Assert.ThrowsException<ArgumentNullException>(() => stringBuilder.AppendCustomPluginsContext(null));
    }

    [TestMethod]
    public void AppendCustomPluginsContextWhenEmptyContextTest()
    {
        var stringBuilder = new StringBuilder();
        var plugins = new List<CustomPlugin>
        {
            new TestCustomPlugin("PluginA", new Dictionary<string, object>())
        };

        stringBuilder.AppendCustomPluginsContext(plugins);
        Assert.AreEqual(0, stringBuilder.Length);
    }

    [TestMethod]
    public void AppendCustomPluginsContextWhenSimpleValuesTest()
    {
        var stringBuilder = new StringBuilder();
        var plugins = new List<CustomPlugin>
        {
            new TestCustomPlugin("PluginA", new Dictionary<string, object>
            {
                { "SimpleKey", "Value" }
            })
        };

        stringBuilder.AppendCustomPluginsContext(plugins);

        var output = stringBuilder.ToString();
        Assert.IsTrue(output.Contains("PluginA:"));
        Assert.IsTrue(output.Contains("SimpleKey=Value"));
    }

    [TestMethod]
    public void AppendCustomPluginsContextWhenComplexValuesTest()
    {
        var stringBuilder = new StringBuilder();
        var plugins = new List<CustomPlugin>
        {
            new TestCustomPlugin("PluginB", new Dictionary<string, object>
            {
                { "Complex", new DummyContext { Key = "ZZZ" } }
            })
        };

        stringBuilder.AppendCustomPluginsContext(plugins);

        var output = stringBuilder.ToString();
        Assert.IsTrue(output.Contains("PluginB:"));
        Assert.IsTrue(output.Contains("Complex="));
        Assert.IsTrue(output.Contains("Key"));
        Assert.IsTrue(output.Contains("ZZZ"));
    }

    [TestMethod]
    public void AppendCustomPluginsContextWhenAllValuesIsNullTest()
    {
        var stringBuilder = new StringBuilder();
        var plugins = new List<CustomPlugin>
        {
            new TestCustomPlugin("PluginC", new Dictionary<string, object>
            {
                { "NullValue", null }
            })
        };

        stringBuilder.AppendCustomPluginsContext(plugins);

        Assert.AreEqual(0, stringBuilder.Length);
    }
}