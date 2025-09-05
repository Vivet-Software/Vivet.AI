namespace Vivet.AI.Config.Enums;

/// <summary>
/// Specifies the strategy used to calculate how the recency boost decays over time.
/// Different strategies model different decay curves to influence scoring behavior.
/// </summary>
public enum RecencyDecayStrategy
{
    /// <summary>
    /// Applies a <b>linear decay</b> to the recency boost.
    /// The score decreases evenly from the maximum boost down to zero over the configured decay period.
    /// <br />
    /// Formula: <code>RecencyBoost = max(0, RecencyBoostMax - (AgeInDays * (RecencyBoostMax / RecencyDecayDays)))</code>
    /// Use this when you want a simple, predictable fading of boost over time.
    /// </summary>
    Linear,

    /// <summary>
    /// Applies an <b>exponential decay</b> to the recency boost.
    /// The score decreases rapidly at first and then more slowly over time, following an exponential curve.
    /// <br />
    /// Formula: <code>RecencyBoost = RecencyBoostMax * exp(-AgeInDays / RecencyDecayDays)</code>
    /// Use this when recent items should have a strong boost that fades smoothly, but older items still retain some influence.
    /// </summary>
    Exponential,

    /// <summary>
    /// Applies a <b>sigmoid decay</b> curve to the recency boost.
    /// The score remains relatively flat around the midpoint of the decay period
    /// and drops off more sharply before and after this range.
    /// <br />
    /// Formula: <code>RecencyBoost = RecencyBoostMax / (1 + exp((AgeInDays - RecencyDecayDays) / SigmoidSteepness))</code>
    /// Use this when you want recent results to retain high boost for a time, then quickly lose influence after the midpoint.
    /// </summary>
    Sigmoid
}