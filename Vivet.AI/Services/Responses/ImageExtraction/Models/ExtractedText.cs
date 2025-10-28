namespace Vivet.AI.Services.Responses.ImageExtraction.Models;

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
    /// The language of the transcribed text.
    /// </summary>
    public virtual string Language { get; set; }
}