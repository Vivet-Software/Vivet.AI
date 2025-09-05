using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Vivet.AI.Services.Models.Blobs.Data;

namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents the base class for a blob, providing access to the blob's data.
/// </summary>
public abstract class BaseBlob
{
    /// <summary>
    /// Gets or sets the data associated with this blob.
    /// </summary>
    [Required]
    public virtual BaseBlobData Data { get; set; }

    internal abstract Task<(string Base64, string MimeType, string DataUri)> GetBlobData();
}

/// <summary>
/// Represents a blob with a specific MIME type.
/// </summary>
/// <typeparam name="TMimeType">The type of MIME type. Must inherit from <see cref="BaseMimeType"/>.</typeparam>
public abstract class BaseBlob<TMimeType> : BaseBlob
    where TMimeType : BaseMimeType
{
    /// <summary>
    /// Gets or sets the MIME type of the blob.
    /// </summary>
    [Required]
    public virtual TMimeType MimeType { get; set; }

    internal override async Task<(string Base64, string MimeType, string DataUri)> GetBlobData()
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