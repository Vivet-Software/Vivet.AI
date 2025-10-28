using System;
using System.Collections.Generic;
using System.Linq;

namespace Vivet.AI.Services.Models.MimeTypes;

/// <summary>
/// Represents an audio MIME type and provides predefined instances for common audio formats.
/// </summary>
public sealed class AudioMimeType : BaseMimeType
{
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="AudioMimeType"/> class with the specified MIME type value.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    private AudioMimeType(string value)
        : base(value)
    {
    }

    /// <summary>
    /// Represents the "audio/mpeg" MIME type (MP3 format).
    /// </summary>
    public static readonly AudioMimeType Mp3 = new("audio/mpeg");

    /// <summary>
    /// Represents the "audio/mp4" MIME type (M4A format).
    /// </summary>
    public static readonly AudioMimeType M4a = new("audio/mp4");

    /// <summary>
    /// Represents the "audio/wav" MIME type.
    /// </summary>
    public static readonly AudioMimeType Wav = new("audio/wav");

    /// <summary>
    /// Represents the "audio/x-wav" MIME type.
    /// </summary>
    public static readonly AudioMimeType Wavx = new("audio/x-wav");

    /// <summary>
    /// Represents the "audio/aac" MIME type.
    /// </summary>
    public static readonly AudioMimeType Aac = new("audio/aac");

    /// <summary>
    /// Represents the "audio/ogg" MIME type.
    /// </summary>
    public static readonly AudioMimeType Ogg = new("audio/ogg");

    /// <summary>
    /// Gets a read-only list of all predefined <see cref="AudioMimeType"/> instances.
    /// </summary>
    public static IReadOnlyList<AudioMimeType> All = new List<AudioMimeType>
    {
        Mp3,
        M4a,
        Wav,
        Wavx,
        Aac,
        Ogg
    };

    /// <summary>
    /// Returns an <see cref="AudioMimeType"/> instance that matches the specified MIME type string.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    /// <returns>The matching <see cref="AudioMimeType"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> does not match any predefined MIME type.</exception>
    public static AudioMimeType FromValue(string value)
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
    /// Determines whether the specified object is equal to the current <see cref="AudioMimeType"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) => obj is AudioMimeType other && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a hash code for the current <see cref="AudioMimeType"/>.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // Normalize case for consistent hashing
        return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
    }

    /// <summary>
    /// Determines whether two <see cref="AudioMimeType"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="AudioMimeType"/>.</param>
    /// <param name="b">The second <see cref="AudioMimeType"/>.</param>
    /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(AudioMimeType a, AudioMimeType b) => a?.Equals(b) ?? b is null;

    /// <summary>
    /// Determines whether two <see cref="AudioMimeType"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="AudioMimeType"/>.</param>
    /// <param name="b">The second <see cref="AudioMimeType"/>.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(AudioMimeType a, AudioMimeType b) => !(a == b);
}