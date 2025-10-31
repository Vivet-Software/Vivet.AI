namespace Vivet.AI.Services.Responses.Vision.Models;

/// <summary>
/// Represents extracted text related to the image extraction operation.
/// </summary>
public class ExtractedText
{
    /// <summary>
    /// The content of the extracted text.
    /// </summary>
    public virtual string Content { get; set; }

    /// <summary>
    /// The language of the extracted text.
    /// </summary>
    public virtual string Language { get; set; }
}