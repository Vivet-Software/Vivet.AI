namespace Vivet.AI.Services.Models.Blobs;

/// <summary>
/// Represents a blob with a specific MIME type and additional custom metadata.
/// </summary>
/// <typeparam name="TMimeType">The type of MIME type. Must inherit from <see cref="BaseMimeType"/>.</typeparam>
/// <typeparam name="TMetadata">The type of additional metadata. Must be a reference type with a parameterless constructor.</typeparam>
public abstract class BaseBlobAdditionalMetadata<TMimeType, TMetadata> : BaseBlobMetadata<TMimeType>
    where TMimeType : BaseMimeType
    where TMetadata : class, new()
{
    /// <summary>
    /// Gets or sets additional custom metadata for the blob.
    /// </summary>
    public virtual TMetadata AdditionalMetadata { get; set; }
}