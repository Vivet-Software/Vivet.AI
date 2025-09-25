using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Metadata.Models;

namespace Vivet.AI.Services.Requests.Metadata;

/// <summary>
/// Represents a request to retrieve metadata from a blob.
/// </summary>
public class GetMetadataRequest
{
    /// <summary>
    /// Gets or sets the blob from which metadata will be retrieved.
    /// </summary>
    [Required]
    public virtual BaseBlob Blob { get; set; }

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual MetadataConfigOverrides ConfigOverrides { get; } = new();
}