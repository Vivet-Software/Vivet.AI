namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to memory blob metadata retrieval operations.
/// </summary>
public class EmbeddingMetadataConfigOverrides
{
    /// <summary>
    /// Ocerride the configuration for automatically to retrieve metadata for blobs.
    /// This will use the configured metadata chat model and incur costs.
    /// </summary>
    public virtual bool? UseAutomaticMetadataRetrieval { get; set; }

    /// <summary>
    /// The max word count for the metadata summary.
    /// The summary is vectorized and later used for searching blobs in the vector store.
    /// </summary>
    public virtual int SummaryMaxWords { get; set; } = 30;

    /// <summary>
    /// The max word count for the metadata description.
    /// The description is later passed to the chat model when the found by searching the vector store.
    /// </summary>
    public virtual int DescriptionMaxWords { get; set; } = 90;
}