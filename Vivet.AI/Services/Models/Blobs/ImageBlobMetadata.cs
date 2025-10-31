using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents a blob containing image data with metadata of type <see cref="ImageMimeType"/>.
/// </summary>
public class ImageBlobMetadata : BaseBlobMetadata<ImageMimeType>;

/// <summary>
/// Represents a blob containing image data with metadata of type <see cref="ImageMimeType"/> 
/// and additional custom metadata of type <typeparamref name="TMetadata"/>.
/// </summary>
/// <typeparam name="TMetadata">The type of additional metadata. Must be a reference type with a parameterless constructor.</typeparam>
public class ImageBlob<TMetadata> : BaseBlobAdditionalMetadata<ImageMimeType, TMetadata>
    where TMetadata : class, new();