using Vivet.AI.Services.Models.Blobs;

namespace Vivet.AI.Services.Requests.Transcription;

/// <summary>
/// Represents a transcribe audio request.
/// </summary>
public class TranscribeAudioRequest : BaseTranscribeRequest<AudioBlob>;