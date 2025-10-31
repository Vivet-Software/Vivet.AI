using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Vision.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Vision;

/// <summary>
/// Abstract base class representing a vision request.
/// </summary>
/// <typeparam name="T">The type of blob.</typeparam>
public abstract class BaseVisionRequest<T>
    where T : BaseBlob
{
    /// <summary>
    /// Gets or sets the the blob for the vision request.
    /// </summary>
    [Required]
    public virtual T Blob { get; set; }

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual VisionConfigOverrides ConfigOverrides { get; } = new();
}