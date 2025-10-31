using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Transcription;
using Vivet.AI.Services.Responses.Transcription;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Transcription service interface.
/// </summary>
public interface ITranscriptionService
{
    /// <summary>
    /// Transcribes audio into text.
    /// </summary>
    /// <param name="request">The transcribe request containing the audio blob to transcribe.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the <see cref="TranscribeResponse"/>.</returns>
    Task<TranscribeResponse> Transcribe<T>(BaseTranscribeRequest<T> request, CancellationToken cancellationToken = default)
        where T : BaseBlob;
}