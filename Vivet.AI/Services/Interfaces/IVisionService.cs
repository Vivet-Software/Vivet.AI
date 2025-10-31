using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Vision;
using Vivet.AI.Services.Responses.Vision;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Vision service interface.
/// </summary>
public interface IVisionService
{
    /// <summary>
    /// Extracts the text from an image.
    /// </summary>
    /// <typeparam name="T">The type of blob.</typeparam>
    /// <param name="request">The text extraction request containing the image to extract text from.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the <see cref="TextResponse"/>.</returns>
    Task<TextResponse> ExtractText<T>(BaseTextExtractionRequest<T> request, CancellationToken cancellationToken = default)
        where T : BaseBlob;

    /// <summary>
    /// Extracts the imaegs from a video.
    /// </summary>
    /// <typeparam name="T">The type of blob.</typeparam>
    /// <param name="request">The text extraction request containing the image to extract text from.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the <see cref="TextResponse"/>.</returns>
    Task<ImagesResponse> ExtractImages<T>(BaseImagesExtractionRequest<T> request, CancellationToken cancellationToken = default)
        where T : BaseBlob;
}