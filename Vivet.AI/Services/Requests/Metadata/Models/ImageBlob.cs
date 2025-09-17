using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Requests.Metadata.Models;

/// <summary>
/// Represents a blob containing image data with metadata of type <see cref="ImageMimeType"/>.
/// </summary>
public class ImageBlob : BaseBlob<ImageMimeType>;