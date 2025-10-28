using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.ImageExtraction.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.ImageExtraction;

/// <summary>
/// Represents a image extraction request.
/// </summary>
public class ImageExtractionRequest
{
    /// <summary>
    /// Gets or sets the the image blob to extract.
    /// </summary>
    [Required]
    public virtual ImageBlob Blob { get; set; }

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual ImageExtractionConfigOverrides ConfigOverrides { get; } = new();
}