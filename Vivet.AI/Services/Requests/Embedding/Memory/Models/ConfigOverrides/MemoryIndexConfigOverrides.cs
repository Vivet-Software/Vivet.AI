using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Metadata.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Summarization.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to memory index operations.
/// </summary>
public class MemoryIndexConfigOverrides : BaseIndexConfigOverrides
{
    /// <summary>
    /// Use extended context when retrieving memories.
    /// This will enable counterpart lookups of questions and answers. When the current question matches a previously memorized question,
    /// the corresponding memorized answer is searched. If a relevant match is found between the memorized question and its answer,
    /// the matching part of the answer is included in the context.
    /// Basically, the LLM will know what answer it gave last time you asked a similar question, and the other way around for answers.
    /// Enabling this will inrich the chat context and imrove accuracy and precision, but will also use more input tokens. 
    /// </summary>
    public virtual bool? UseExtendedMemoryContext { get; set; }

    /// <summary>
    /// Automatically summarize questions and answers.
    /// This will use the configured summarization chat model and incur costs.
    /// </summary>
    public virtual bool? UseAutomaticSummarization { get; set; }

    /// <summary>
    /// Metadata retrieval overrides.
    /// </summary>
    [Required]
    public virtual MetadataConfigOverrides Metadata { get; internal set; } = new();

    /// <summary>
    /// Summarization overrides.
    /// </summary>
    [Required]
    public virtual SummarizationConfigOverrides Summarization { get; internal set; } = new();
}