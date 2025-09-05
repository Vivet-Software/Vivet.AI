using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Services.Models.Blobs.Data;

/// <summary>
/// Represents blob data that is sourced from an <see cref="IFormFile"/>.
/// </summary>
public class BlobDataFile : BaseBlobData
{
    /// <summary>
    /// Gets or sets the form file containing the blob data.
    /// </summary>
    [Required]
    public virtual IFormFile File { get; set; }

    internal override async Task<string> GetBase64()
    {
        await using var stream = this.File
            .OpenReadStream();

        return stream
            .ToBase64();
    }
}