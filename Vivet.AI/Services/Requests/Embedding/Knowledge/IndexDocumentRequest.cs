using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge;

/// <summary>
/// Represents a request to index document blobs with optional knowledge blob configuration overrides.
/// </summary>
public class IndexDocumentRequest : BaseIndexBlobRequest<DocumentMimeType>;
