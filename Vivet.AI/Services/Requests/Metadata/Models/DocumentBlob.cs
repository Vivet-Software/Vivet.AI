using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Requests.Metadata.Models;

/// <summary>
/// Represents a blob containing document data with metadata of type <see cref="DocumentMimeType"/>.
/// </summary>
public class DocumentBlob : BaseBlob<DocumentMimeType>;