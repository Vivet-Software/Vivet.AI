using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Requests.ImageExtraction;
using Vivet.AI.Services.Responses.ImageExtraction;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Image extraction service interface.
/// </summary>
public interface IImageExtractionService
{
    /// <summary>
    /// Converts and image into text and extracts images.
    /// </summary>
    /// <param name="request">The image extraction request containing the image blob to extract text and images from.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the <see cref="ImageExtractionResponse"/>.</returns>
    Task<ImageExtractionResponse> Extract(ImageExtractionRequest request, CancellationToken cancellationToken = default);
}