using System;
using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models.Plugins;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class ChatHistoryExtensionsTests
{
    private sealed class TestBuiltInPluginsContext : BaseBuiltInPluginsContext<object, object, object>;

    [TestMethod]
    public void AddChatSystemPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddChatSystemPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        const string SYSTEM_MESSAGE = "Test message";

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatSystemPrompt<string>(SYSTEM_MESSAGE));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddBuiltInPluginsContextPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddBuiltInPluginsContextPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddBuiltInPluginsContextPrompt(new TestBuiltInPluginsContext()));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddBuiltInPluginsContextPromptWhenPluginsContextIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddBuiltInPluginsContextPrompt<object, object, object>(null));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddCustomPluginContextPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddCustomPluginContextPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddCustomPluginContextPrompt(new List<CustomPlugin>()));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddCustomPluginContextPromptWhenCustomPluginsIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddCustomPluginContextPrompt(null));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddChatUserPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddChatUserPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        const string QUESTION = "Test question";
        var dataUris = Array.Empty<BinaryContent>();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatUserPrompt(QUESTION, dataUris));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddChatUserPromptWhenQuestionIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        var dataUris = Array.Empty<BinaryContent>();

        Assert.Throws<ArgumentNullException>(() => chatHistory.AddChatUserPrompt(null, dataUris));
    }

    [TestMethod]
    public void AddChatUserPromptWhenDataUrisIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        const string QUESTION = "Test question";

        Assert.Throws<ArgumentNullException>(() => chatHistory.AddChatUserPrompt(QUESTION, null));
    }


    [TestMethod]
    public void AddMetadataPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddMetadataPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddMetadataPrompt<object>(new BinaryContent(), 0, 0));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddMetadataPromptWhenDataUriIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();

        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddMetadataPrompt<object>(null, 0, 0));
    }


    [TestMethod]
    public void AddSuummarizationMemoryPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddSuummarizationMemoryPromptChatHistoryIsWhenNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        const string QUESTION = "Q";
        const string ANSWER = "A";

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddSummarizationMemoryPrompt(QUESTION, ANSWER, 50));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddSuummarizationMemoryPromptWhenQuestionIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        const string ANSWER = "A";

        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddSummarizationMemoryPrompt(null, ANSWER, 50));
    }

    [TestMethod]
    public void AddSuummarizationMemoryPromptWhenAnswerIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        const string QUESTION = "Q";

        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddSummarizationMemoryPrompt(QUESTION, null, 50));
    }


    [TestMethod]
    public void GetPromptAsTextWhenTextContentTest()
    {
        var chatHistory = new ChatHistory
        {
            new ChatMessageContent
            {
                Role = AuthorRole.System,
                Items = { new TextContent("System message") }
            },
            new ChatMessageContent
            {
                Role = AuthorRole.User,
                Items = { new TextContent("User message") }
            }
        };

        var result = chatHistory.GetPromptAsText();
        StringAssert.Contains(result, "system:");
        StringAssert.Contains(result, "user:");
        StringAssert.Contains(result, "System message");
        StringAssert.Contains(result, "User message");
    }

    [TestMethod]
    public void GetPromptAsTextWhenBinaryContentTest()
    {
        var chatHistory = new ChatHistory
        {
            new ChatMessageContent
            {
                Role = AuthorRole.Assistant,
                Items =
                {
                    new BinaryContent("data:image/png;base64,AAAA")
                }
            }
        };

        var result = chatHistory.GetPromptAsText();
        StringAssert.Contains(result, "assistant:");
        StringAssert.Contains(result, "[BinaryContent]");
    }

    [TestMethod]
    public void GetPromptAsTextWhenUnknownContentTypeTest()
    {
        var chatHistory = new ChatHistory
        {
            new ChatMessageContent
            {
                Role = AuthorRole.Tool,
                Items =
                {
                    new ChatMessageContent()
                }
            }
        };

        // Act
        var result = chatHistory.GetPromptAsText();

        // Assert
        StringAssert.Contains(result, "[ChatMessageContent]");
    }

    [TestMethod]
    public void GetPromptAsTextWhenEmptyChatHistoryTest()
    {
        // Arrange
        var chatHistory = new ChatHistory();

        // Act
        var result = chatHistory.GetPromptAsText();

        // Assert
        Assert.AreEqual(string.Empty, result.Trim());
    }
}