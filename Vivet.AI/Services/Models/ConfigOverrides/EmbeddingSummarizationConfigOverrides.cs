using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Embedding.Memory.Models;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to embedding summarization operations.
/// </summary>
public class EmbeddingSummarizationConfigOverrides
{
    /// <summary>
    /// Override whether to automatically summarize questions and answers.
    /// Any inline JSON or XML will not be summarized, but left as is.
    /// This will use the configured summarization chat model and incur costs.
    /// </summary>
    public virtual bool? UseAutomaticSummarization { get; set; }

    /// <summary>
    /// The degree of summarization (0 - 100).
    /// Higher values means higher compression and less precision.
    /// 0: No summarization.
    /// 25: Preserve nearly all details, only remove fluff.,
    /// 50: Keep core meaning but make it more concise.,
    /// 75: Summarize concisely and remove non-essential details.,
    /// 100: Compress the content to its most essential ideas only.
    /// Only in effect if <see cref="MemoryConfigOverrides"/> has been configured and <see cref="UseAutomaticSummarization"/> is set to true.
    /// </summary>
    [Range(0, 100)]
    public virtual int? SummarizationDegree { get; set; }
}