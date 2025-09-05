namespace Vivet.AI.Models.Enums;

/// <summary>
/// Defines how much reasoning effort a model or operation should apply.
/// </summary>
public enum ReasoningEffort
{
    /// <summary>
    /// Lightweight reasoning, fast and cost-efficient.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Balanced reasoning between performance and depth.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// Deep reasoning, slower but higher accuracy.
    /// </summary>
    High = 2
}