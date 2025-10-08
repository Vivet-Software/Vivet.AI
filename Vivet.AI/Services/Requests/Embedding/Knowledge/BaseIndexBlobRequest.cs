using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.MimeTypes;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge;

/// <summary>
/// Represents the base request for indexing a single blob with optional knowledge blob configuration overrides.
/// </summary>
/// <typeparam name="TMimeType">The type of the blob MIME type. Must inherit from <see cref="BaseMimeType"/>.</typeparam>
public abstract class BaseIndexBlobRequest<TMimeType> : BaseIndexKnowledgeRequst<KnowledgeBlobConfigOverrides>
    where TMimeType : BaseMimeType
{
    /// <summary>
    /// Gets or sets the blob to be indexed.
    /// </summary>
    [Required]
    public virtual BaseBlobMetadata<TMimeType> Blob { get; set; }
}