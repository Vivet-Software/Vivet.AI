namespace Vivet.AI.Services.Responses.Metadata;

/// <summary>
/// Represents a response containing metadata.
/// </summary>
public class MetadataResponse : BaseResponse
{
    /// <summary>
    /// The extracted metadata.
    /// </summary>
    public virtual Models.Metadata Metadata { get; set; }
}

/// <summary>
/// Represents a response containing metadata along with additional typed metadata.
/// </summary>
/// <typeparam name="T">The type of additional metadata.</typeparam>
public class MetadataResponse<T> : MetadataResponse
    where T : class, new()
{
    /// <summary>
    /// Additional metadata of type <typeparamref name="T"/>.
    /// </summary>
    public virtual T AdditionalMetadata { get; set; }
}