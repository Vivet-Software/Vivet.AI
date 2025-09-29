using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Requests.Agent.Models.Plugins;
using Vivet.AI.Services.Requests.Chat.Models.Plugins;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class ChatHistoryExtensionsTests
{
    [TestMethod]
    public void AddChatSystemPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddChatSystemPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        const string ADDITIONAL_SYSTEM_MESSAGE = "Test message";

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddChatSystemPrompt<string>(ADDITIONAL_SYSTEM_MESSAGE));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddChatPluginsContextPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddChatPluginsContextPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        var chatPlugins = new ChatPlugins();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddChatPluginsContextPrompt(chatPlugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddChatPluginsContextPromptWhenChatPluginsIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        ChatPlugins chatPlugins = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddChatPluginsContextPrompt(chatPlugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddAgentPluginsContextPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddAgentPluginsContextPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        var agentPlugins = new AgentPlugins();
        var parentAgentPlugins = new AgentPlugins();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddAgentPluginsContextPrompt(agentPlugins, parentAgentPlugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddAgentPluginsContextPromptWhenAgentPluginsIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        AgentPlugins agentPlugins = null;
        var parentAgentPlugins = new AgentPlugins();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddAgentPluginsContextPrompt(agentPlugins, parentAgentPlugins));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddAgentPluginsContextPromptWhenParentAgentPluginsIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        var agentPlugins = new AgentPlugins();
        AgentPlugins parentAgentPlugins = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddAgentPluginsContextPrompt(agentPlugins, parentAgentPlugins));
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
        const string QUESTION = "question";
        KernelContent[] blobContents = [];

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddChatUserPrompt(QUESTION, blobContents));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddChatUserPromptWhenQuestionIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        string question = null;
        KernelContent[] blobContents = [];

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddChatUserPrompt(question, blobContents));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddChatUserPromptWhenblobContentsIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        string question = "question";
        KernelContent[] blobContents = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddChatUserPrompt(question, blobContents));
        // ReSharper restore ExpressionIsAlwaysNull
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
        var blobContent = new BinaryContent();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddMetadataPrompt<object>(blobContent, 0, 0));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddMetadataPromptWhenBlobContentIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        BinaryContent blobContent = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddMetadataPrompt<object>(blobContent, 0, 0));
        // ReSharper restore ExpressionIsAlwaysNull
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
        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddSummarizationMemoryPrompt(QUESTION, ANSWER, 50));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddSuummarizationMemoryPromptWhenQuestionIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        const string ANSWER = "A";

        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddSummarizationMemoryPrompt(null, ANSWER, 50));
    }

    [TestMethod]
    public void AddSuummarizationMemoryPromptWhenAnswerIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        const string QUESTION = "Q";

        Assert.ThrowsExactly<ArgumentNullException>(() => chatHistory.AddSummarizationMemoryPrompt(QUESTION, null, 50));
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