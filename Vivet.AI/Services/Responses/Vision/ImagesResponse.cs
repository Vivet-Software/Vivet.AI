using System.Collections.Generic;
using Vivet.AI.Services.Responses.Vision.Models;

namespace Vivet.AI.Services.Responses.Vision;

/// <summary>
/// Represents the response related to a vision operation.
/// </summary>
public class ImagesResponse : BaseResponse
{
    /// <summary>
    /// The image extracted from the blob.
    /// </summary>
    public virtual IEnumerable<ExtractedImage> Texts { get; set; } = [];
}