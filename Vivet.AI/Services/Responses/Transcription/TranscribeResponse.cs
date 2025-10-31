using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Services.Responses.Transcription.Models;

namespace Vivet.AI.Services.Responses.Transcription;

/// <summary>
/// Represents the response related to the transcribe operation.
/// </summary>
public class TranscribeResponse : BaseResponse
{
    /// <summary>
    /// The transcribed texts.
    /// </summary>
    public virtual IEnumerable<TranscribedText> Texts { get; set; } = [];

    /// <summary>
    /// The total duration of the transcribed audio file.
    /// </summary>
    public virtual TimeSpan? TotalDuration => this.Texts
        .Aggregate(TimeSpan.Zero, (sum, x) => sum + x.Duration.GetValueOrDefault());
}