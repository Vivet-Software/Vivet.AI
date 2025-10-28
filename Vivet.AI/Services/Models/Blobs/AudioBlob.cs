using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents a blob containing audio data with metadata of type <see cref="AudioMimeType"/>.
/// </summary>
public class AudioBlob : BaseBlob<AudioMimeType>;