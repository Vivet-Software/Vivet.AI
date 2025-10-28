using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents a blob containing video data with metadata of type <see cref="VideoMimeType"/>.
/// </summary>
public class VideoBlobMetadata : BaseBlobMetadata<VideoMimeType>;

/// <summary>
/// Represents a blob containing video data with metadata of type <see cref="VideoMimeType"/> 
/// and additional custom metadata of type <typeparamref name="TMetadata"/>.
/// </summary>
/// <typeparam name="TMetadata">The type of additional metadata. Must be a reference type with a parameterless constructor.</typeparam>
public class VideoBlob<TMetadata> : BaseBlobAdditionalMetadata<VideoMimeType, TMetadata>
    where TMetadata : class, new();