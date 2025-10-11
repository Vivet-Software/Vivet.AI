using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class ChatMessageContentExtensionsTests
{
    [TestMethod]
    public void GetAgentIdTest()
    {
        var id = Guid.NewGuid();
        const string NAME = "name";

        var chatMessageContent = new ChatMessageContent
        {
            AuthorName = $"{NAME} [{id}]"
        };

        var agentId = chatMessageContent
            .GetAgentId();

        Assert.IsNotNull(agentId);
        Assert.AreEqual(id, agentId);
    }

    [TestMethod]
    public void GetAgentIdWhenBracketStartIsMissingTest()
    {
        var id = Guid.NewGuid().ToString();
        const string NAME = "name";

        var chatMessageContent = new ChatMessageContent
        {
            AuthorName = $"{NAME} {id}]"
        };

        var agentId = chatMessageContent
            .GetAgentId();

        Assert.IsNull(agentId);
    }

    [TestMethod]
    public void GetAgentIdWhenBracketEndIsMissingTest()
    {
        var id = Guid.NewGuid().ToString();
        const string NAME = "name";

        var chatMessageContent = new ChatMessageContent
        {
            AuthorName = $"{NAME} [{id}"
        };

        var agentId = chatMessageContent
            .GetAgentId();

        Assert.IsNull(agentId);
    }

    [TestMethod]
    public void GetAgentIdWhenBracketStartIsAfterBracketEndTest()
    {
        var id = Guid.NewGuid().ToString();
        const string NAME = "name";

        var chatMessageContent = new ChatMessageContent
        {
            AuthorName = $"{NAME} ]{id}["
        };

        var agentId = chatMessageContent
            .GetAgentId();

        Assert.IsNull(agentId);
    }

    [TestMethod]
    public void GetAgentIdWhenAuthorNameIsNullTest()
    {
        var chatMessageContent = new ChatMessageContent
        {
            AuthorName = null
        };

        var agentId = chatMessageContent
            .GetAgentId();

        Assert.IsNull(agentId);
    }


    [TestMethod]
    public void GetExternalIdTest()
    {
        const string EXPECTED = "id";

        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["Id"] = EXPECTED
            }
        };

        var result = content.GetExternalId();
        Assert.AreEqual(EXPECTED, result);
    }

    [TestMethod]
    public void GetExternalIdWhenChatMessageContentIsNullTest()
    {
        ChatMessageContent content = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(content.GetExternalId);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetExternalIdWhenMetadataIsNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = null
        };

        var result = content.GetExternalId();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetExternalIdWhenIdKeyIsMissingTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>()
        };

        var result = content.GetExternalId();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetExternalIdWhenWhenNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["Id"] = null
            }
        };

        var result = content.GetExternalId();

        Assert.IsNull(result);
    }


    [TestMethod]
    public void GetCreatedAtTest()
    {
        var expected = DateTimeOffset.UtcNow;

        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["CreatedAt"] = expected.ToString()
            }
        };

        var result = content.GetCreatedAt();
        Assert.IsNotNull(result);
        Assert.AreEqual(expected.ToString(), result.Value.ToString());
    }

    [TestMethod]
    public void GetCreatedAtWhenChatMessageContentIsNullTest()
    {
        ChatMessageContent content = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => content.GetCreatedAt());
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetCreatedAtWhenMetadataIsNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = null
        };

        var result = content.GetCreatedAt();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetCreatedAtWhenCreatedAtKeyIsMissingTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>()
        };

        var result = content.GetCreatedAt();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetCreatedAtWhenWhenNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["CreatedAt"] = null
            }
        };

        var result = content.GetCreatedAt();

        Assert.IsNull(result);
    }


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
    public void GetTokenUsageWhenChatMessageContentIsNullTest()
    {
        ChatMessageContent content = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(content.GetTokenUsage);
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


    [TestMethod]
    public void GetFinishReasonIdTest()
    {
        const string EXPECTED = "finish";

        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["FinishReason"] = EXPECTED
            }
        };

        var result = content.GetFinishReason();
        Assert.AreEqual(EXPECTED, result);
    }

    [TestMethod]
    public void GetFinishReasonlIdWhenChatMessageContentIsNullTest()
    {
        ChatMessageContent content = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(content.GetFinishReason);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetFinishReasonlIdWhenMetadataIsNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = null
        };

        var result = content.GetFinishReason();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetFinishReasonlIdWhenIdKeyIsMissingTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>()
        };

        var result = content.GetFinishReason();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetFinishReasonlIdWhenWhenFinishReasonIsNullTest()
    {
        var content = new ChatMessageContent
        {
            Metadata = new Dictionary<string, object>
            {
                ["FinishReason"] = null
            }
        };

        var result = content.GetFinishReason();

        Assert.IsNull(result);
    }
}