using System.ComponentModel.DataAnnotations;
using Vivet.AI.Models;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Summarization.Models;

/// <summary>
/// Represents configuration overrides specific to summarization operations.
/// </summary>
public class SummarizationConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the model parameters to use for the summarization.
    /// </summary>
    public virtual ChatModelParameters ModelParameters { get; set; }

    /// <summary>
    /// Gets or sets the degree of summarization.
    /// Higher values mean higher compression and less precision.
    /// 0: No summarization.
    /// 25: Preserve nearly all details, only remove fluff.
    /// 50: Keep core meaning but make it more concise.
    /// 75: Summarize concisely and remove non-essential details.
    /// 100: Compress the content to its most essential ideas only.
    /// </summary>
    [Range(0, 100)]
    public virtual int? SummarizationDegree { get; set; }
}