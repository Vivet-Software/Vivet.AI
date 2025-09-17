using System;
using System.Collections.Generic;
using System.Linq;

namespace Vivet.AI.Services.Models.MimeTypes;

/// <summary>
/// Represents a document MIME type and provides predefined instances for common document formats.
/// </summary>
public sealed class DocumentMimeType : BaseMimeType
{
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="DocumentMimeType"/> class with the specified MIME type value.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    private DocumentMimeType(string value)
        : base(value)
    {
    }

    /// <summary>
    /// Represents the "text/plain" MIME type.
    /// </summary>
    public static readonly DocumentMimeType Plain = new("text/plain");

    /// <summary>
    /// Represents the "text/html" MIME type.
    /// </summary>
    public static readonly DocumentMimeType Html = new("text/html");

    /// <summary>
    /// Represents the "application/xhtml+xml" MIME type.
    /// </summary>
    public static readonly DocumentMimeType Xhtml = new("application/xhtml+xml");

    /// <summary>
    /// Represents the "application/json" MIME type.
    /// </summary>
    public static readonly DocumentMimeType Json = new("application/json");

    /// <summary>
    /// Represents the "application/pdf" MIME type.
    /// </summary>
    public static readonly DocumentMimeType Pdf = new("application/pdf");

    /// <summary>
    /// Gets a read-only list of all predefined <see cref="DocumentMimeType"/> instances.
    /// </summary>
    public static IReadOnlyList<DocumentMimeType> All = new List<DocumentMimeType>
    {
        Plain,
        Html,
        Xhtml,
        Json,
        Pdf
    };

    /// <summary>
    /// Returns a <see cref="DocumentMimeType"/> instance that matches the specified MIME type string.
    /// </summary>
    /// <param name="value">The MIME type string value.</param>
    /// <returns>The matching <see cref="DocumentMimeType"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> does not match any predefined MIME type.</exception>
    public static DocumentMimeType FromValue(string value)
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
    /// Determines whether the specified object is equal to the current <see cref="DocumentMimeType"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) => obj is DocumentMimeType other && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a hash code for the current <see cref="DocumentMimeType"/>.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        // Normalize case for consistent hashing
        return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
    }

    /// <summary>
    /// Determines whether two <see cref="DocumentMimeType"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="DocumentMimeType"/>.</param>
    /// <param name="b">The second <see cref="DocumentMimeType"/>.</param>
    /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(DocumentMimeType a, DocumentMimeType b) => a?.Equals(b) ?? b is null;

    /// <summary>
    /// Determines whether two <see cref="DocumentMimeType"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="DocumentMimeType"/>.</param>
    /// <param name="b">The second <see cref="DocumentMimeType"/>.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(DocumentMimeType a, DocumentMimeType b) => !(a == b);
}