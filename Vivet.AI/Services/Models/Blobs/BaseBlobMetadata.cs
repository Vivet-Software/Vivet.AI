using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents the base metadata for a blob, providing a <see cref="Metadata"/> property.
/// </summary>
public abstract class BaseBlobMetadata : BaseBlob
{
    /// <summary>
    /// Metadata used to get content and full context for the vector entry.
    /// If null, metadata will automatically be retrieved, unless deactiveated in configuration. Make sure that if disabled this is not left null,
    /// or and incomplete record will be stored in the vector store.
    /// </summary>
    public virtual Metadata Metadata { get; set; }
}

/// <summary>
/// Represents the base metadata for a blob with a specific MIME type.
/// </summary>
/// <typeparam name="TMimeType">The type of MIME type. Must inherit from <see cref="BaseMimeType"/>.</typeparam>
public abstract class BaseBlobMetadata<TMimeType> : BaseBlobMetadata
    where TMimeType : BaseMimeType
{
    /// <summary>
    /// Gets or sets the MIME type of the blob.
    /// </summary>
    [Required]
    public virtual TMimeType MimeType { get; set; }

    internal override async Task<(string Base64, string MimeType, string DataUri)> GetBlobData(CancellationToken cancellationToken = default)
    {
        var base64 = await this.Data
            .GetBase64()
            .ConfigureAwait(false);

        var dataUri = this.Data
            .GetDataUri(base64, this.MimeType.Value);

        return new()
        {
            Base64 = base64,
            MimeType = this.MimeType.Value,
            DataUri = dataUri
        };
    }
}