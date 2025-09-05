using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;

namespace Tests.Vivet.AI.Services.Extensions;

[TestClass]
public class ChatMessageContentExtensionsTests
{
    [TestMethod]
    public void GetTokenUsageTest()
    {
        // Arrange: use ExpandoObject so dynamic binding works
        dynamic usage = new ExpandoObject();
        usage.InputTokenCount = 5;
        usage.OutputTokenCount = 10;

        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["Usage"] = usage
            }
        };

        var result = content.GetTokenUsage();
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.InputTokens);
        Assert.AreEqual(10, result.OutputTokens);
    }

    [TestMethod]
    public void GetTokenUsageThrowsArgumentNullExceptionTest()
    {
        ChatMessageContent content = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(content.GetTokenUsage);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetTokenUsageWhenMetadataIsNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = null
        };

        var result = content.GetTokenUsage();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetTokenUsageWhenUsageKeyIsMissingTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>()
        };

        var result = content.GetTokenUsage();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetTokenUsageWhenWhenNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["Usage"] = null
            }
        };

        var result = content.GetTokenUsage();

        Assert.IsNull(result);
    }
}