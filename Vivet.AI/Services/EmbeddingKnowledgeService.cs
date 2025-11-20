using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;
using Vivet.AI.Data.Stores;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Helpers.Models;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Models.MimeTypes;
using Vivet.AI.Services.Requests.Embedding;
using Vivet.AI.Services.Requests.Embedding.Knowledge;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Metadata;
using Vivet.AI.Services.Responses.Embeddings.Knowledge;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;
using Vivet.AI.Services.Responses.Metadata;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

/// <inheritdoc cref="IEmbeddingKnowledgeService"/>
public class EmbeddingKnowledgeService(EmbeddingOptions options, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, KnowledgeVectorStore vectorStore, IMetadataService metadataService = null)
    : BaseEmbeddingService(options, embeddingGenerator, metadataService), IEmbeddingKnowledgeService
{
    private readonly KnowledgeVectorStore vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
    private readonly EmbeddingKnowledgeOptions knowledgeOptions = options.Knowledge ?? throw new ArgumentNullException(nameof(options.Knowledge));

    /// <inheritdoc />
    public virtual async Task<IndexKnowledgeResponse> IndexAsync<TOverrides>(BaseIndexKnowledgeRequst<TOverrides> request, CancellationToken cancellationToken = default)
        where TOverrides : BaseConfigOverrides, new()
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        request
            .Validate();

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        var response = request switch
        {
            BaseIndexKnowledgeRequst<KnowledgeIndexConfigOverrides> genericRequest
                when genericRequest.GetType().IsGenericType && genericRequest.GetType().GetGenericTypeDefinition() == typeof(IndexTextRequest<>)
                => await this.IndexTextReflectionAsync(genericRequest, cancellationToken),

            IndexTextRequest textRequest => await this.IndexTextAsync(textRequest, cancellationToken),
            IndexImageRequest imageRequest => await this.IndexBlobAsync(imageRequest, cancellationToken),
            IndexAudioRequest videoRequest => await this.IndexBlobAsync(videoRequest, cancellationToken),
            IndexVideoRequest videoRequest => await this.IndexBlobAsync(videoRequest, cancellationToken),
            IndexDocumentRequest documentRequest => await this.IndexBlobAsync(documentRequest, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };

        stopwatch
            .Stop();

        response.ElapsedTime = stopwatch.Elapsed;

        return response;
    }

    /// <inheritdoc />
    public virtual async Task<SearchKnowledgeResponse> SearchAsync(SearchKnowledgeRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var vectorSearchOptions = new VectorSearchOptions<Knowledge>
        {
            Filter = request.Criteria  
                .BuildFilter()
        };

        var useQueryDeduplication = request.ConfigOverrides.UseQueryDeduplication ?? this.knowledgeOptions.Search.UseQueryDeduplication;
        var contextQueryLimit = request.ConfigOverrides.ContextQueryLimit ?? this.knowledgeOptions.Search.ContextQueryLimit;

        var limit = request.Limit ?? (useQueryDeduplication
            ? contextQueryLimit * 2
            : contextQueryLimit);

        var knowledges = this.vectorStore.Collection
            .SearchAsync(request.Query, limit, vectorSearchOptions, cancellationToken);

        var results = await knowledges
            .Select(result =>
            {
                var baseScore = result.Score ?? 0.0;
                var recencyScore = result.Record
                    .GetRecencyScore(this.knowledgeOptions.Search.Scoring, request.ConfigOverrides.Scoring);

                var adjustedScore = baseScore + recencyScore;

                return new
                {
                    AdjustedScore = adjustedScore,
                    OriginalScore = baseScore,
                    Result = result
                };
            })
            .Where(x => x.AdjustedScore >= knowledgeOptions.Search.Scoring.MatchScoreThreshold)
            .OrderByDescending(x => x.AdjustedScore)
            .Select(x => new SearchKnowledgeResult
            {
                Score = x.AdjustedScore,
                Result = new KnowledgeResult(x.Result.Record)
            })
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        if (useQueryDeduplication)
        {
            var deduplicationMatchScoreThreshold = request.ConfigOverrides.Scoring.DeduplicationMatchScoreThreshold ?? this.knowledgeOptions.Search.Scoring.DeduplicationMatchScoreThreshold;
            var deduplicatedResults = ContextDeduplicator.DeduplicateKnowledgeResults(results, deduplicationMatchScoreThreshold);

            results = deduplicatedResults
                .Take(contextQueryLimit)
                .ToArray();
        }

        stopwatch
            .Stop();

        return new SearchKnowledgeResponse
        {
            Results = results,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    /// <inheritdoc />
    public virtual async Task<QueryKnowledgeResponse> QueryAsync(QueryKnowledgeRequest request, CancellationToken cancellationToken = default)
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

        var retrievalOptions = new FilteredRecordRetrievalOptions<Knowledge>
        {
            Skip = request.Skip,
            OrderBy = x => x
                .Ascending(y => y.UnixTimestamp)
        };

        var knowledges = this.vectorStore.Collection
            .GetAsync(filter, request.Limit, retrievalOptions, cancellationToken);

        var results = await knowledges
            .Select(x => new QueryKnowledgeResult
            {
                Result = new KnowledgeResult(x),
                Size = x.Content
                    .GetUtf8ByteCount()
            })
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        stopwatch
            .Stop();

        return new QueryKnowledgeResponse
        {
            Results = results,
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


    private Task<IndexKnowledgeResponse> IndexTextReflectionAsync(object request, CancellationToken cancellationToken)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        var method = typeof(EmbeddingKnowledgeService)
            .GetMethod(nameof(IndexTextAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
        {
            throw new NullReferenceException(nameof(method));
        }

        var genericTypeArgument = request.GetType().GenericTypeArguments[0];

        var genericMethod = method
            .MakeGenericMethod(genericTypeArgument);

        if (genericMethod == null)
        {
            throw new NullReferenceException(nameof(genericMethod));
        }

        return (Task<IndexKnowledgeResponse>)genericMethod.Invoke(this, [request, cancellationToken]);
    }
    private async Task<IndexKnowledgeResponse> IndexTextAsync<T>(IndexTextRequest<T> request, CancellationToken cancellationToken = default) 
        where T : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var text = request.Text switch
        {
            string answer => answer,
            _ => JsonConvert.SerializeObject(request.Text, Formatting.None, Settings.SerializerSettings)
        };

        var tenantId = request.TenantId?.ToString();
        var subTenantId = request.SubTenantId?.ToString();
        var scopeId = request.ScopeId?.ToString();
        var userId = request.UserId?.ToString();

        var minTokens = request.ConfigOverrides.TextChunking.MinTokens ?? this.knowledgeOptions.Indexing.TextChunking.MinTokens;
        var maxTokens = request.ConfigOverrides.TextChunking.MaxTokens ?? this.knowledgeOptions.Indexing.TextChunking.MaxTokens;

        var textChunks = TextChunking.GetTextChunks(text, minTokens, maxTokens);

        var embedTextChunks = new List<TextChunk>();
        foreach (var textChunk in textChunks)
        {
            var contentHash = textChunk.Text
                .GetContentHash();

            var existingEmbedding = await this.vectorStore.Collection
                .GetAsync(x =>
                        x.ContentHash == contentHash &&
                        x.TenantId == tenantId &&
                        x.SubTenantId == subTenantId &&
                        x.ScopeId == scopeId &&
                        x.UserId == userId,
                    1, cancellationToken: cancellationToken)    
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (existingEmbedding == null)
            {
                embedTextChunks
                    .Add(textChunk);
            }
        }
        
        var embeddings = await this.GenerateEmbeddings(embedTextChunks.Select(x => x.Text).ToArray(), request.ConfigOverrides, cancellationToken)
            .ConfigureAwait(false);

        var knowledges = embedTextChunks
            .Select((x, i) =>
            {
                var contextWindow = request.ConfigOverrides.TextChunking.NeighborContext.ContextWindow ?? this.knowledgeOptions.Indexing.TextChunking.NeighborContext.ContextWindow;
                var restrictToSameParagraph = request.ConfigOverrides.TextChunking.NeighborContext.RestrictToSameParagraph ?? this.knowledgeOptions.Indexing.TextChunking.NeighborContext.RestrictToSameParagraph;

                var fullContext = TextChunking.GetTextChunkNeighboringContext(embedTextChunks.ToArray(), i, contextWindow, restrictToSameParagraph);

                return new Knowledge
                {
                    Vector = embeddings[i].Vector,
                    Content = x.Text,
                    FullContext = fullContext,
                    Order = i,
                    Language = request.Language,
                    EmbeddingModel = this.options.Model.Name,
                    TenantId = tenantId,
                    SubTenantId = subTenantId,
                    ScopeId = scopeId,
                    UserId = userId,
                    Source = request.Source,
                    CreatedBy = request.CreatedBy,
                    Tags = request.Tags
                };
            })
            .ToArray();

        await this.vectorStore.Collection
            .UpsertAsync(knowledges, cancellationToken)
            .ConfigureAwait(false);

        return new IndexKnowledgeResponse
        {
            TotalEmbeddings = knowledges.Length,
            TotalEmbeddingsSize = knowledges
                .Select(x => x.Content)
                .Sum(x => x.GetUtf8ByteCount()),
            TokenUsage = embeddings.Usage == null || embeddings.Count == 0
                ? null
                : new TokenUsage
                {
                    InputTokens = embeddings.Usage.InputTokenCount,
                    OutputTokens = embeddings.Usage.OutputTokenCount
                }
        };
    }
    private async Task<IndexKnowledgeResponse> IndexBlobAsync<TMimeType>(BaseIndexBlobRequest<TMimeType> request, CancellationToken cancellationToken = default) 
        where TMimeType : BaseMimeType
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var blobData = await request.Blob
            .GetBlobData(cancellationToken)
            .ConfigureAwait(false);

        var metadataResponse = await this.CreateBlobMetadata(request, cancellationToken).ConfigureAwait(false);
        var embeddings = await this.GenerateEmbeddings([metadataResponse.Metadata.Summary], request.ConfigOverrides, cancellationToken).ConfigureAwait(false);

        var embedding = embeddings
            .FirstOrDefault();

        if (embedding == null)
        {
            throw new NullReferenceException(nameof(embedding));
        }

        var tenantId = request.TenantId?.ToString();
        var subTenantId = request.SubTenantId?.ToString();
        var scopeId = request.ScopeId?.ToString();
        var userId = request.UserId?.ToString();

        await this.vectorStore.Collection
            .UpsertAsync(new Knowledge
            {
                Vector = embedding.Vector,
                Content = metadataResponse.Metadata.Summary,
                FullContext = metadataResponse.Metadata.Description,
                Order = 0,
                Language = request.Language,
                EmbeddingModel = this.options.Model.Name,
                TenantId = tenantId,
                SubTenantId = subTenantId,
                ScopeId = scopeId,
                UserId = userId,
                Source = request.Source,
                CreatedBy = request.CreatedBy,
                Tags = request.Tags,
                BlobBase64 = blobData.Base64,
                BlobMimeType = blobData.MimeType,
                BlobMetadata = metadataResponse.AdditionalMetadata == null
                    ? null
                    : JsonConvert.SerializeObject(metadataResponse.AdditionalMetadata, Settings.SerializerSettings),
                IsImage = request is IndexImageRequest,
                IsAudio = request is IndexAudioRequest,
                IsVideo = request is IndexVideoRequest,
                IsDocument = request is IndexDocumentRequest
            }, cancellationToken)
            .ConfigureAwait(false);

        return new IndexKnowledgeResponse
        {
            TotalEmbeddings = 1,
            TotalEmbeddingsSize = blobData.DataUri.GetUtf8ByteCount(),
            TokenUsage = embeddings.Usage == null || embeddings.Count == 0
                ? null
                : new TokenUsage
                {
                    InputTokens = embeddings.Usage.InputTokenCount,
                    OutputTokens = embeddings.Usage.OutputTokenCount
                },
            MetadataTokenUsage = metadataResponse.TokenUsage
        };
    }
    private async Task<dynamic> CreateBlobMetadata<TMimeType>(BaseIndexBlobRequest<TMimeType> request, CancellationToken cancellationToken = default)
        where TMimeType : BaseMimeType
    {
        var blobType = request.Blob
            .GetType();

        var additionalMetadataProperty = blobType
            .GetProperty(nameof(BaseBlobAdditionalMetadata<BaseMimeType, dynamic>.AdditionalMetadata));

        var requestMetadata = request.Blob.Metadata;

        var requestAdditionalMetadata = additionalMetadataProperty?
            .GetValue(request.Blob);

        var additionalMetadataType = blobType
            .GetMetadataType();

        additionalMetadataType = requestAdditionalMetadata != null
            ? typeof(object)
            : additionalMetadataType ?? typeof(object);

        dynamic metadataResponse = null;
        if (requestMetadata != null && (requestAdditionalMetadata != null || additionalMetadataType == typeof(object)))
        {
            var genericType = typeof(MetadataResponse<>)
                .MakeGenericType(additionalMetadataType);

            metadataResponse = Activator.CreateInstance(genericType);

            genericType
                .GetProperty(nameof(MetadataResponse.Metadata))?
                .SetValue(metadataResponse, requestMetadata);

            genericType
                .GetProperty(nameof(MetadataResponse<dynamic>.AdditionalMetadata))?
                .SetValue(metadataResponse, requestAdditionalMetadata);
        }
        else if (this.metadataService != null && (request.ConfigOverrides.UseAutomaticMetadataRetrieval ?? this.knowledgeOptions.Indexing.UseAutomaticMetadataRetrieval))
        {
            var metadataMethod = this.metadataService
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == nameof(MetadataService.GetAsync) && m.IsGenericMethodDefinition);

            var metadataRequest = new GetMetadataRequest
            {
                Blob = request.Blob,
                ConfigOverrides = request.ConfigOverrides.Metadata
            };

            var task = (Task)metadataMethod?
                .MakeGenericMethod(additionalMetadataType)
                .Invoke(this.metadataService, [metadataRequest, cancellationToken]);

            if (task == null)
            {
                throw new NullReferenceException(nameof(task));
            }

            await task
                .ConfigureAwait(false);

            metadataResponse = task
                .GetType()
                .GetProperty(nameof(Task<dynamic>.Result))?
                .GetValue(task);

            if (requestMetadata != null)
            {
                metadataResponse?
                    .GetType()
                    .GetProperty(nameof(MetadataResponse.Metadata))?
                    .SetValue(metadataResponse, requestMetadata);
            }

            if (requestAdditionalMetadata != null)
            {
                metadataResponse?
                    .GetType()
                    .GetProperty(nameof(MetadataResponse<dynamic>.AdditionalMetadata))?
                    .SetValue(metadataResponse, requestAdditionalMetadata);
            }
        }

        if (metadataResponse == null)
        {
            throw new InvalidOperationException("No metadata available. Either include metadata in the request, or enable automatic metadata retrieval in the configuration or for this request.");
        }

        if (metadataResponse.Exception != null)
        {
            throw metadataResponse.Exception;
        }

        return metadataResponse;
    }
}