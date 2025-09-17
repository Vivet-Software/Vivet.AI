using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Vivet.AI.Services.Models.Blobs.Data;

/// <summary>
/// Represents blob data that is provided as a Base64-encoded string.
/// </summary>
public class BlobDataBase64 : BaseBlobData
{
    /// <summary>
    /// Gets or sets the Base64-encoded string representing the blob data.
    /// </summary>
    [Required]
    public virtual string Base64 { get; set; }

    internal override async Task<string> GetBase64()
    {
        await Task.CompletedTask;

        return this.Base64;
    }
}