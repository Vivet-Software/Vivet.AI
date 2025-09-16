using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Summarization Options.
/// </summary>
public class SummarizationOptions
{
    /// <summary>
    /// The chat model to use for summarization.
    /// </summary>
    [Required]
    public virtual ChatModel Model { get; set; } = new();

    /// <summary>
    /// The degree of summarization (0 - 100).
    /// Higher values means higher compression and less precision.
    /// 0: No summarization.
    /// 25: Preserve nearly all details, only remove fluff.,
    /// 50: Keep core meaning but make it more concise.,
    /// 75: Summarize concisely and remove non-essential details.,
    /// 100: Compress the content to its most essential ideas only.
    /// </summary>
    [Required]
    [Range(0, 100)]
    public virtual int SummarizationDegree { get; set; } = 25;

    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
}