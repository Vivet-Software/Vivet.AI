using System;
using System.Collections.Generic;
using System.Linq;

namespace Vivet.AI.Services.Models.MimeTypes;

/// <summary>
/// Represents a MIME type and provides predefined instances for common text, image, audio, and video formats.
/// </summary>
public class MimeType : BaseMimeType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MimeType"/> class with the specified MIME type value.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    protected MimeType(string value)
        : base(value)
    {
    }

    /// <summary>
    /// Represents the "text/plain" MIME type.
    /// </summary>
    public static readonly MimeType Plain = new("text/plain");

    /// <summary>
    /// Represents the "text/html" MIME type.
    /// </summary>
    public static readonly MimeType Html = new("text/html");

    /// <summary>
    /// Represents the "application/xhtml+xml" MIME type.
    /// </summary>
    public static readonly MimeType Xhtml = new("application/xhtml+xml");

    /// <summary>
    /// Represents the "application/json" MIME type.
    /// </summary>
    public static readonly MimeType Json = new("application/json");

    /// <summary>
    /// Represents the "application/pdf" MIME type.
    /// </summary>
    public static readonly MimeType Pdf = new("application/pdf");

    /// <summary>
    /// Represents the "image/jpeg" MIME type.
    /// </summary>
    public static readonly MimeType Jpg = new("image/jpeg");

    /// <summary>
    /// Represents the "image/png" MIME type.
    /// </summary>
    public static readonly MimeType Png = new("image/png");

    /// <summary>
    /// Represents the "audio/mpeg" MIME type.
    /// </summary>
    public static readonly MimeType Mp3 = new("audio/mpeg");

    /// <summary>
    /// Represents the "audio/mp4" MIME type.
    /// </summary>
    public static readonly MimeType M4A = new("audio/mp4");

    /// <summary>
    /// Represents the "audio/wav" MIME type.
    /// </summary>
    public static readonly MimeType Wav = new("audio/wav");

    /// <summary>
    /// Represents the "audio/x-wav" MIME type.
    /// </summary>
    public static readonly MimeType Wavx = new("audio/x-wav");

    /// <summary>
    /// Represents the "audio/aac" MIME type.
    /// </summary>
    public static readonly MimeType Aac = new("audio/aac");

    /// <summary>
    /// Represents the "audio/ogg" MIME type.
    /// </summary>
    public static readonly MimeType Ogg = new("audio/ogg");

    /// <summary>
    /// Represents the "video/mp4" MIME type.
    /// </summary>
    public static readonly MimeType Mp4 = new("video/mp4");

    /// <summary>
    /// Represents the "video/webm" MIME type.
    /// </summary>
    public static readonly MimeType Webm = new("video/webm");

    /// <summary>
    /// Represents the "video/x-msvideo" MIME type (AVI format).
    /// </summary>
    public static readonly MimeType Avi = new("video/x-msvideo");

    /// <summary>
    /// Represents the "video/quicktime" MIME type (MOV format).
    /// </summary>
    public static readonly MimeType Mov = new("video/quicktime");

    /// <summary>
    /// Represents the "video/x-matroska" MIME type (MKV format).
    /// </summary>
    public static readonly MimeType Mkv = new("video/x-matroska");

    /// <summary>
    /// Gets a read-only list of all predefined <see cref="MimeType"/> instances.
    /// </summary>
    public static IReadOnlyList<MimeType> All = new List<MimeType>
    {
        Plain, Html, Xhtml, Json, Pdf, Jpg, Png,
        Mp3, M4A, Wav, Wavx, Aac, Ogg,
        Mp4, Webm, Avi, Mov, Mkv
    };

    /// <summary>
    /// Returns a <see cref="MimeType"/> instance that matches the specified MIME type string.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    /// <returns>The matching <see cref="MimeType"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> does not match any predefined MIME type.</exception>
    public static MimeType FromValue(string value)
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
    /// Determines whether the specified object is equal to the current <see cref="MimeType"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) => obj is MimeType other && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a hash code for the current <see cref="MimeType"/>.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // Normalize case for consistent hashing
        return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
    }

    /// <summary>
    /// Determines whether two <see cref="MimeType"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="MimeType"/>.</param>
    /// <param name="b">The second <see cref="MimeType"/>.</param>
    /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(MimeType a, MimeType b) => a?.Equals(b) ?? b is null;

    /// <summary>
    /// Determines whether two <see cref="MimeType"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="MimeType"/>.</param>
    /// <param name="b">The second <see cref="MimeType"/>.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(MimeType a, MimeType b) => !(a == b);
}