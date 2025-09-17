using System;

namespace Vivet.AI.Services.Models;

/// <summary>
/// Represents token usage details, including input, output, and total tokens.
/// Provides safe handling of negative values and supports arithmetic operations.
/// </summary>
public class TokenUsage
{
    private long? inputTokens;
    private long? outputTokens;

    /// <summary>
    /// Gets or sets the number of input tokens.
    /// Negative values are automatically clamped to zero.
    /// </summary>
    public virtual long? InputTokens
    {
        get => inputTokens;
        set => inputTokens = value < 0 ? 0 : value;
    }

    /// <summary>
    /// Gets or sets the number of output tokens.
    /// Negative values are automatically clamped to zero.
    /// </summary>
    public virtual long? OutputTokens
    {
        get => outputTokens;
        set => outputTokens = value < 0 ? 0 : value;
    }

    /// <summary>
    /// Gets the total number of tokens, calculated as the sum of
    /// <see cref="InputTokens"/> and <see cref="OutputTokens"/>.
    /// </summary>
    public virtual long? TotalTokens => (this.InputTokens ?? 0) + (this.OutputTokens ?? 0);

    /// <summary>
    /// Adds two <see cref="TokenUsage"/> instances together.
    /// Input and output tokens are summed independently, and results are clamped to zero.
    /// </summary>
    /// <param name="a">The first <see cref="TokenUsage"/> instance.</param>
    /// <param name="b">The second <see cref="TokenUsage"/> instance.</param>
    /// <returns>A new <see cref="TokenUsage"/> representing the combined values.</returns>
    public static TokenUsage operator +(TokenUsage a, TokenUsage b)
    {
        return new TokenUsage
        {
            InputTokens = Math.Max((a?.InputTokens ?? 0) + (b?.InputTokens ?? 0), 0),
            OutputTokens = Math.Max((a?.OutputTokens ?? 0) + (b?.OutputTokens ?? 0), 0)
        };
    }

    /// <summary>
    /// Subtracts one <see cref="TokenUsage"/> instance from another.
    /// Input and output tokens are subtracted independently, and results are clamped to zero.
    /// </summary>
    /// <param name="a">The first <see cref="TokenUsage"/> instance.</param>
    /// <param name="b">The second <see cref="TokenUsage"/> instance to subtract from <paramref name="a"/>.</param>
    /// <returns>
    /// A new <see cref="TokenUsage"/> representing the difference.
    /// If <paramref name="a"/> is null and <paramref name="b"/> is not, the result will contain the values of <paramref name="b"/>.
    /// </returns>
    public static TokenUsage operator -(TokenUsage a, TokenUsage b)
    {
        if (a == null && b != null)
        {
            return new TokenUsage
            {
                InputTokens = b.InputTokens ?? 0,
                OutputTokens = b.OutputTokens ?? 0
            };
        }

        return new TokenUsage
        {
            InputTokens = Math.Max((a?.InputTokens ?? 0) - (b?.InputTokens ?? 0), 0),
            OutputTokens = Math.Max((a?.OutputTokens ?? 0) - (b?.OutputTokens ?? 0), 0)
        };
    }
}