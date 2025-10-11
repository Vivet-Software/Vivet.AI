using Microsoft.SemanticKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Models.Blobs.Data;
using Vivet.AI.Services.Models.MimeTypes;

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

    internal abstract Task<(string Base64, string MimeType, string DataUri)> GetBlobData(CancellationToken cancellationToken = default);

    internal async Task<BinaryContent> GetBinaryContent(CancellationToken cancellationToken = default)
    {
        var blobData = await this
            .GetBlobData(cancellationToken)
            .ConfigureAwait(false);

        switch (this)
        {
            case ImageBlob:
            case Requests.Metadata.Models.ImageBlob:
            case var x when x.GetType().IsGenericType && x.GetType().GetGenericTypeDefinition() == typeof(ImageBlob<>):
                return new ImageContent(blobData.DataUri);

            case AudioBlob:
            case Requests.Metadata.Models.AudioBlob:
            case var x when x.GetType().IsGenericType && x.GetType().GetGenericTypeDefinition() == typeof(AudioBlob<>):
                return new AudioContent(blobData.DataUri);

            case DocumentBlob:
            case Requests.Metadata.Models.DocumentBlob:
            case var x when x.GetType().IsGenericType && x.GetType().GetGenericTypeDefinition() == typeof(DocumentBlob<>):
                return new BinaryContent(blobData.DataUri);

            case VideoBlob:
            case Requests.Metadata.Models.VideoBlob:
            case var x when x.GetType().IsGenericType && x.GetType().GetGenericTypeDefinition() == typeof(VideoBlob<>):
                return new BinaryContent(blobData.DataUri);

            default:
                throw new ArgumentOutOfRangeException(nameof(BaseBlob), this, "The blob type is not supported.");
        }
    }
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