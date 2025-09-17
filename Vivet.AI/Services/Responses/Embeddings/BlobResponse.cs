using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs.Consts;
using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Responses.Embeddings;

/// <summary>
/// Represents the response containing blob data.
/// </summary>
public class BlobResponse
{
    /// <summary>
    /// The Base64-encoded content of the blob.
    /// </summary>
    [Required]
    public virtual string Base64 { get; set; }

    /// <summary>
    /// The MIME type of the blob.
    /// </summary>
    [Required]
    public virtual MimeType MimeType { get; set; }

    /// <summary>
    /// The hash of the blob content.
    /// </summary>
    [Required]
    public virtual string Hash { get; set; }

    /// <summary>
    /// Generates the Data URI representation of the blob.
    /// </summary>
    internal virtual string GetDataUri()
    {
        return string.Format(BlobDataTemplates.BLOB_DATA_TEMPLATE, this.MimeType.Value, this.Base64);
    }
}