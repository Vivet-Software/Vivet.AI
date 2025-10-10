using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Represents configuration overrides specific to memory index operations.
/// </summary>
public class MemoryIndexOptions
{
    /// <summary>
    /// Use extended context when retrieving memories.
    /// This will enable counterpart lookups of questions and answers. When the current question matches a previously memorized question,
    /// the corresponding memorized answer is searched. If a relevant match is found between the memorized question and its answer,
    /// the matching part of the answer is included in the context.
    /// Basically, the LLM will know what answer it gave last time you asked a similar question, and the other way around for answers.
    /// Enabling this will inrich the chat context and imrove accuracy and precision, but will also use more input tokens. 
    /// </summary>
    [Required]
    public virtual bool UseExtendedMemoryContext { get; set; } = true;

    /// <summary>
    /// Configuration for automatically to retrieve metadata for blobs when saving to memory.
    /// This will use the configured metadata chat model and incur costs.
    /// It's recommended to enable this, in order to ensure meaningful data for similarity comparison when the memory is later queried.
    /// If disabled metadata must be passed alongisde the blob when invoking the index request.
    /// </summary>
    [Required]
    public virtual bool UseAutomaticMetadataRetrieval { get; set; } = true;

    /// <summary>
    /// Automatically summarize questions and answers.
    /// This will use the configured summarization chat model and incur costs.
    /// </summary>
    [Required]
    public virtual bool UseAutomaticSummarization { get; set; } = false;

    /// <summary>
    /// Options for text chunking.
    /// </summary>
    [Required]
    public virtual TextChunkingOptions TextChunking { get; set; } = new();
}