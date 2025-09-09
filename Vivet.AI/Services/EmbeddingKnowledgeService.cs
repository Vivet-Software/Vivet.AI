using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Newtonsoft.Json;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;
using Vivet.AI.Data.Stores;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Helpers.Models;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding;
using Vivet.AI.Services.Requests.Embedding.Knowledge;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models;
using Vivet.AI.Services.Requests.Metadata;
using Vivet.AI.Services.Responses.Embeddings.Knowledge;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;
using Vivet.AI.Services.Responses.Metadata;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

// TODO: AI Services
// - Read the rest of MS web pages
// - Check common services (Azure, HuggingFace) and consider whether we should integrate them into the library
//   - Check in Azure AI Foundry which types of models that can be deployed (when deploying a model there is a list to filter model types)
//   - SemanticKernel Services: https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/integrations
//   - all azure ai services: https://portal.azure.com/#view/Microsoft_Azure_ProjectOxford/CognitiveServicesHub/~/overview (Azure Document Intelligence)
//   - Text Analytics: Azure Cognitive Services Text Analytics is a cloud service that provides advanced natural language processing over raw text,
//     and features like Language Detection, Sentiment Analysis, Key Phrase Extraction, Named Entity Recognition, Personally Identifiable Information (PII) Recognition,
//     Linked Entity Recognition, Text Analytics for Health, and more.
// - https://mem0.ai memory service
// - https://n8n.io - Flexible AI workflow automation for technical teams
// - FlowiseAI, CrewA (from wastehero add, what is it?)
// - https://learn.microsoft.com/en-us/microsoft-copilot-studio/fundamentals-what-is-copilot-studio  -  Copilot Studio is a graphical, low-code tool for building agents and agent flows.
// - https://dev.to/zenstack/turning-your-database-into-an-mcp-server-with-auth-32mp - Turning Your Database Into an MCP Server With Auth
// - https://dev.to/copilotkit/30-mcp-ideas-with-complete-source-code-d8e - 30+ MCP Ideas with Complete Source Code
// - Vision models
// TODO: Handle Blobs better. (after AI Services, e.g. Docuemnt Intelligence)
// - SK: Image to Text (Experimental) ???
// - SK: Audio to Text (Experimental) ???
// - Azure.AI.DocumentIntelligence + There was a package also to store files available to the LLM on blob. Check it out.
// TODO: Automatic Language Detection usig AI, and setting the Language on memory / knowledge
// We could for memory just let the chat model return it as part of the json???? GOOD IDEA

// ----------------------------------------------------------------------------------------------------------------------

// TODO: Functions / Plugins (check other TODO's)

// TODO: Check CodeQL and add as check in branch protection when merging to master (it won't trigger before it seems)
// TODO: Check Sponsors (GitHub is reviewing)

