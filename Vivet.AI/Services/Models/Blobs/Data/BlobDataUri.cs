using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Services.Models.Blobs.Data;

/// <summary>
/// Represents blob data that is sourced from a URI.
/// </summary>
public class BlobDataUri : BaseBlobData
{
    /// <summary>
    /// Gets or sets the URI from which the blob data is retrieved.
    /// </summary>
    [Required]
    public virtual Uri Uri { get; set; }

    internal override async Task<string> GetBase64()
    {
        var response = await new HttpClient()
            .GetAsync(this.Uri)
            .ConfigureAwait(false);

        response
            .EnsureSuccessStatusCode();

        await using var stream = await response.Content
            .ReadAsStreamAsync()
            .ConfigureAwait(false);

        return stream
            .ToBase64();
    }
}