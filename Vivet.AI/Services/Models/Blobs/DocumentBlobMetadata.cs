using Vivet.AI.Services.Models.MimeTypes;

namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents a blob containing document data with metadata of type <see cref="DocumentMimeType"/>.
/// </summary>
public class DocumentBlobMetadata : BaseBlobMetadata<DocumentMimeType>;

/// <summary>
/// Represents a blob containing document data with metadata of type <see cref="DocumentMimeType"/> 
/// and additional custom metadata of type <typeparamref name="TMetadata"/>.
/// </summary>
/// <typeparam name="TMetadata">The type of additional metadata. Must be a reference type with a parameterless constructor.</typeparam>
public class DocumentBlob<TMetadata> : BaseBlobAdditionalMetadata<DocumentMimeType, TMetadata>
    where TMetadata : class, new();