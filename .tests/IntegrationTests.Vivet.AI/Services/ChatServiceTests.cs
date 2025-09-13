using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Chat;
using Vivet.AI.Services.Requests.Embedding.Knowledge;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Responses;

namespace IntegrationTests.Vivet.AI.Services;

[TestClass]
public class ChatServiceTests : BaseTests
{
    private IChatService ChatService => this.ServiceProvider.GetRequiredService<IChatService>();

    internal sealed class JsonClass
    {
        public bool SummerAlways { get; set; }
    }

    [TestMethod]
    public async Task ChatTest()
    {
        const string SYSTEM_MESSAGE = "You are an expert in meteorology.";
        const string QUESTION = "Is it always summer in Denmark?";

        var onMemoryIndexedTask = new TaskCompletionSource<bool>();

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                SystemMessage = SYSTEM_MESSAGE,
                Question = QUESTION,
                UserId = this.userId,
                CurrentThreadId = Guid.NewGuid().ToString()
            }, memoryResponse =>
            {
                try
                {
                    Assert.IsNotNull(memoryResponse);
                    Assert.IsNotNull(memoryResponse.TokenUsage);
                    Assert.AreEqual(2, memoryResponse.TotalEmbeddings);
                    Assert.IsTrue(memoryResponse.TotalEmbeddingsSize > 0);

                    onMemoryIndexedTask
                        .SetResult(true);
                }
                catch (Exception ex)
                {
                    onMemoryIndexedTask
                        .SetException(ex);
                }

                return Task.CompletedTask;
            });

        Assert.IsNotNull(response);
        Assert.IsNull(response.ErrorMessage);
        Assert.IsNull(response.Thinking);
        Assert.IsNotNull(response.Answer);
        Assert.IsNotNull(response.Reasoning);
        Assert.IsNotNull(response.RawResponse);
        Assert.IsNotNull(response.InputPrompt);
        Assert.AreEqual("en", response.Language);
        Assert.IsNotNull(response.TokenUsage);
        Assert.IsTrue(response.TokenUsage.InputTokens > 200);
        Assert.IsTrue(response.TokenUsage.OutputTokens > 50);

        await onMemoryIndexedTask.Task;
    }



    [TestMethod]
    public async Task ChatTest222222()
    {
        const string QUESTION = "Turn on the table lamp.";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                UserId = this.userId,
                CurrentThreadId = Guid.NewGuid().ToString()
            });

        Assert.IsNotNull(response);
    }



    [TestMethod]
    public async Task ChatWhenJsonResponseTest()
    {
        const string SYSTEM_MESSAGE = "You are an expert in meteorology.";
        const string QUESTION = "Is it always summer in Denmark? Please respond in the following json format: { \"SummerAlways\": \"true/false\" }";

        var response = await this.ChatService
            .ChatAsync<JsonClass>(new ChatRequest
            {
                SystemMessage = SYSTEM_MESSAGE,
                Question = QUESTION,
                UserId = this.userId,
                CurrentThreadId = Guid.NewGuid().ToString()
            }, async memoryResponse =>
            {
                await Task.CompletedTask;

                Assert.IsNotNull(memoryResponse);
                Assert.IsNotNull(memoryResponse.TokenUsage);
                Assert.AreEqual(2, memoryResponse.TotalEmbeddings);
                Assert.IsTrue(memoryResponse.TotalEmbeddingsSize > 0);
            });

        Assert.IsNotNull(response);
        Assert.IsNull(response.ErrorMessage);
        Assert.IsNull(response.Thinking);
        Assert.IsNotNull(response.Answer);
        Assert.IsFalse(response.Answer.SummerAlways);
        Assert.IsNotNull(response.Reasoning);
        Assert.IsNotNull(response.RawResponse);
        Assert.IsNotNull(response.InputPrompt);
        Assert.AreEqual("en", response.Language);
        Assert.IsNotNull(response.TokenUsage);
        Assert.IsTrue(response.TokenUsage.InputTokens > 300);
        Assert.IsTrue(response.TokenUsage.OutputTokens > 40);
    }

    [TestMethod]
    public async Task ChatWhenInlineJsonTest()
    {
        const string SYSTEM_MESSAGE = "You are an expert in meteorology.";
        const string QUESTION = "Is it always summer in Denmark? Please respond both with natural language text and also include the following json format: { \"SummerAlways\": \"true/false\" } in the response";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                SystemMessage = SYSTEM_MESSAGE,
                Question = QUESTION,
                UserId = this.userId,
                CurrentThreadId = Guid.NewGuid().ToString()
            }, async memoryResponse =>
            {
                await Task.CompletedTask;

                Assert.IsNotNull(memoryResponse);
                Assert.IsNotNull(memoryResponse.TokenUsage);
                Assert.AreEqual(2, memoryResponse.TotalEmbeddings);
                Assert.IsTrue(memoryResponse.TotalEmbeddingsSize > 0);
            });

        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Answer);
        Assert.IsTrue(response.Answer.Contains("{ \"SummerAlways\": \"false\" }"));
    }

    [TestMethod]
    public async Task ChatWhenStreamingResponseTest()
    {
        const string SYSTEM_MESSAGE = "You are an expert in meteorology.";
        const string QUESTION = "Is it always summer in Denmark?";

        var request = new ChatRequest
        {
            SystemMessage = SYSTEM_MESSAGE,
            Question = QUESTION,
            UserId = this.userId,
            CurrentThreadId = Guid.NewGuid().ToString()
        };

        var streamedResults = new List<string>();
        var onChatCompletedTask = new TaskCompletionSource<bool>();

        await foreach (var chunk in this.ChatService.ChatStreamingAsync(request, null, chatResponse =>
                       {
                           try
                           {
                               Assert.IsNotNull(chatResponse);
                               Assert.IsNull(chatResponse.ErrorMessage);
                               Assert.IsNotNull(chatResponse.Answer);
                               Assert.AreEqual(chatResponse.RawResponse, string.Join("", streamedResults));
                               Assert.IsNull(chatResponse.TokenUsage);

                               onChatCompletedTask
                                   .SetResult(true);
                           }
                           catch (Exception ex)
                           {
                               onChatCompletedTask
                                   .SetException(ex);
                           }

                           return Task.CompletedTask;
                       }))
        {
            Assert.IsTrue(chunk.Length > 0);

            streamedResults
                .Add(chunk);
        }

        await onChatCompletedTask.Task;
    }

    [TestMethod]
    public async Task ChatWhenOverrideModelTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ChatWhenMemoryTest()
    {
        var embeddingMemoryService = this.ServiceProvider.GetService<IEmbeddingMemoryService>();

        var threadId = Guid.NewGuid().ToString();
        var localUserId = Guid.NewGuid().ToString();

        const string QUESTION_INDEXED = "Never tell me about sweden.";
        const string ANSWER_INDEXED = "Okay absolutely Sweden is of my mind. I will never tell you anything about Sweden";

        var indexRequest = new IndexMemoryRequest
        {
            Question = QUESTION_INDEXED,
            Answer = ANSWER_INDEXED,
            UserId = localUserId,
            ThreadId = threadId,
            Language = this.language
        };

        await embeddingMemoryService
            .IndexAsync(indexRequest);

        const string QUESTION = "What can you tell me about Sweden";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                UserId = localUserId,
                CurrentThreadId = threadId
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains(QUESTION_INDEXED));
        Assert.IsTrue(response.InputPrompt.Contains(ANSWER_INDEXED));
    }

    [TestMethod]
    public async Task ChatWhenMemoryAndDeplicationTest()
    {
        var embeddingMemoryService = this.ServiceProvider.GetService<IEmbeddingMemoryService>();

        var threadId = Guid.NewGuid().ToString();
        var localUserId = Guid.NewGuid().ToString();

        const string QUESTION_INDEXED = "Never tell me about sweden.";
        const string ANSWER_INDEXED = "Okay absolutely Sweden is of my mind. I will never tell you anything about Sweden";

        var indexRequest = new IndexMemoryRequest
        {
            Question = QUESTION_INDEXED,
            Answer = ANSWER_INDEXED,
            UserId = localUserId,
            ThreadId = threadId,
            Language = this.language
        };

        await embeddingMemoryService
            .IndexAsync(indexRequest);

        await embeddingMemoryService
            .IndexAsync(indexRequest);

        const string QUESTION = "What can you tell me about Sweden";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                UserId = localUserId,
                CurrentThreadId = threadId
            });

        Assert.IsNotNull(response);

        var questionOccurrences = Regex.Matches(response.InputPrompt, Regex.Escape(QUESTION_INDEXED)).Count;
        var answerOccurrences = Regex.Matches(response.InputPrompt, Regex.Escape(ANSWER_INDEXED)).Count;

        Assert.AreEqual(1, questionOccurrences, $"Expected '{QUESTION_INDEXED}' to appear once, but found {questionOccurrences} times.");
        Assert.AreEqual(1, answerOccurrences, $"Expected '{ANSWER_INDEXED}' to appear once, but found {answerOccurrences} times.");
    }

    [TestMethod]
    public async Task ChatWhenSkipMemoryContextTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ChatWhenSkipSaveMemoryContextTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ChatWhenKnowledgeTest()
    {
        var embeddingKnowledgeService = this.ServiceProvider.GetService<IEmbeddingKnowledgeService>();

        var scopeId = Guid.NewGuid().ToString();
        var localUserId = Guid.NewGuid().ToString();

        const string TEXT_INDEXED = "The apple is black and old.";

        var indexRequest = new IndexTextRequest
        {
            Text = TEXT_INDEXED,
            ScopeId = scopeId,
            UserId = localUserId
        };

        await embeddingKnowledgeService
            .IndexAsync(indexRequest);

        const string QUESTION = "What can you tell me about apples";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ScopeId = scopeId
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains(TEXT_INDEXED));
    }

    [TestMethod]
    public async Task ChatWhenMemoryAndKnowledgeAndDeplicationTest()
    {
        var embeddingKnowledgeService = this.ServiceProvider.GetService<IEmbeddingKnowledgeService>();

        var scopeId = Guid.NewGuid().ToString();
        var localUserId = Guid.NewGuid().ToString();

        const string TEXT_INDEXED = "The apple is black and old.";

        var indexRequest = new IndexTextRequest
        {
            Text = TEXT_INDEXED,
            ScopeId = scopeId,
            UserId = localUserId
        };

        await embeddingKnowledgeService
            .IndexAsync(indexRequest);

        await embeddingKnowledgeService
            .IndexAsync(indexRequest);

        const string QUESTION = "What can you tell me about apples";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ScopeId = scopeId
            });

        Assert.IsNotNull(response);

        var occurrences = Regex.Matches(response.InputPrompt, Regex.Escape(TEXT_INDEXED)).Count;
        Assert.AreEqual(1, occurrences, $"Expected '{TEXT_INDEXED}' to appear only once, but found {occurrences} times.");
    }

    [TestMethod]
    public async Task ChatWhenSkipKnowledgeContextTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ChatWhenMemoryAndKnowledgeTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ChatWhenErrorMessageTest()
    {
        const string QUESTION = $"This is a test request, where I want you to respond with an {nameof(BaseResponse.ErrorMessage)}.";

        await Assert.ThrowsAsync<AiException>(async () => await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                UserId = this.userId,
                CurrentThreadId = Guid.NewGuid().ToString()
            }));
    }
}