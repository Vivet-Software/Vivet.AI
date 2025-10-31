using Vivet.AI.Services.Models.Blobs;

namespace Vivet.AI.Services.Requests.Vision;

/// <summary>
/// Represents a document text extraction request.
/// </summary>
public class DocumentTextExtractionRequest : BaseTextExtractionRequest<DocumentBlob>;