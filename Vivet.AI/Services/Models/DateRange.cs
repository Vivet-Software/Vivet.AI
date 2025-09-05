using System;

namespace Vivet.AI.Services.Models;

/// <summary>
/// Represents a range of dates using nullable start and end points.
/// </summary>
public class DateRange
{
    /// <summary>
    /// Gets or sets the starting date and time of the range.
    /// Can be <c>null</c> if the range has no defined start.
    /// </summary>
    public virtual DateTimeOffset? FromAt { get; set; }

    /// <summary>
    /// Gets or sets the ending date and time of the range.
    /// Can be <c>null</c> if the range has no defined end.
    /// </summary>
    public virtual DateTimeOffset? ToAt { get; set; }
}