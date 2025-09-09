using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;
using Vivet.AI.Services.Responses.Embeddings.Memory.Models;

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
        const string SYSTEM_MESSAGE = "Test message";

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatSystemPrompt(SYSTEM_MESSAGE));
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AddChatMemoryPromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddChatMemoryPromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        var memoryResults = Array.Empty<MemoryResult>();
        const int COUNTERPART_CONTEXT_QUERY_LIMIT = 1;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatMemoryPrompt(memoryResults, COUNTERPART_CONTEXT_QUERY_LIMIT));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddChatMemoryPromptWhenMemoryResultsIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();

        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatMemoryPrompt(null, 1));
    }


    [TestMethod]
    public void AddChatKnowledgePromptTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void AddChatKnowledgePromptWhenChatHistoryIsNullThrowsArgumentNullExceptionTest()
    {
        ChatHistory chatHistory = null;
        var knowledgeResults = Array.Empty<KnowledgeResult>();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatKnowledgePrompt(knowledgeResults));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddChatKnowledgePromptWhenKnowledgeResultsIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();

        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatKnowledgePrompt(null));
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
        var dataUris = Array.Empty<string>();

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddChatUserPrompt(QUESTION, dataUris));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddChatUserPromptWhenQuestionIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        var dataUris = Array.Empty<string>();

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
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddMetadataPrompt<object>("datauri", 0, 0));
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
        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddSuummarizationMemoryPrompt(QUESTION, ANSWER, 50));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void AddSuummarizationMemoryPromptWhenQuestionIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        const string ANSWER = "A";

        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddSuummarizationMemoryPrompt(null, ANSWER, 50));
    }

    [TestMethod]
    public void AddSuummarizationMemoryPromptWhenAnswerIsNullThrowsArgumentNullExceptionTest()
    {
        var chatHistory = new ChatHistory();
        const string QUESTION = "Q";

        Assert.ThrowsException<ArgumentNullException>(() => chatHistory.AddSuummarizationMemoryPrompt(QUESTION, null, 50));
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
                Items = { new BinaryContent("data:image/png;base64,AAAA") }
            }
        };

        var result = chatHistory.GetPromptAsText();
        StringAssert.Contains(result, "assistant:");
        StringAssert.Contains(result, "[BinaryContent]");

        //"assistant:\r\n[BinaryContent]\r\n\r\n"
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