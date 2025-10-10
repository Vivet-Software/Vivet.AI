using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Responses.Embeddings;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;
using Vivet.AI.Services.Responses.Embeddings.Memory.Models;

namespace UnitTests.Vivet.AI.Services.Helpers;

[TestClass]
public class ContextDeduplicatorTests
{
    [TestMethod]
    public void DeduplicateMemoryResultsTest()
    {
        const string QUESTION = "Q";
        const string ANSWER = "A";

        var question = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION,
                CounterpartContext = [ANSWER]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question]);
        Assert.AreEqual(1, results.Length);

        Assert.IsTrue(results[0].Result.IsQuestion);
        Assert.AreEqual(question.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(question.Result.CounterpartContext[0], results[0].Result.CounterpartContext[0]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenThrowsArgumentNullExceptionTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => ContextDeduplicator.DeduplicateMemoryResults(null));
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenQuestionOnlyTest()
    {
        const string QUESTION = "Q";
        const string ANSWER = "A";

        var question = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION,
                CounterpartContext = [ANSWER]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question]);
        Assert.AreEqual(1, results.Length);

        Assert.IsTrue(results[0].Result.IsQuestion);
        Assert.AreEqual(question.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(question.Result.CounterpartContext[0], results[0].Result.CounterpartContext[0]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenAnswerOnlyTest()
    {
        const string QUESTION = "Q";
        const string ANSWER = "A";

        var answer = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER,
                CounterpartContext = [QUESTION]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([answer]);
        Assert.AreEqual(1, results.Length);

        Assert.IsTrue(results[0].Result.IsAnswer);
        Assert.AreEqual(answer.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(answer.Result.CounterpartContext[0], results[0].Result.CounterpartContext[0]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenDuplicateTest()
    {
        const string QUESTION = "Q";
        const string ANSWER = "A";

        var question = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION,
                CounterpartContext = [ANSWER]
            }
        };

        var answer = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER,
                CounterpartContext = [QUESTION]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question, answer]);
        Assert.AreEqual(1, results.Length);

        Assert.IsTrue(results[0].Result.IsQuestion);
        Assert.AreEqual(question.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(answer.Result.FullContext, results[0].Result.CounterpartContext[0]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenDuplicatesPairsTest()
    {
        const string QUESTION_1 = "Q1";
        const string ANSWER_1 = "A1";

        const string QUESTION_2 = "Q2";
        const string ANSWER_2 = "A2";

        var question1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION_1,
                CounterpartContext = [ANSWER_1]
            }
        };

        var answer1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER_1,
                CounterpartContext = [QUESTION_1]
            }
        };

        var question2 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION_2,
                CounterpartContext = [ANSWER_2]
            }
        };

        var answer2 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER_2,
                CounterpartContext = [QUESTION_2]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question1, answer1, question2, answer2]);
        Assert.AreEqual(2, results.Length);

        Assert.IsTrue(results[0].Result.IsQuestion);
        Assert.AreEqual(question1.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(answer1.Result.FullContext, results[0].Result.CounterpartContext[0]);

        Assert.IsTrue(results[1].Result.IsQuestion);
        Assert.AreEqual(question2.Result.FullContext, results[1].Result.FullContext);
        Assert.AreEqual(answer2.Result.FullContext, results[1].Result.CounterpartContext[0]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenDuplicateQuestionsTest()
    {
        const string QUESTION_1 = "Q1";
        const string ANSWER_1 = "A1";
        const string ANSWER_2 = "A2";

        var question1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION_1,
                CounterpartContext = [ANSWER_1]
            }
        };

        var question2 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION_1,
                CounterpartContext = [ANSWER_2]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question1, question2]);
        Assert.AreEqual(1, results.Length);

        Assert.IsTrue(results[0].Result.IsQuestion);
        Assert.AreEqual(question1.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(question1.Result.CounterpartContext[0], results[0].Result.CounterpartContext[0]);
        Assert.AreEqual(question2.Result.CounterpartContext[0], results[0].Result.CounterpartContext[1]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenDuplicateQuestionsTakesNewestTest()
    {
        const string QUESTION = "Q";

        var question1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            }
        };

        var question2 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION,
                CreatedAt = DateTime.UtcNow
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question1, question2]);

        Assert.AreEqual(1, results.Length);
        Assert.IsTrue(results[0].Result.IsQuestion);
        Assert.AreEqual(question2.Result.FullContext, results[0].Result.FullContext);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenDuplicateAnswersTest()
    {
        const string QUESTION_1 = "Q1";
        const string QUESTION_2 = "Q2";
        const string ANSWER_1 = "A1";

        var answer1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER_1,
                CounterpartContext = [QUESTION_1]
            }
        };

        var answer2 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER_1,
                CounterpartContext = [QUESTION_2]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([answer1, answer2]);
        Assert.AreEqual(1, results.Length);

        Assert.IsTrue(results[0].Result.IsAnswer);
        Assert.AreEqual(answer1.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(answer1.Result.CounterpartContext[0], results[0].Result.CounterpartContext[0]);
        Assert.AreEqual(answer2.Result.CounterpartContext[0], results[0].Result.CounterpartContext[1]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenDuplicateAnswersTakesNewestTest()
    {
        const string ANSWER = "Q";

        var answer1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            }
        };

        var answer2 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER,
                CreatedAt = DateTime.UtcNow
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([answer1, answer2]);

        Assert.AreEqual(1, results.Length);
        Assert.IsTrue(results[0].Result.IsAnswer);
        Assert.AreEqual(answer2.Result.FullContext, results[0].Result.FullContext);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenDuplicateQuestionsAndAnswersTest()
    {
        const string QUESTION_1 = "Q1";
        const string QUESTION_2 = "Q2";
        const string ANSWER_1 = "A1";
        const string ANSWER_2 = "A2";

        var question1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION_1,
                CounterpartContext = [ANSWER_1, ANSWER_2]
            }
        };

        var question2 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = QUESTION_2,
                CounterpartContext = [ANSWER_1]
            }
        };

        var answer1 = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = ANSWER_2,
                CounterpartContext = [QUESTION_1]
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question1, question2, answer1]);
        Assert.AreEqual(3, results.Length);

        Assert.IsTrue(results[0].Result.IsQuestion);
        Assert.AreEqual(question1.Result.FullContext, results[0].Result.FullContext);
        Assert.AreEqual(question1.Result.CounterpartContext[0], results[0].Result.CounterpartContext[0]);
        Assert.AreEqual(question1.Result.CounterpartContext[1], results[0].Result.CounterpartContext[1]);

        Assert.IsTrue(results[1].Result.IsQuestion);
        Assert.AreEqual(question2.Result.FullContext, results[1].Result.FullContext);
        Assert.AreEqual(question2.Result.CounterpartContext[0], results[1].Result.CounterpartContext[0]);

        Assert.IsTrue(results[2].Result.IsAnswer);
        Assert.AreEqual(answer1.Result.FullContext, results[2].Result.FullContext);
        Assert.AreEqual(answer1.Result.CounterpartContext[0], results[2].Result.CounterpartContext[0]);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenSameBlobTest()
    {
        var now = DateTime.UtcNow;
        var questions = new[]
        {
            new SearchMemoryResult
            {
                Result = new MemoryResult
                {
                    FullContext = "Q1",
                    IsQuestion = true,
                    CreatedAt = now,
                    Blob = new BlobResponse
                    {
                        Hash = "h1"
                    }
                }
            },
            new SearchMemoryResult
            {
                Result = new MemoryResult
                {
                    FullContext = "Q1 again",
                    IsQuestion = true,
                    CreatedAt = now.AddMinutes(1),
                    Blob = new BlobResponse
                    {
                        Hash = "h1"
                    }
                }
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults(questions);

        Assert.AreEqual(1, results.Length);
        Assert.AreEqual("h1", results[0].Result.Blob.Hash);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenSimilarityTest()
    {
        var now = DateTime.UtcNow;
        var questions = new[]
        {
            new SearchMemoryResult
            {
                Result = new MemoryResult
                {
                    FullContext = "What is AI?",
                    IsQuestion = true,
                    CreatedAt = now
                }
            },
            new SearchMemoryResult
            {
                Result = new MemoryResult
                {
                    FullContext = "What is A.I.?",
                    IsQuestion = true,
                    CreatedAt = now.AddMinutes(1)
                }
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults(questions);

        Assert.AreEqual(1, results.Length);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhenSimilarityAndLowerMatchThreshold()
    {
        var now = DateTime.UtcNow;
        var questions = new[]
        {
            new SearchMemoryResult
            {
                Result = new MemoryResult
                {
                    FullContext = "What is AI?",
                    IsQuestion = true,
                    CreatedAt = now
                }
            },
            new SearchMemoryResult
            {
                Result = new MemoryResult
                {
                    FullContext = "What is Artificial Intelligence?",
                    IsQuestion = true,
                    CreatedAt = now.AddMinutes(1)
                }
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults(questions, 0.50);
        Assert.AreEqual(1, results.Length);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsCaseInsensitiveTest()
    {
        var now = DateTime.UtcNow;

        var question = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = "Never tell me about Sweden.",
                CreatedAt = now
            }
        };

        var questionLowercase = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = "never tell me about sweden.",
                CreatedAt = now.AddMinutes(1)
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question, questionLowercase]);

        Assert.AreEqual(1, results.Length);
        Assert.AreEqual(questionLowercase.Result.FullContext, results[0].Result.FullContext);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWhitespaceNormalizedTest()
    {
        var now = DateTime.UtcNow;

        var question = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = "What is AI?",
                CreatedAt = now
            }
        };

        var questionWhitespace = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = "What is AI? ",
                CreatedAt = now.AddMinutes(1)
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question, questionWhitespace]);

        Assert.AreEqual(1, results.Length);
        Assert.AreEqual(questionWhitespace.Result.FullContext, results[0].Result.FullContext);
    }

    [TestMethod]
    public void DeduplicateMemoryResultsWithNullOrEmptyCounterpartContextTest()
    {
        var now = DateTime.UtcNow;

        var question = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsQuestion = true,
                FullContext = "Q1",
                CounterpartContext = null,
                CreatedAt = now
            }
        };

        var answer = new SearchMemoryResult
        {
            Result = new MemoryResult
            {
                IsAnswer = true,
                FullContext = "A1",
                CounterpartContext = [],
                CreatedAt = now
            }
        };

        var results = ContextDeduplicator.DeduplicateMemoryResults([question, answer]);

        Assert.AreEqual(2, results.Length);
        Assert.IsTrue(results.Any(x => x.Result.IsQuestion));
        Assert.IsTrue(results.Any(x => x.Result.IsAnswer));
    }


    [TestMethod]
    public void DeduplicateKnowledgeResultsTest()
    {
        var knowledges = new[]
        {
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "Document A",
                    CreatedAt = DateTime.UtcNow
                }
            },
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "Totally unrelated content",
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        var results = ContextDeduplicator.DeduplicateKnowledgeResults(knowledges);

        Assert.AreEqual(2, results.Length);
    }

    [TestMethod]
    public void DeduplicateKnowledgeResultsWhenThrowsArgumentNullExceptionTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => ContextDeduplicator.DeduplicateKnowledgeResults(null));
    }

    [TestMethod]
    public void DeduplicateKnowledgeResultsWhenSameBlobTest()
    {
        var now = DateTime.UtcNow;
        var knowledges = new[]
        {
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "Doc1",
                    CreatedAt = now,
                    Blob = new BlobResponse
                    {
                        Hash = "hash123"
                    }
                }
            },
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "Doc1 newer",
                    CreatedAt = now.AddMinutes(5),
                    Blob = new BlobResponse
                    {
                        Hash = "hash123"
                    }
                }
            }
        };

        var results = ContextDeduplicator.DeduplicateKnowledgeResults(knowledges);

        Assert.AreEqual(1, results.Length);
        Assert.AreEqual("hash123", results[0].Result.Blob.Hash);
    }

    [TestMethod]
    public void DeduplicateKnowledgeResultsWhenHighSimilarityTest()
    {
        var now = DateTime.UtcNow;
        var knowledges = new[]
        {
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "AI is awesome",
                    CreatedAt = now
                }
            },
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "AI is awsome",
                    CreatedAt = now.AddMinutes(1)
                }
            }
        };

        var results = ContextDeduplicator.DeduplicateKnowledgeResults(knowledges);

        Assert.AreEqual(1, results.Length);
    }

    [TestMethod]
    public void DeduplicateKnowledgeResultsWhenLowerMatchScoreThresholdTest()
    {
        var now = DateTime.UtcNow;
        var knowledges = new[]
        {
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "AI is awesome",
                    CreatedAt = now
                }
            },
            new SearchKnowledgeResult
            {
                Result = new KnowledgeResult
                {
                    FullContext = "AI is so awsome",
                    CreatedAt = now.AddMinutes(1)
                }
            }
        };

        var results = ContextDeduplicator.DeduplicateKnowledgeResults(knowledges, 0.85);

        Assert.AreEqual(1, results.Length);
    }
}