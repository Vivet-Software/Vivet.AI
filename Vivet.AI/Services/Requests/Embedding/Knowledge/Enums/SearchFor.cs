namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Enums;

/// <summary>
/// Specifies the type of content to search for.
/// </summary>
public enum SearchFor
{
    /// <summary>
    /// Search for text content.
    /// </summary>
    Text,

    /// <summary>
    /// Search for image content.
    /// </summary>
    Image,

    /// <summary>
    /// Search for audio content.
    /// </summary>
    Audio,

    /// <summary>
    /// Search for video content.
    /// </summary>
    Video,

    /// <summary>
    /// Search for document content.
    /// </summary>
    Document
}