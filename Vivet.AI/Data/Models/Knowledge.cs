using Microsoft.Extensions.VectorData;
using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Data.Models;

/// <summary>
/// Represents a knowledge entry used for embedding and vector search,
/// including metadata such as tenant, user, source, tags, and content type flags.
/// Inherits from <see cref="BaseEmbedding"/>.
/// </summary>
public class Knowledge : BaseEmbedding
{
    /// <summary>
    /// Gets or sets the tenant identifier associated with this knowledge entry.
    /// Indexed for text search.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the sub-tenant identifier associated with this knowledge entry.
    /// Indexed for text search.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string SubTenantId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with this knowledge entry.
    /// Indexed for text search.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string UserId { get; set; }

    /// <summary>
    /// Gets or sets the source of the knowledge entry, e.g., document or URL.
    /// Indexed for vector store queries.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string Source { get; set; }

    /// <summary>
    /// Gets or sets the creator of this knowledge entry.
    /// Indexed for vector store queries.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets an array of tags associated with this knowledge entry.
    /// Required and indexed for query filtering.
    /// </summary>
    [Required]
    [VectorStoreData(IsIndexed = true)]
    public virtual string[] Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets the serialized metadata of any associated blob.
    /// </summary>
    [VectorStoreData]
    public string BlobMetadata { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this knowledge entry represents an image.
    /// Indexed for vector store queries.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual bool IsImage { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this knowledge entry represents an audio file.
    /// Indexed for vector store queries.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual bool IsAudio { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this knowledge entry represents a video.
    /// Indexed for vector store queries.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual bool IsVideo { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this knowledge entry represents a document.
    /// Indexed for vector store queries.
    /// </summary>
    [VectorStoreData(IsIndexed = true)]
    public virtual bool IsDocument { get; set; } = false;
}