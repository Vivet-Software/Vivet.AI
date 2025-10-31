using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents a blob containing audio data with metadata of type <see cref="AudioMimeType"/>.
/// </summary>
public class AudioBlobMetadata : BaseBlobMetadata<AudioMimeType>;

/// <summary>
/// Represents a blob containing audio data with metadata of type <see cref="AudioMimeType"/> 
/// and additional custom metadata of type <typeparamref name="TMetadata"/>.
/// </summary>
/// <typeparam name="TMetadata">The type of additional metadata. Must be a reference type with a parameterless constructor.</typeparam>
public class AudioBlob<TMetadata> : BaseBlobAdditionalMetadata<AudioMimeType, TMetadata>
    where TMetadata : class, new();