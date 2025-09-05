namespace Vivet.AI.Config;

/// <summary>
/// Memory Scoring Options.
/// </summary>
public class MemoryScoringOptions : BaseScoringOptions
{
    /// <summary>
    /// The score boost applied to memories that matches the current conversation thread.
    /// </summary>
    public virtual double ThreadMatchBoost { get; set; } = 0.2F;
}