namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to embedding index operations.
/// </summary>
public abstract class BaseEmbedingIndexConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the name of the model to use for this request, overriding the default configured model.
    /// The specified model must be supported by the registered orchestration; otherwise, the request may fail.
    /// </summary>
    public virtual string ModelName { get; set; }

    /// <summary>
    /// Configuration for automatically to retrieve metadata for blobs when saving to knowledge.
    /// This will use the configured metadata chat model and incur costs.
    /// It's recommended to enable this, in order to ensure meaningful data for similarity comparison when the memory is later queried.
    /// If disabled metadata must be passed alongisde the blob when invoking the index request.
    /// </summary>
    public virtual bool? UseAutomaticMetadataRetrieval { get; set; }

    /// <summary>
    /// Overrides for text chunking.
    /// </summary>
    public virtual BaseEmbedingIndexTextChunkingConfigOverrides TextChunking { get; internal set; } = new();
}