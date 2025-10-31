using Vivet.AI.Services.Models.Blobs;

namespace Vivet.AI.Services.Requests.Transcription;

/// <summary>
/// Represents a transcribe video request.
/// </summary>
public class TranscribeVideoRequest : BaseTranscribeRequest<VideoBlob>;