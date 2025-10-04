using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;
using Vivet.AI.Data.Stores;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Helpers.Models;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Embedding;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Requests.Embedding.Memory.Models;
using Vivet.AI.Services.Requests.Metadata;
using Vivet.AI.Services.Requests.Summarization;
using Vivet.AI.Services.Responses.Embeddings.Memory;
using Vivet.AI.Services.Responses.Embeddings.Memory.Models;
using Vivet.AI.Services.Responses.Metadata;
using Vivet.AI.Services.Responses.Summarization;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

/// <inheritdoc cref="IEmbeddingMemoryService"/>
public class EmbeddingMemoryService(EmbeddingOptions options, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, MemoryVectorStore vectorStore, IMetadataService metadataService = null, ISummarizationService summarizationService = null)
    : BaseEmbeddingService(options, embeddingGenerator, metadataService), IEmbeddingMemoryService
{
    private readonly MemoryVectorStore vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
    private readonly EmbeddingOptions.MemoryOptions memoryOptions = options.Memory ?? throw new ArgumentNullException(nameof(options.Memory));

    /// <inheritdoc />
    public virtual async Task<IndexMemoryResponse> IndexAsync<T>(IndexMemoryRequest<T> request, CancellationToken cancellationToken = default) 
        where T : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request
            .Validate();

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        var summarizationResposne = await this.SummarizeQuestionAndAnswer(request.Question, request.Answer, request.ConfigOverrides, cancellationToken).ConfigureAwait(false);

        var question = summarizationResposne?.QuestionSummarized ?? request.Question;
        var answer = summarizationResposne?.AnswerSummarized ?? JsonConvert.SerializeObject(request.Answer, Formatting.None, Settings.SerializerSettings);

        var questionTextChunks = TextChunking.GetTextChunks(question, this.memoryOptions.TextChunking.MinTokens, this.memoryOptions.TextChunking.MaxTokens);
        var answerTextChunks = TextChunking.GetTextChunks(answer, this.memoryOptions.TextChunking.MinTokens, this.memoryOptions.TextChunking.MaxTokens);

        var questionEmbeddingsTask = this.GenerateEmbeddings(questionTextChunks.Select(x => x.Text).ToArray(), request.ConfigOverrides, cancellationToken);
        var answerEmbeddingsTask = this.GenerateEmbeddings(answerTextChunks.Select(x => x.Text).ToArray(), request.ConfigOverrides, cancellationToken);

        var questionEmbeddings = await questionEmbeddingsTask.ConfigureAwait(false);
        var answerEmbeddings = await answerEmbeddingsTask.ConfigureAwait(false);

        var questionAnswerId = Guid.NewGuid().ToString();

        var questionMemories = this.GetMemories(request, questionAnswerId, questionTextChunks, questionEmbeddings, answerTextChunks, answerEmbeddings, true);
        var answerMemories = this.GetMemories(request, questionAnswerId, answerTextChunks, answerEmbeddings, questionTextChunks, questionEmbeddings, false);
        var (memories, blobsUsage, metadataUsage) = await this.GetBlobMemories(request, questionAnswerId, cancellationToken).ConfigureAwait(false);

        var embeddings = questionMemories
            .Union(answerMemories)
            .Union(memories)
            .ToArray();

        await this.vectorStore.Collection
            .UpsertAsync(embeddings, cancellationToken)
            .ConfigureAwait(false);

        var tokenUsage = EmbeddingMemoryService.GetTokenUsageOrDefault(questionEmbeddings.Usage, answerEmbeddings.Usage, blobsUsage);

        var totalEmbeddings = embeddings
            .Select(x => x.Content)
            .Sum(x => x.GetUtf8ByteCount());

        stopwatch
            .Stop();

        return new IndexMemoryResponse
        {
            TotalEmbeddings = embeddings.Length,
            TotalEmbeddingsSize = totalEmbeddings,
            ElapsedTime = stopwatch.Elapsed,
            TokenUsage = tokenUsage,
            MetadataTokenUsage = metadataUsage,
            SummarizationTokenUsage = summarizationResposne?.TokenUsage
        };
    }

    /// <inheritdoc />
    public virtual async Task<SearchMemoryResponse> SearchAsync(SearchMemoryRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var vectorSearchOptions = new VectorSearchOptions<Memory>
        {
            Filter = request.Criteria
                .BuildFilter()
        };

        var memories = this.vectorStore.Collection
            .SearchAsync(request.Query, request.Limit, vectorSearchOptions, cancellationToken);

        var results = await memories
            .Select(result =>
            {
                var baseScore = result.Score ?? 0.0;

                var sameThreadScore = this.GetSameThreadScore(result, request.CurrentThreadId);
                var recencyScore = result.Record
                    .GetRecencyScore(this.memoryOptions.Scoring);

                var adjustedScore = baseScore + sameThreadScore + recencyScore;

                return new
                {
                    AdjustedScore = adjustedScore,
                    OriginalScore = baseScore,
                    Result = result
                };
            })
            .Where(x => x.AdjustedScore >= this.options.MatchScoreThreashold)
            .OrderByDescending(x => x.AdjustedScore)
            .Select(x => new SearchMemoryResult
            {
                Score = x.AdjustedScore,
                Result = new MemoryResult(x.Result.Record)
            })
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        stopwatch
            .Stop();

        return new SearchMemoryResponse
        {
            Results = results,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    /// <inheritdoc />
    public virtual async Task<QueryMemoryResponse> QueryAsync(QueryMemoryRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var filter = request.Criteria
            .BuildFilter();

        var retrievalOptions = new FilteredRecordRetrievalOptions<Memory>
        {
            Skip = request.Skip,
            OrderBy = x => x
                .Ascending(y => y.UnixTimestamp)
        };

        var memories = this.vectorStore.Collection
            .GetAsync(filter, request.Limit, retrievalOptions, cancellationToken);

        var memoryResults = await memories
            .Select(x => new QueryMemoryResult
            {
                Result = new MemoryResult(x),
                Size = x.Content.GetUtf8ByteCount()
            })
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        stopwatch
            .Stop();

        return new QueryMemoryResponse
        {
            Results = memoryResults,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request
            .Validate();

        return this.vectorStore.Collection
            .DeleteAsync(request.Ids, cancellationToken);
    }


    private async Task<SummarizationMemoryResponse> SummarizeQuestionAndAnswer<T>(string question, T answer, MemoryConfigOverrides configOverrides, CancellationToken cancellationToken = default)
        where T : class
    {
        if (question == null)
            throw new ArgumentNullException(nameof(question));

        if (answer == null)
            throw new ArgumentNullException(nameof(answer));

        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        var isSummarization =
            summarizationService != null && 
            (
                (configOverrides.Summarization.UseAutomaticSummarization ?? false) || 
                (
                    this.memoryOptions.UseAutomaticSummarization &&
                    configOverrides.Summarization.UseAutomaticSummarization != false
                )
            );

        if (answer is string stringAnswer)
        {
            if (isSummarization)
            {
                return await summarizationService
                    .SummarizeMemoryAsync(new SummarizeMemoryRequest
                    {
                        Question = question,
                        Answer = stringAnswer,
                        ConfigOverrides = 
                        {
                            SummarizationDegree = configOverrides.Summarization.SummarizationDegree
                        }
                    }, cancellationToken)
                    .ConfigureAwait(false);
            }

            return new SummarizationMemoryResponse
            {
                QuestionSummarized = question,
                AnswerSummarized = stringAnswer
            };
        }

        return null;
    }
    private IEnumerable<Memory> GetMemories<T>(IndexMemoryRequest<T> request, string questionAnswerId, TextChunk[] textChunks, GeneratedEmbeddings<Embedding<float>> embeddings, TextChunk[] counterPartTextChunks, GeneratedEmbeddings<Embedding<float>> counterpartEmbeddings, bool isQuestion) 
        where T : class
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        if (questionAnswerId == null) 
            throw new ArgumentNullException(nameof(questionAnswerId));
        
        if (textChunks == null) 
            throw new ArgumentNullException(nameof(textChunks));
        
        if (embeddings == null) 
            throw new ArgumentNullException(nameof(embeddings));
        
        if (counterPartTextChunks == null) 
            throw new ArgumentNullException(nameof(counterPartTextChunks));
        
        if (counterpartEmbeddings == null) 
            throw new ArgumentNullException(nameof(counterpartEmbeddings));

        var userId = request.UserId?.ToString();
        var agentId = request.AgentId?.ToString();
        var scopeId = request.ScopeId?.ToString();
        var threadId = request.ThreadId.ToString();

        var memories = embeddings
            .Select((x, i) =>
            {
                var fullContext = TextChunking.GetTextChunkNeighboringContext(textChunks, i, this.memoryOptions.TextChunking.NeighborContext.ContextWindow, this.memoryOptions.TextChunking.NeighborContext.RestrictToSameParagraph);

                string[] counterpartContext = [];

                if (this.memoryOptions.UseExtendedMemoryContext)
                {
                    counterpartContext = counterpartEmbeddings
                        .Select((y, j) =>
                        {
                            var score = CosineSimilarity.GetMatches(x.Vector.ToArray(), y.Vector.ToArray());

                            if (score >= this.options.MatchScoreThreashold)
                            {
                                var fullContextCounterpart = counterPartTextChunks[j].Text;

                                return (fullContextCounterpart, score);
                            }

                            return (null, 0);
                        })
                        .Where(y => y.fullContextCounterpart != null)
                        .OrderByDescending(y => y.score)
                        .Select(y => y.fullContextCounterpart)
                        .Distinct()
                        .ToArray();
                }

                return new Memory
                {
                    Vector = x.Vector,
                    Content = textChunks[i].Text,
                    FullContext = fullContext,
                    CounterpartContext = counterpartContext,
                    Order = i,
                    Language = request.Language,
                    EmbeddingModel = this.options.Model.Name,
                    UserId = userId,
                    AgentId = agentId,
                    ScopeId = scopeId,
                    ThreadId = threadId,
                    QuestionAnswerId = questionAnswerId,
                    IsQuestion = isQuestion,
                    IsAnswer = !isQuestion
                };
            })
            .ToArray();

        return memories;
    }
    private async Task<(IEnumerable<Memory> Memories, TokenUsage tokenUsage, TokenUsage MetadataTokenUsage)> GetBlobMemories<T>(IndexMemoryRequest<T> request, string questionAnswerId, CancellationToken cancellationToken = default)
        where T : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (questionAnswerId == null)
            throw new ArgumentNullException(nameof(questionAnswerId));

        var userId = request.UserId?.ToString();
        var agentId = request.AgentId?.ToString();
        var scopeId = request.ScopeId?.ToString();
        var threadId = request.ThreadId.ToString();

        TokenUsage tokenUsage = null;
        TokenUsage metadataTokenUsage = null;

        var blobMemoriesTasks = request.Blobs
            .Select(async x =>
            {
                MetadataResponse metadataResponse = null;
                if (x.Metadata != null)
                {
                    metadataResponse = new MetadataResponse
                    {
                        Metadata = x.Metadata
                    };
                }
                else if (this.metadataService != null && ((request.ConfigOverrides.Metadata.UseAutomaticMetadataRetrieval ?? false) || (this.memoryOptions.UseAutomaticMetadataRetrieval && request.ConfigOverrides.Metadata.UseAutomaticMetadataRetrieval != false)))
                {
                    metadataResponse = await this.metadataService
                        .GetAsync(new GetMetadataRequest
                        {
                            Blob = x,
                            ConfigOverrides = 
                            {
                                SummaryMaxWords = request.ConfigOverrides.Metadata.SummaryMaxWords,
                                DescriptionMaxWords = request.ConfigOverrides.Metadata.DescriptionMaxWords
                            }
                        }, cancellationToken)
                        .ConfigureAwait(false);
                }

                if (metadataResponse == null)
                {
                    throw new InvalidOperationException("No metadata available. Either include metadata in the request, or enable automatic metadata retrieval in the configuration or for this request.");
                }

                if (metadataResponse.Exception != null)
                {
                    throw metadataResponse.Exception;
                }

                var blobData = await x
                    .GetBlobData(cancellationToken)
                    .ConfigureAwait(false);

                var embeddings = await this.GenerateEmbeddings([metadataResponse.Metadata.Summary], request.ConfigOverrides, cancellationToken)
                    .ConfigureAwait(false);

                var embedding = embeddings
                    .FirstOrDefault();

                if (embedding == null)
                {
                    throw new NullReferenceException(nameof(embedding));
                }

                var memory = new Memory
                {
                    Vector = embedding.Vector,
                    Content = metadataResponse.Metadata.Summary,
                    FullContext = metadataResponse.Metadata.Description,
                    Order = 0,
                    Language = request.Language,
                    EmbeddingModel = this.options.Model.Name,
                    UserId = userId,
                    AgentId = agentId,
                    ScopeId = scopeId,
                    ThreadId = threadId,
                    QuestionAnswerId = questionAnswerId,
                    IsQuestion = true,
                    BlobBase64 = blobData.Base64,
                    BlobMimeType = blobData.MimeType
                };

                tokenUsage ??= new TokenUsage();

                if (embeddings.Usage != null)
                {
                    tokenUsage += new TokenUsage
                    {
                        InputTokens = embeddings.Usage.InputTokenCount,
                        OutputTokens = embeddings.Usage.OutputTokenCount
                    };
                }

                metadataTokenUsage ??= new TokenUsage();
                metadataTokenUsage += metadataResponse.TokenUsage;

                return memory;
            })
            .ToArray();

        var blobMemories = await Task.WhenAll(blobMemoriesTasks).ConfigureAwait(false);

        return (blobMemories, tokenUsage, metadataTokenUsage);
    }
    private double GetSameThreadScore(VectorSearchResult<Memory> result, Guid? currentThreadId = null)
    {
        if (result == null) 
            throw new ArgumentNullException(nameof(result));

        if (currentThreadId == null)
        {
            return 0.00D;
        }

        if (result.Record.ThreadId == currentThreadId.ToString())
        {
            return this.memoryOptions.Scoring.ThreadMatchBoost;
        }

        return 0.00D;
    }
    
    private static TokenUsage GetTokenUsageOrDefault(UsageDetails questionsUsage = null, UsageDetails answersUsage = null, TokenUsage blobsUsage = null)
    {
        var questionsTokenUsage = questionsUsage == null
            ? null
            : new TokenUsage
            {
                InputTokens = questionsUsage.InputTokenCount,
                OutputTokens = questionsUsage.OutputTokenCount
            };

        var answersTokenUsage = answersUsage == null
            ? null
            : new TokenUsage
            {
                InputTokens = answersUsage.InputTokenCount,
                OutputTokens = answersUsage.OutputTokenCount
            };

        if (questionsTokenUsage == null && answersTokenUsage == null && blobsUsage == null)
        {
            return null;
        }

        return (questionsTokenUsage ?? new TokenUsage()) + (answersTokenUsage ?? new TokenUsage()) + (blobsUsage ?? new TokenUsage());
    }
}