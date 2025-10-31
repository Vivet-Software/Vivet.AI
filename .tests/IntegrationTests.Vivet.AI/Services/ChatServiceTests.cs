using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Models.Enums;
using Vivet.AI.Services;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.Blobs.Data;
using Vivet.AI.Services.Models.MimeTypes;
using Vivet.AI.Services.Models.Plugins.Contexts;
using Vivet.AI.Services.Requests.Chat;
using Vivet.AI.Services.Requests.Chat.Models.Plugins.Contexts;
using Vivet.AI.Services.Requests.Embedding.Knowledge;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;
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
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = this.userId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            }, memoryResponse =>
            {
                try
                {
                    Assert.IsNotNull(memoryResponse);
                    Assert.IsNotNull(memoryResponse.Result);
                    Assert.IsNotNull(memoryResponse.Result.TokenUsage);
                    Assert.AreEqual(2, memoryResponse.Result.TotalEmbeddings);
                    Assert.IsTrue(memoryResponse.Result.TotalEmbeddingsSize > 0);

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
    public async Task ChatWhenBlobsTest()
    {
        const string BASE64 = "/9j/4AAQSkZJRgABAQEASABIAAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAEAAAAAAAD/7AARRHVja3kAAQAEAAAAWgAA/+EDgmh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8APD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4NCjx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4NCgk8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPg0KCQk8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIgeG1wTU06T3JpZ2luYWxEb2N1bWVudElEPSJ4bXAuZGlkOjAxODAxMTc0MDcyMDY4MTE4QTZERjJGNUE3NDM0RDNEIiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjdFODQyQUYwNkQ1QjExRTRCMDA0REFDNDU5NzQxRTc4IiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjdFODQyQUVGNkQ1QjExRTRCMDA0REFDNDU5NzQxRTc4IiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCBDUzYgKE1hY2ludG9zaCkiPg0KCQkJPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6MDE4MDExNzQwNzIwNjgxMThBNkRGMkY1QTc0MzREM0QiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6MDE4MDExNzQwNzIwNjgxMThBNkRGMkY1QTc0MzRE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";
        const string SYSTEM_MESSAGE = "You are an expert in meteorology.";
        const string QUESTION = "Is it always summer in Denmark?";

        var onMemoryIndexedTask = new TaskCompletionSource<bool>();

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                SystemMessage = SYSTEM_MESSAGE,
                Question = QUESTION,
                Blobs = new List<BaseBlobMetadata>
                {
                    new ImageBlobMetadata
                    {
                        Data = new BlobDataBase64
                        {
                            Base64 = BASE64
                        },
                        MimeType = ImageMimeType.Jpg
                    }
                },
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = this.userId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            }, memoryResponse =>
            {
                Assert.IsNotNull(memoryResponse);
                Assert.IsNotNull(memoryResponse.Result);
                Assert.IsNotNull(memoryResponse.Result.TokenUsage);
                Assert.AreEqual(3, memoryResponse.Result.TotalEmbeddings);
                Assert.IsTrue(memoryResponse.Result.TotalEmbeddingsSize > 0);

                onMemoryIndexedTask
                    .SetResult(true);

                return Task.CompletedTask;
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains("[ImageContent]"));

        await onMemoryIndexedTask.Task;
    }

    [TestMethod]
    public async Task ChatWhenJsonResponseTest()
    {
        const string SYSTEM_MESSAGE = "You are an expert in meteorology.";
        const string QUESTION = "Is it always summer in Denmark?";

        var response = await this.ChatService
            .ChatAsync<JsonClass>(new ChatRequest
            {
                SystemMessage = SYSTEM_MESSAGE,
                Question = QUESTION,
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = this.userId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            }, async memoryResponse =>
            {
                await Task.CompletedTask;

                Assert.IsNotNull(memoryResponse);
                Assert.IsNotNull(memoryResponse.Result);
                Assert.IsNotNull(memoryResponse.Result.TokenUsage);
                Assert.AreEqual(2, memoryResponse.Result.TotalEmbeddings);
                Assert.IsTrue(memoryResponse.Result.TotalEmbeddingsSize > 0);
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
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = this.userId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            }, async memoryResponse =>
            {
                await Task.CompletedTask;

                Assert.IsNotNull(memoryResponse);
                Assert.IsNotNull(memoryResponse.Result);
                Assert.IsNotNull(memoryResponse.Result.TokenUsage);
                Assert.AreEqual(2, memoryResponse.Result.TotalEmbeddings);
                Assert.IsTrue(memoryResponse.Result.TotalEmbeddingsSize > 0);
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
            Plugins =
            {
                Context =
                {
                    Memory = new ChatMemoryContext
                    {
                        UserId = this.userId,
                        CurrentThreadId = Guid.NewGuid()
                    }
                }
            }
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
    public async Task ChatWhenCustomPluginTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ChatWhenMemoryTest()
    {
        var embeddingMemoryService = this.ServiceProvider.GetService<IEmbeddingMemoryService>();

        var threadId = Guid.NewGuid();
        var localUserId = Guid.NewGuid();

        const string QUESTION_INDEXED = "Tell me about Ceasar.";
        const string ANSWER_INDEXED = "Ceasar was a dictator of rome.";

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

        const string QUESTION = "Yesterday you mentioned that when Ceasar was emporer, can you recall that";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Knowledge =
                        {
                            EnableKnowledgePlugin = false
                        },
                        WebSearch =
                        {
                            EnableWebSearchPlugin = false   
                        }
                    }
                },
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = localUserId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionResultContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[MEMORY]"));
        Assert.IsTrue(response.InputPrompt.Contains(QUESTION_INDEXED));
        Assert.IsTrue(response.InputPrompt.Contains(ANSWER_INDEXED));
        Assert.IsNotEmpty(response.FunctionCalls);
    }

    [TestMethod]
    public async Task ChatWhenMemoryAndDeplicationTest()
    {
        var embeddingMemoryService = this.ServiceProvider.GetService<IEmbeddingMemoryService>();

        var threadId = Guid.NewGuid();
        var localUserId = Guid.NewGuid();

        const string QUESTION_INDEXED = "Tell me about Ceasar.";
        const string ANSWER_INDEXED = "Ceasar was a dictator of rome.";

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

        const string QUESTION = "Yesterday you mentioned that when Ceasar was emporer, can you recall that";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = localUserId,
                            CurrentThreadId = Guid.NewGuid()
                        }   
                    }
                }
            });

        Assert.IsNotNull(response);

        var questionOccurrences = Regex.Matches(response.InputPrompt, Regex.Escape(QUESTION_INDEXED)).Count;
        var answerOccurrences = Regex.Matches(response.InputPrompt, Regex.Escape(ANSWER_INDEXED)).Count;

        Assert.AreEqual(1, questionOccurrences, $"Expected '{QUESTION_INDEXED}' to appear once, but found {questionOccurrences} times.");
        Assert.AreEqual(1, answerOccurrences, $"Expected '{ANSWER_INDEXED}' to appear once, but found {answerOccurrences} times.");
    }

    [TestMethod]
    public async Task ChatWhenEnableMemoryPluginIsFalseTest()
    {
        var embeddingMemoryService = this.ServiceProvider.GetService<IEmbeddingMemoryService>();

        var threadId = Guid.NewGuid();
        var localUserId = Guid.NewGuid();

        const string QUESTION_INDEXED = "Tell me about Ceasar.";
        const string ANSWER_INDEXED = "Ceasar was a dictator of rome.";

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

        const string QUESTION = "Yesterday you mentioned that when Ceasar was emporer, can you recall that";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Memory =
                        {
                            EnableMemoryPlugin = false
                        },
                        Knowledge =
                        {
                            EnableKnowledgePlugin = false
                        },
                        WebSearch =
                        {
                            EnableWebSearchPlugin = false
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsFalse(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsFalse(response.InputPrompt.Contains("[FunctionResultContent]"));
        Assert.IsFalse(response.InputPrompt.Contains("[MEMORY]"));
    }

    [TestMethod]
    public async Task ChatWhenMemoryWhenConfigOverridesTest()
    {
        var embeddingMemoryService = this.ServiceProvider.GetService<IEmbeddingMemoryService>();

        var threadId = Guid.NewGuid();
        var localUserId = Guid.NewGuid();

        const string QUESTION_INDEXED = "Tell me about Ceasar.";
        const string ANSWER_INDEXED = "Ceasar was a dictator of rome.";

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

        const string QUESTION = "Yesterday you mentioned that when Ceasar was emporer, can you recall that";

        var request = new ChatRequest
        {
            Question = QUESTION,
            ConfigOverrides =
            {
                Plugins =
                {
                    Memory =
                    {
                        Search =
                        {
                            UseQueryDeduplication = true,
                            ContextQueryLimit = 2,
                            RetentionInDays = 30,
                            CounterpartContextQueryLimit = 2,
                            Scoring =
                            {
                                DeduplicationMatchScoreThreshold = 0.7,
                                MatchScoreThreshold = 0.92,
                                RecencyBoostMax = 0.3,
                                RecencyDecayStrategy = RecencyDecayStrategy.Linear,
                                ThreadMatchBoost = 0.2,
                                RecencySigmoidSteepness = 0.2
                            }
                        }
                    },
                    Knowledge =
                    {
                        EnableKnowledgePlugin = false
                    },
                    WebSearch =
                    {
                        EnableWebSearchPlugin = false
                    }
                }
            },
            Plugins =
            {
                Context =
                {
                    Memory = new ChatMemoryContext
                    {
                        UserId = localUserId,
                        CurrentThreadId = Guid.NewGuid()
                    }
                }
            }
        };

        var response = await this.ChatService
            .ChatAsync(request);

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionResultContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[MEMORY]"));
        Assert.IsTrue(response.InputPrompt.Contains(QUESTION_INDEXED));
        Assert.IsTrue(response.InputPrompt.Contains(ANSWER_INDEXED));
        Assert.IsNotEmpty(response.FunctionCalls);

        var configOverride = (MemorySearchConfigOverrides)response.FunctionCalls.First().Arguments.First(x => x.Key == "configOverrides").Value;
        Assert.IsNotNull(configOverride);
        Assert.AreEqual(configOverride.UseQueryDeduplication, request.ConfigOverrides.Plugins.Memory.Search.UseQueryDeduplication);
        Assert.AreEqual(configOverride.ContextQueryLimit, request.ConfigOverrides.Plugins.Memory.Search.ContextQueryLimit);
        Assert.AreEqual(configOverride.RetentionInDays, request.ConfigOverrides.Plugins.Memory.Search.RetentionInDays);
        Assert.AreEqual(configOverride.CounterpartContextQueryLimit, request.ConfigOverrides.Plugins.Memory.Search.CounterpartContextQueryLimit);
        Assert.AreEqual(configOverride.Scoring.DeduplicationMatchScoreThreshold, request.ConfigOverrides.Plugins.Memory.Search.Scoring.DeduplicationMatchScoreThreshold);
        Assert.AreEqual(configOverride.Scoring.MatchScoreThreshold, request.ConfigOverrides.Plugins.Memory.Search.Scoring.MatchScoreThreshold);
        Assert.AreEqual(configOverride.Scoring.RecencyBoostMax, request.ConfigOverrides.Plugins.Memory.Search.Scoring.RecencyBoostMax);
        Assert.AreEqual(configOverride.Scoring.RecencyDecayStrategy, request.ConfigOverrides.Plugins.Memory.Search.Scoring.RecencyDecayStrategy);
        Assert.AreEqual(configOverride.Scoring.ThreadMatchBoost, request.ConfigOverrides.Plugins.Memory.Search.Scoring.ThreadMatchBoost);
        Assert.AreEqual(configOverride.Scoring.RecencySigmoidSteepness, request.ConfigOverrides.Plugins.Memory.Search.Scoring.RecencySigmoidSteepness);
    }

    [TestMethod]
    public async Task ChatWhenKnowledgeTest()
    {
        var embeddingKnowledgeService = this.ServiceProvider.GetService<IEmbeddingKnowledgeService>();

        var scopeId = Guid.NewGuid();

        const string TEXT_INDEXED = "We had 100 orders in 2022.";

        var indexRequest = new IndexTextRequest
        {
            Text = TEXT_INDEXED,
            ScopeId = scopeId
        };

        await embeddingKnowledgeService
            .IndexAsync(indexRequest);

        const string QUESTION = "Can you look up how many orders we had in 2022?";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Memory =
                        {
                            EnableMemoryPlugin = false
                        },
                        WebSearch = 
                        {
                            EnableWebSearchPlugin = false
                        }
                    }
                },
                Plugins =
                {
                    Context =
                    {
                        Knowledge = new KnowledgeContext
                        {
                            ScopeId = scopeId
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionResultContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[KNOWLEDGE]"));
        Assert.IsTrue(response.InputPrompt.Contains(TEXT_INDEXED));
        Assert.IsNotEmpty(response.FunctionCalls);
    }

    [TestMethod]
    public async Task ChatWhenEnableKnowledgePluginIsFalseTest()
    {
        var embeddingKnowledgeService = this.ServiceProvider.GetService<IEmbeddingKnowledgeService>();

        var scopeId = Guid.NewGuid();

        const string TEXT_INDEXED = "We had 100 orders last year.";

        var indexRequest = new IndexTextRequest
        {
            Text = TEXT_INDEXED,
            ScopeId = scopeId
        };

        await embeddingKnowledgeService
            .IndexAsync(indexRequest);

        const string QUESTION = "Can you look up how many orders did we have last year?";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Knowledge = 
                        {
                            EnableKnowledgePlugin = false
                        },
                        WebSearch =
                        {
                            EnableWebSearchPlugin = false
                        }
                    }
                },
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = this.userId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsFalse(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsFalse(response.InputPrompt.Contains("[FunctionResultContent]"));
        Assert.IsFalse(response.InputPrompt.Contains("[KNOWLEDGE]"));
    }

    [TestMethod]
    public async Task ChatWhenKnowledgeWhenConfigOverridesTest()
    {
        var embeddingKnowledgeService = this.ServiceProvider.GetService<IEmbeddingKnowledgeService>();

        var scopeId = Guid.NewGuid();

        const string TEXT_INDEXED = "We had 100 orders in 2022.";

        var indexRequest = new IndexTextRequest
        {
            Text = TEXT_INDEXED,
            ScopeId = scopeId
        };

        await embeddingKnowledgeService
            .IndexAsync(indexRequest);

        const string QUESTION = "Can you check how many orders we had in 2022?";

        var request = new ChatRequest
        {
            Question = QUESTION,
            ConfigOverrides =
            {
                Plugins =
                {
                    Memory =
                    {
                        EnableMemoryPlugin = false
                    },
                    Knowledge =
                    {
                        Search =
                        {
                            UseQueryDeduplication = true,
                            ContextQueryLimit = 2,
                            Scoring =
                            {
                                DeduplicationMatchScoreThreshold = 0.7,
                                MatchScoreThreshold = 0.92,
                                RecencyBoostMax = 0.3,
                                RecencyDecayStrategy = RecencyDecayStrategy.Linear,
                                RecencySigmoidSteepness = 0.2
                            }
                        }
                    },
                    WebSearch =
                    {
                        EnableWebSearchPlugin = false
                    }
                }
            },
            Plugins =
            {
                Context =
                {
                    Knowledge = new KnowledgeContext
                    {
                        ScopeId = scopeId
                    }
                }
            }
        };

        var response = await this.ChatService
            .ChatAsync(request);

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionResultContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[KNOWLEDGE]"));
        Assert.IsTrue(response.InputPrompt.Contains(TEXT_INDEXED));
        Assert.IsNotEmpty(response.FunctionCalls);

        var configOverride = (KnowledgeSearchConfigOverrides)response.FunctionCalls.First().Arguments.First(x => x.Key == "configOverrides").Value;
        Assert.IsNotNull(configOverride);
        Assert.AreEqual(configOverride.UseQueryDeduplication, request.ConfigOverrides.Plugins.Knowledge.Search.UseQueryDeduplication);
        Assert.AreEqual(configOverride.ContextQueryLimit, request.ConfigOverrides.Plugins.Knowledge.Search.ContextQueryLimit);
        Assert.AreEqual(configOverride.Scoring.DeduplicationMatchScoreThreshold, request.ConfigOverrides.Plugins.Knowledge.Search.Scoring.DeduplicationMatchScoreThreshold);
        Assert.AreEqual(configOverride.Scoring.MatchScoreThreshold, request.ConfigOverrides.Plugins.Knowledge.Search.Scoring.MatchScoreThreshold);
        Assert.AreEqual(configOverride.Scoring.RecencyBoostMax, request.ConfigOverrides.Plugins.Knowledge.Search.Scoring.RecencyBoostMax);
        Assert.AreEqual(configOverride.Scoring.RecencyDecayStrategy, request.ConfigOverrides.Plugins.Knowledge.Search.Scoring.RecencyDecayStrategy);
        Assert.AreEqual(configOverride.Scoring.RecencySigmoidSteepness, request.ConfigOverrides.Plugins.Knowledge.Search.Scoring.RecencySigmoidSteepness);
    }

    [TestMethod]
    public async Task ChatWhenWebSearchTest()
    {
        const string QUESTION = "What's the latest with .NET?";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Memory = 
                        {
                            EnableMemoryPlugin = false
                        },
                        Knowledge = 
                        {
                            EnableKnowledgePlugin = false
                        }
                    }
                },
                Plugins =
                {
                    Context =
                    {
                        WebSearch = new WebSearchContext
                        {
                            Site = "microsoft.com",
                            Limit = 3
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsTrue(response.InputPrompt.Contains("[FunctionResultContent]"));
    }

    [TestMethod]
    public async Task ChatWhenWebSearchWhenEnableWebSearchPluginIsFalseTest()
    {
        const string QUESTION = "What's the latest with .NET?";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Memory =
                        {
                            EnableMemoryPlugin = false
                        },
                        Knowledge =
                        {
                            EnableKnowledgePlugin = false
                        },
                        WebSearch =
                        {
                            EnableWebSearchPlugin = false
                        }
                    }
                },
                Plugins =
                {
                    Context =
                    {
                        WebSearch = new WebSearchContext
                        {
                            Site = "microsoft.com",
                            Limit = 3
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsFalse(response.InputPrompt.Contains("[FunctionCallContent]"));
        Assert.IsFalse(response.InputPrompt.Contains("[FunctionResultContent]"));
    }

    [TestMethod]
    public async Task ChatWhenErrorMessageTest()
    {
        const string QUESTION = $"This is a test request, where I want you to respond with an {nameof(BaseResponse.ErrorMessage)}.";

        var response = await this.ChatService
            .ChatAsync(new ChatRequest
            {
                Question = QUESTION,
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = this.userId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Exception);
        Assert.AreEqual(typeof(AiException), response.Exception.GetType());
    }

    [TestMethod]
    public async Task ChatWhenOnMemoryIndexedIsFaultedTest()
    {
        var chatOptions = this.ServiceProvider.GetRequiredService<ChatOptions>();
        var chatCompletionService = this.ServiceProvider.GetRequiredKeyedService<IChatCompletionService>(ServiceIds.CHAT_SERVICE_ID);
        var kernelBuilder = Kernel.CreateBuilder();
        var promptExecutionSettings = new PromptExecutionSettings();

        var mockEmbeddingMemoryService = new Mock<IEmbeddingMemoryService>();
        mockEmbeddingMemoryService
            .Setup(s => s.IndexAsync(It.IsAny<IndexMemoryRequest<string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Indexing failed"));

        var chatService = new ChatService(chatOptions, chatCompletionService, kernelBuilder, this.ServiceProvider, promptExecutionSettings, mockEmbeddingMemoryService.Object);

        const string SYSTEM_MESSAGE = "You are an expert in meteorology.";
        const string QUESTION = "Is it always summer in Denmark?";

        var onMemoryIndexedTask = new TaskCompletionSource<bool>();

        var response = await chatService
            .ChatAsync(new ChatRequest
            {
                SystemMessage = SYSTEM_MESSAGE,
                Question = QUESTION,
                Plugins =
                {
                    Context =
                    {
                        Memory = new ChatMemoryContext
                        {
                            UserId = this.userId,
                            CurrentThreadId = Guid.NewGuid()
                        }
                    }
                }
            }, memoryResponse =>
            {
                Assert.IsNotNull(memoryResponse.Exception);

                onMemoryIndexedTask
                    .SetException(memoryResponse.Exception);

                return Task.CompletedTask;
            });

        Assert.IsNotNull(response);

        await onMemoryIndexedTask.Task
            .ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    Assert.IsNotNull(x.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
    }
}