using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Requests.Embedding;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Responses.Embeddings.Memory;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Provides functionality for indexing, searching, querying, and deleting conversational memory embeddings in a vector store,
/// with optional summarization and metadata retrieval support.
/// </summary>
public interface IEmbeddingMemoryService
{
    /// <summary>
    /// Indexes a conversational memory item (question, answer, and optional blobs) into the vector store.
    /// The answer won't be summarized even if summarization is configured. The answer type <typeparamref name="T"/> will be serialized as JSON and indexed.
    /// </summary>
    /// <typeparam name="T">The type of answer object to index.</typeparam>
    /// <param name="request">The memory indexing request.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>0
    /// <returns>The indexing result, including total embeddings, size, and token usage details.</returns>
    Task<IndexMemoryResponse> IndexAsync<T>(IndexMemoryRequest<T> request, CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Searches the memory vector store for entries semantically similar to the query text.
    /// Results are scored based on semantic similarity, thread match, and recency.
    /// </summary>
    /// <param name="request">The search request containing query and filtering options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A collection of matching memory search results with scores.</returns>
    Task<SearchMemoryResponse> SearchAsync(SearchMemoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries memory entries directly from the vector store using filter and ordering criteria
    /// without performing semantic similarity scoring.
    /// </summary>
    /// <param name="request">The query request containing filter and paging information.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A collection of query results with size information.</returns>
    Task<QueryMemoryResponse> QueryAsync(QueryMemoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes memory entries from the vector store by their unique identifiers.
    /// </summary>
    /// <param name="request">The delete request containing IDs to remove.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the delete operation.</returns>
    Task DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default);
}