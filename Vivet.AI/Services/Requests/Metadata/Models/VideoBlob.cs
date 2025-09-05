using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Requests.Metadata.Models;

/// <summary>
/// Represents a blob containing video data with metadata of type <see cref="VideoMimeType"/>.
/// </summary>
public class VideoBlob : BaseBlob<VideoMimeType>;