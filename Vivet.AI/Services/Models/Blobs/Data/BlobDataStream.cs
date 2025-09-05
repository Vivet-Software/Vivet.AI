using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Services.Models.Blobs.Data;

/// <summary>
/// Represents blob data that is sourced from a <see cref="Stream"/>.
/// </summary>
public class BlobDataStream : BaseBlobData
{
    /// <summary>
    /// Gets or sets the stream containing the blob data.
    /// </summary>
    [Required]
    public virtual Stream Stream { get; set; }

    internal override async Task<string> GetBase64()
    {
        await Task.CompletedTask;

        return this.Stream
            .ToBase64();
    }
}