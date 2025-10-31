using Vivet.AI.Services.Models.Blobs;

namespace Vivet.AI.Services.Requests.Vision;

/// <summary>
/// Abstract base class representing an images extraction request.
/// </summary>
/// <typeparam name="T">The type of blob.</typeparam>
public abstract class BaseImagesExtractionRequest<T> : BaseVisionRequest<T>
    where T : BaseBlob;