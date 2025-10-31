using System;
using System.Collections.Generic;
using System.Linq;

namespace Vivet.AI.Services.Responses.Transcription.Models;

/// <summary>
/// Represents the transcribed text related to transcription operations.
/// </summary>
public class TranscribedText
{
    private TimeSpan? duration;

    /// <summary>
    /// The content of the transcribed text.
    /// </summary>
    public virtual string Content { get; set; }

    /// <summary>
    /// The start time in the audio file.
    /// </summary>
    public virtual TimeSpan StartTime { get; set; }

    /// <summary>
    /// The end time in the audio file.
    /// </summary>
    public virtual TimeSpan? EndTime { get; set; }

    /// <summary>
    /// The language of the transcribed text.
    /// </summary>
    public virtual string Language { get; set; }

    /// <summary>
    /// The duration of the transcribed audio.
    /// </summary>
    public virtual TimeSpan? Duration
    {
        get
        {
            if (this.duration.HasValue)
            {
                return this.duration.Value;
            }

            return this.Segments
                .Aggregate(TimeSpan.Zero, (sum, x) => sum + x.Duration.GetValueOrDefault());
        }
        set => this.duration = value;
    }

    /// <summary>
    /// The individual segments of the transcribed text.
    /// </summary>
    public virtual IEnumerable<TranscribedSegment> Segments { get; set; } = [];
}