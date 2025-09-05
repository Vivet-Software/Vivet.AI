using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Data.Models;

/// <summary>
/// Represents a base class for embeddings used in vector stores,
/// including metadata, content, vector representation, and indexing information.
/// </summary>
public abstract class BaseEmbedding
{
    private string content;

    /// <summary>
    /// Gets or sets the unique identifier for this embedding.
    /// Automatically initialized to a new GUID.
    /// </summary>
    [Required]
    [VectorStoreKey]
    [TextSearchResultName]
    public virtual Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the scope identifier for this embedding, used to segregate data in multi-tenant or scoped contexts.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string ScopeId { get; set; }

    /// <summary>
    /// Gets or sets the vector representation of this embedding.
    /// Required for vector similarity searches.
    /// </summary>
    [Required]
    [VectorStoreVector(1, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public virtual ReadOnlyMemory<float> Vector { get; internal set; } = new();

    /// <summary>
    /// Gets or sets the raw content represented by this embedding.
    /// Setting this property automatically updates <see cref="ContentHash"/>.
    /// </summary>
    [Required]
    [VectorStoreData]
    [TextSearchResultValue]
    public virtual string Content
    {
        get => this.content;
        set
        {
            this.content = value;

            this.ContentHash = this.content?
                .GetContentHash();
        }
    }

    /// <summary>
    /// Gets the hash of the content, used for deduplication or integrity checks.
    /// Automatically updated when <see cref="Content"/> is set.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual string ContentHash { get; internal set; }

    /// <summary>
    /// Gets or sets the full context for this embedding, typically including surrounding text or metadata.
    /// </summary>
    [Required]
    [VectorStoreData]
    public virtual string FullContext { get; set; }

    /// <summary>
    /// Gets or sets the Unix timestamp representing when this embedding was created.
    /// Defaults to the current UTC time.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public long UnixTimestamp { get; internal set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// Gets or sets the order of this embedding in a sequence or document.
    /// Useful for preserving context in multi-part content.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual int Order { get; internal set; } = 0;

    /// <summary>
    /// Gets or sets the language of the content represented by this embedding.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string Language { get; set; }

    /// <summary>
    /// Gets or sets the name of the embedding model used to generate this vector.
    /// </summary>
    [VectorStoreData]
    public virtual string EmbeddingModel { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded representation of a blob associated with this embedding.
    /// </summary>
    [VectorStoreData]
    public virtual string BlobBase64 { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the blob associated with this embedding.
    /// </summary>
    [VectorStoreData]
    public virtual string BlobMimeType { get; set; }
}