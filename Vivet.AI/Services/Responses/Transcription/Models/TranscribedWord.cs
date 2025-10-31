using System;

namespace Vivet.AI.Services.Responses.Transcription.Models;

/// <summary>
/// Represents a word in a transcribed text.
/// </summary>
public class TranscribedWord
{
    /// <summary>
    /// The word in the audio file.
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
    /// The order of the segment in the transcribed text.
    /// </summary>
    public virtual int Order { get; set; } = 0;

    /// <summary>
    /// The duration of the word.
    /// </summary>
    public virtual TimeSpan? Duration => this.EndTime - this.StartTime;
}