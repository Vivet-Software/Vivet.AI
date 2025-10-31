using System;
using System.Collections.Generic;

namespace Vivet.AI.Services.Responses.Transcription.Models;

/// <summary>
/// Represents a segment in a transcribed text.
/// </summary>
public class TranscribedSegment
{
    /// <summary>
    /// The content of the transcribed text.
    /// </summary>
    public virtual string Content { get; set; }

    /// <summary>
    /// The start time in the audio file.
    /// </summary>
    public virtual TimeSpan? StartTime { get; set; }

    /// <summary>
    /// The end time in the audio file.
    /// </summary>
    public virtual TimeSpan? EndTime { get; set; }

    /// <summary>
    /// The compression ratio of the audio segment.
    /// </summary>
    public virtual double? CompressionRatio { get; set; }

    /// <summary>
    /// The order of the segment in the transcribed text.
    /// </summary>
    public virtual int Order { get; set; } = 0;

    /// <summary>
    /// The duration of the segment.
    /// </summary>
    public virtual TimeSpan? Duration => this.EndTime - this.StartTime;

    /// <summary>
    /// The individual words of the transcribed text.
    /// </summary>
    public virtual IEnumerable<TranscribedWord> Words { get; set; } = [];
}