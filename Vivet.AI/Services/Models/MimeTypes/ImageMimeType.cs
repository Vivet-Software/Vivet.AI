using System;
using System.Collections.Generic;
using System.Linq;

namespace Vivet.AI.Services.Models.MimeTypes;

/// <summary>
/// Represents an image MIME type and provides predefined instances for common image formats.
/// </summary>
public sealed class ImageMimeType : BaseMimeType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageMimeType"/> class with the specified MIME type value.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    private ImageMimeType(string value)
        : base(value)
    {
    }

    /// <summary>
    /// Represents the "image/jpeg" MIME type.
    /// </summary>
    public static readonly ImageMimeType Jpg = new("image/jpeg");

    /// <summary>
    /// Represents the "image/png" MIME type.
    /// </summary>
    public static readonly ImageMimeType Png = new("image/png");

    /// <summary>
    /// Gets a read-only list of all predefined <see cref="ImageMimeType"/> instances.
    /// </summary>
    public static IReadOnlyList<ImageMimeType> All = new List<ImageMimeType>
    {
        Jpg,
        Png
    };

    /// <summary>
    /// Returns an <see cref="ImageMimeType"/> instance that matches the specified MIME type string.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    /// <returns>The matching <see cref="ImageMimeType"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> does not match any predefined MIME type.</exception>
    public static ImageMimeType FromValue(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var mimeType = All
            .FirstOrDefault(m => m.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

        if (mimeType == null)
        {
            throw new ArgumentException($"Unsupported MIME type: {value}");
        }

        return mimeType;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="ImageMimeType"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) => obj is ImageMimeType other && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a hash code for the current <see cref="ImageMimeType"/>.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // Normalize case for consistent hashing
        return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
    }

    /// <summary>
    /// Determines whether two <see cref="ImageMimeType"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="ImageMimeType"/>.</param>
    /// <param name="b">The second <see cref="ImageMimeType"/>.</param>
    /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(ImageMimeType a, ImageMimeType b) => a?.Equals(b) ?? b is null;

    /// <summary>
    /// Determines whether two <see cref="ImageMimeType"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="ImageMimeType"/>.</param>
    /// <param name="b">The second <see cref="ImageMimeType"/>.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ImageMimeType a, ImageMimeType b) => !(a == b);
}