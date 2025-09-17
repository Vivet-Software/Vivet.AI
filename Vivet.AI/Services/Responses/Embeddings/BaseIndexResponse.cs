using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Responses.Embeddings;

/// <summary>
/// Represents the base response for an indexing operation.
/// </summary>
public abstract class BaseIndexResponse : BaseResponse
{
    /// <summary>
    /// The total number of embeddings created.
    /// </summary>
    public virtual int TotalEmbeddings { get; set; } = 0;

    /// <summary>
    /// The total size of all embeddings.
    /// </summary>
    public virtual long TotalEmbeddingsSize { get; set; } = 0L;

    /// <summary>
    /// The token usage associated with metadata operations.
    /// </summary>
    public virtual TokenUsage MetadataTokenUsage { get; set; }
}