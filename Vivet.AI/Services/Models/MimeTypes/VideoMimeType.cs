using System;
using System.Collections.Generic;
using System.Linq;

namespace Vivet.AI.Services.Models.MimeTypes;

/// <summary>
/// Represents a video MIME type and provides predefined instances for common formats.
/// </summary>
public sealed class VideoMimeType : BaseMimeType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoMimeType"/> class with the specified MIME type value.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    private VideoMimeType(string value)
        : base(value)
    {
    }

    /// <summary>
    /// Represents the "video/mp4" MIME type.
    /// </summary>
    public static readonly VideoMimeType Mp4 = new("video/mp4");

    /// <summary>
    /// Represents the "video/webm" MIME type.
    /// </summary>
    public static readonly VideoMimeType Webm = new("video/webm");

    /// <summary>
    /// Represents the "video/x-msvideo" MIME type (AVI format).
    /// </summary>
    public static readonly VideoMimeType Avi = new("video/x-msvideo");

    /// <summary>
    /// Represents the "video/quicktime" MIME type (MOV format).
    /// </summary>
    public static readonly VideoMimeType Mov = new("video/quicktime");

    /// <summary>
    /// Represents the "video/x-matroska" MIME type (MKV format).
    /// </summary>
    public static readonly VideoMimeType Mkv = new("video/x-matroska");

    /// <summary>
    /// Gets a read-only list of all predefined <see cref="VideoMimeType"/> instances.
    /// </summary>
    public static IReadOnlyList<VideoMimeType> All = new List<VideoMimeType>
    {
        Mp4,
        Webm,
        Avi,
        Mov,
        Mkv
    };

    /// <summary>
    /// Returns a <see cref="VideoMimeType"/> instance that matches the specified MIME type string.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    /// <returns>The matching <see cref="VideoMimeType"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> does not match any predefined MIME type.</exception>
    public static VideoMimeType FromValue(string value)
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
    /// Determines whether the specified object is equal to the current <see cref="VideoMimeType"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) => obj is VideoMimeType other && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a hash code for the current <see cref="VideoMimeType"/>.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // Normalize case for consistent hashing
        return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
    }

    /// <summary>
    /// Determines whether two <see cref="VideoMimeType"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="VideoMimeType"/>.</param>
    /// <param name="b">The second <see cref="VideoMimeType"/>.</param>
    /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(VideoMimeType a, VideoMimeType b) => a?.Equals(b) ?? b is null;

    /// <summary>
    /// Determines whether two <see cref="VideoMimeType"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="VideoMimeType"/>.</param>
    /// <param name="b">The second <see cref="VideoMimeType"/>.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(VideoMimeType a, VideoMimeType b) => !(a == b);
}