using System.Collections.Generic;
using Vivet.AI.Services.Responses.ImageExtraction.Models;

namespace Vivet.AI.Services.Responses.ImageExtraction;

/// <summary>
/// Represents the response related to the image extraction operation.
/// </summary>
public class ImageExtractionResponse : BaseResponse
{
    /// <summary>
    /// The texts extracted from the image.
    /// </summary>
    public virtual IEnumerable<ExtractedText> Texts { get; set; } = [];
}