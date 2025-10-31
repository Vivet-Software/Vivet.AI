using System.Collections.Generic;
using Vivet.AI.Services.Responses.Vision.Models;

namespace Vivet.AI.Services.Responses.Vision;

/// <summary>
/// Represents the response related to a vision operation.
/// </summary>
public class TextResponse : BaseResponse
{
    /// <summary>
    /// The texts extracted from the blob.
    /// </summary>
    public virtual IEnumerable<ExtractedText> Texts { get; set; } = [];
}