/// <inheritdoc cref="IEmbeddingKnowledgeService"/>
public class EmbeddingKnowledgeService(EmbeddingOptions options, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, KnowledgeVectorStore vectorStore, IMetadataService metadataService = null)
    : BaseEmbeddingService(options, embeddingGenerator, metadataService), IEmbeddingKnowledgeService
{
    private readonly KnowledgeVectorStore vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
    private readonly EmbeddingOptions.KnowledgeOptions knowledgeOptions = options.Knowledge ?? throw new ArgumentNullException(nameof(options.Knowledge));

    /// <inheritdoc />
    public virtual Task<IndexKnowledgeResponse> IndexAsync<TOverrides>(BaseIndexKnowledgeRequst<TOverrides> request, CancellationToken cancellationToken = default)
        where TOverrides : BaseConfigOverrides, new()
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        request
            .Validate();

        return request switch
        {
            BaseIndexKnowledgeRequst<KnowledgeConfigOverrides> genericRequest
                when genericRequest.GetType().IsGenericType && genericRequest.GetType().GetGenericTypeDefinition() == typeof(IndexTextRequest<>)
                => this.IndexTextReflectionAsync(genericRequest, cancellationToken),

            IndexTextRequest textRequest => this.IndexTextAsync(textRequest, cancellationToken),
            IndexImageRequest imageRequest => this.IndexBlobAsync(imageRequest, cancellationToken),
            IndexAudioRequest videoRequest => this.IndexBlobAsync(videoRequest, cancellationToken),
            IndexVideoRequest videoRequest => this.IndexBlobAsync(videoRequest, cancellationToken),
            IndexDocumentRequest documentRequest => this.IndexBlobAsync(documentRequest, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };
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

        var knowledges = this.vectorStore.Collection
            .SearchAsync(request.Query, request.Limit, vectorSearchOptions, cancellationToken);

        var results = await knowledges
            .Select(result =>
            {
                var baseScore = result.Score ?? 0.0;
                var recencyScore = result.Record
                    .GetRecencyScore(this.knowledgeOptions.Scoring);

                var adjustedScore = baseScore + recencyScore;

                return new
                {
                    AdjustedScore = adjustedScore,
                    OriginalScore = baseScore,
                    Result = result
                };
            })
            .Where(x => x.AdjustedScore >= this.options.MatchScoreThreashold)
            .OrderByDescending(x => x.AdjustedScore)
            .Select(x => new SearchKnowledgeResult
            {
                Score = x.AdjustedScore,
                Result = new KnowledgeResult(x.Result.Record)
            })
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

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

        var textChunks = TextChunking.GetTextChunks(text, this.knowledgeOptions.TextChunking.MinTokens, this.knowledgeOptions.TextChunking.MaxTokens);

        var embedTextChunks = new List<TextChunk>();
        foreach (var textChunk in textChunks)
        {
            var contentHash = textChunk.Text
                .GetContentHash();

            var existingEmbedding = await this.vectorStore.Collection
                .GetAsync(x =>
                        x.ContentHash == contentHash &&
                        x.TenantId == request.TenantId &&
                        x.SubTenantId == request.SubTenantId &&
                        x.ScopeId == request.ScopeId &&
                        x.UserId == request.UserId,
                    1, cancellationToken: cancellationToken)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (existingEmbedding == null)
            {
                embedTextChunks
                    .Add(textChunk);
            }
        }
        
        var embeddings = await this.GenerateEmbeddings(embedTextChunks.Select(x => x.Text).ToArray(), cancellationToken)
            .ConfigureAwait(false);

        var knowledges = embedTextChunks
            .Select((x, i) =>
            {
                var fullContext = TextChunking.GetTextChunkNeighboringContext(embedTextChunks.ToArray(), i, this.knowledgeOptions.TextChunking.NeighborContext.ContextWindow, this.knowledgeOptions.TextChunking.NeighborContext.RestrictToSameParagraph);

                return new Knowledge
                {
                    Vector = embeddings[i].Vector,
                    Content = x.Text,
                    FullContext = fullContext,
                    Order = i,
                    Language = request.Language,
                    EmbeddingModel = this.options.Model.Name,
                    TenantId = request.TenantId,
                    SubTenantId = request.SubTenantId,
                    ScopeId = request.ScopeId,
                    UserId = request.UserId,
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

        // TODO: Support Image Embeddings

        var blobData = await request.Blob
            .GetBlobData()
            .ConfigureAwait(false);

        var metadataResponse = await this.GetBlobMetadata(request, cancellationToken).ConfigureAwait(false);
        var embeddings = await this.GenerateEmbeddings([metadataResponse.Metadata.Summary], cancellationToken).ConfigureAwait(false);

        var embedding = embeddings
            .FirstOrDefault();

        if (embedding == null)
        {
            throw new NullReferenceException(nameof(embedding));
        }

        await this.vectorStore.Collection
            .UpsertAsync(new Knowledge
            {
                Vector = embedding.Vector,
                Content = metadataResponse.Metadata.Summary,
                FullContext = metadataResponse.Metadata.Description,
                Order = 0,
                Language = request.Language,
                EmbeddingModel = this.options.Model.Name,
                TenantId = request.TenantId,
                SubTenantId = request.SubTenantId,
                ScopeId = request.ScopeId,
                UserId = request.UserId,
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
    private async Task<dynamic> GetBlobMetadata<TMimeType>(BaseIndexBlobRequest<TMimeType> request, CancellationToken cancellationToken = default)
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
        else if (this.metadataService != null && ((request.ConfigOverrides.Metadata.UseAutomaticMetadataRetrieval ?? false) || (this.knowledgeOptions.UseAutomaticMetadataRetrieval && request.ConfigOverrides.Metadata.UseAutomaticMetadataRetrieval != false))) 
        {
            var metadataMethod = this.metadataService
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == nameof(MetadataService.GetAsync) && m.IsGenericMethodDefinition);

            var metadataRequest = new GetMetadataRequest
            {
                Blob = request.Blob,
                ConfigOverrides =
                {
                    SummaryMaxWords = request.ConfigOverrides.Metadata.SummaryMaxWords,
                    DescriptionMaxWords = request.ConfigOverrides.Metadata.DescriptionMaxWords
                }
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
            throw new AiException("No metadata available. Either include metadata in the request, or enable automatic metadata retrieval in the configuration or for this request."); 
        }

        return metadataResponse;
    }
}