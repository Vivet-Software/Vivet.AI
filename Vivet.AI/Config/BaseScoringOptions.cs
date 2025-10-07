using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Enums;
using Vivet.AI.Models.Enums;

namespace Vivet.AI.Config;

/// <summary>
/// Base class for controlling recency-based score boosts for search results.
/// Supports configurable decay strategies (Linear, Exponential, Sigmoid).
/// </summary>
public abstract class BaseScoringOptions
{
    /// <summary>
    /// The threshold for cosinus similarity marching.
    /// The threshold value highly depends on the chosen embedding model and preference.
    /// A higher match score mean a greater semantic match.
    /// 0.00 - 0.70: Often noise, unless your domain is very narrow.
    /// 0.70 - 0.80: Related but not identical. (useful for brainstorming or looser recall).
    /// 0.80 – 0.85: Good semantic match (typical retrieval threshold).
    /// 0.90+: Very strong / near-duplicate matches.
    /// </summary>
    [Required]
    public virtual double MatchScoreThreshold { get; set; } = 0.86;

    /// <summary>
    /// The matchs score threshold for deduplicating similar memory results,
    /// when building the memory part of the chat prompt.
    /// </summary>
    [Required]
    public virtual double DeduplicationMatchScoreThreshold { get; set; } = 0.90;

    /// <summary>
    /// Strategy used to calculate how recency score decays over time.
    /// </summary>
    [Required]
    public virtual RecencyDecayStrategy RecencyDecayStrategy { get; set; } = RecencyDecayStrategy.Linear;

    /// <summary>
    /// The maximum boost for a result with age = 0 days (i.e., most recent).
    /// </summary>
    [Required]
    public virtual double RecencyBoostMax { get; set; } = 0.1D;

    /// <summary>
    /// Controls the decay period for the recency boost.
    /// - For Linear/Exponential: number of days until the boost becomes negligible.
    /// - For Sigmoid: used as the 'midpoint' where boost is ~50%.
    /// </summary>
    [Required]
    public virtual double RecencyDecayDays { get; set; } = 30;

    /// <summary>
    /// Used only for Sigmoid strategy. Controls how steep the decay is around the midpoint.
    /// Lower values = sharper drop.
    /// </summary>
    [Required]
    public virtual double RecencySigmoidSteepness { get; set; } = 1.0;
}