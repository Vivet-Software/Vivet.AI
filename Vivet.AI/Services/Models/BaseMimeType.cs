using System;

namespace Vivet.AI.Services.Models;

/// <summary>
/// Represents a base type for MIME type values.
/// Designed to be inherited for specific MIME type implementations.
/// </summary>
public class BaseMimeType
{
    /// <summary>
    /// Gets the MIME type value as a string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMimeType"/> class
    /// with the specified MIME type value.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <c>null</c>.
    /// </exception>
    protected BaseMimeType(string value)
    {
        this.Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Returns the MIME type value as a string.
    /// </summary>
    /// <returns>The MIME type string value.</returns>
    public override string ToString() => this.Value;

    /// <summary>
    /// Returns a hash code for the MIME type value.
    /// The hash code is case-insensitive.
    /// </summary>
    /// <returns>A hash code representing this MIME type.</returns>
    public override int GetHashCode() => this.Value.ToLowerInvariant().GetHashCode();
}   