using Vivet.AI.Services.Models.Blobs;

namespace Vivet.AI.Services.Requests.Vision;

/// <summary>
/// Abstract base class representing a text extraction request.
/// </summary>
/// <typeparam name="T">The type of blob.</typeparam>
public abstract class BaseTextExtractionRequest<T> : BaseVisionRequest<T>
    where T : BaseBlob;