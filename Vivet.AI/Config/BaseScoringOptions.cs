using Vivet.AI.Config.Enums;

namespace Vivet.AI.Config;

/// <summary>
/// Base class for controlling recency-based score boosts for search results.
/// Supports configurable decay strategies (Linear, Exponential, Sigmoid).
/// </summary>
public abstract class BaseScoringOptions
{
    /// <summary>
    /// Strategy used to calculate how recency score decays over time.
    /// </summary>
    public virtual RecencyDecayStrategy RecencyDecayStrategy { get; set; } = RecencyDecayStrategy.Linear;

    /// <summary>
    /// The maximum boost for a result with age = 0 days (i.e., most recent).
    /// </summary>
    public virtual double RecencyBoostMax { get; set; } = 0.1D;

    /// <summary>
    /// Controls the decay period for the recency boost.
    /// - For Linear/Exponential: number of days until the boost becomes negligible.
    /// - For Sigmoid: used as the 'midpoint' where boost is ~50%.
    /// </summary>
    public virtual double RecencyDecayDays { get; set; } = 30;

    /// <summary>
    /// Used only for Sigmoid strategy. Controls how steep the decay is around the midpoint.
    /// Lower values = sharper drop.
    /// </summary>
    public virtual double RecencySigmoidSteepness { get; set; } = 1.0;
}