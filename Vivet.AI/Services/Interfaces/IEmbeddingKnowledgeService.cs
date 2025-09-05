using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding;
using Vivet.AI.Services.Requests.Embedding.Knowledge;
using Vivet.AI.Services.Responses.Embeddings.Knowledge;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Provides functionality for indexing, searching, querying, and deleting knowledge embeddings in a vector store,
/// with optional metadata retrieval support.
/// </summary>
public interface IEmbeddingKnowledgeService
{
    /// <summary>
    /// Indexes the specified knowledge request (text, image, audio, video, or document) into the vector store,
    /// creating new embeddings where necessary.
    /// </summary>
    /// <typeparam name="TOverrides">The config override type.</typeparam>
    /// <param name="request">The knowledge request to index.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>The indexing result, including counts and token usage.</returns>
    Task<IndexKnowledgeResponse> IndexAsync<TOverrides>(BaseIndexKnowledgeRequst<TOverrides> request, CancellationToken cancellationToken = default)
        where TOverrides : BaseConfigOverrides, new();

    /// <summary>
    /// Searches for knowledge entries matching the given search request.
    /// Results are scored and filtered based on match and recency scores.
    /// </summary>
    /// <param name="request">The search request criteria.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>A collection of matching knowledge search results.</returns>
    Task<QueryKnowledgeResponse> QueryAsync(QueryKnowledgeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries knowledge entries directly from the vector store using filtering and ordering criteria
    /// without semantic similarity scoring.
    /// </summary>
    /// <param name="request">The query request criteria.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>A collection of query results with size information.</returns>
    Task<SearchKnowledgeResponse> SearchAsync(SearchKnowledgeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes knowledge entries from the vector store by their unique identifiers.
    /// </summary>
    /// <param name="request">The delete request containing IDs to remove.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default);
}