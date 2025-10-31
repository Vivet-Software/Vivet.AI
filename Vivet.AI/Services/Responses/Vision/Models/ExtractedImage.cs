namespace Vivet.AI.Services.Responses.Vision.Models;

/// <summary>
/// Represents extracted image related to the extraction operation.
/// </summary>
public class ExtractedImage
{
    /// <summary>
    /// The content of the extracted text.
    /// </summary>
    public virtual string Base64 { get; set; }
}