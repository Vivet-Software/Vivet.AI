using Microsoft.CSharp.RuntimeBinder;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Services.Responses.Transcription.Models;

namespace Vivet.AI.Services.Extensions;

internal static class TextContentExtensions
{
    internal static string GetLanguage(this TextContent textContent)
    {
        if (textContent == null)
            throw new ArgumentNullException(nameof(textContent));

        var innerContentLanguage = textContent.InnerContent?
            .TryGetPropertyValue<string>("Language");

        if (innerContentLanguage != null)
        {
            return innerContentLanguage;
        }

        if (textContent.Metadata == null)
        {
            return null;
        }

        if (!textContent.Metadata.TryGetValue("Language", out var value) || value == null)
        {
            return null;
        }

        try
        {
            return value as string;
        }
        catch (RuntimeBinderException)
        {
            return null;
        }
    }

    internal static TimeSpan? GetDuration(this TextContent textContent)
    {
        if (textContent == null)
            throw new ArgumentNullException(nameof(textContent));

        var innerContentDuration = textContent.InnerContent?
            .TryGetPropertyValue<TimeSpan?>("Duration");

        if (innerContentDuration != null)
        {
            return innerContentDuration;
        }

        if (textContent.Metadata == null)
        {
            return null;
        }

        if (!textContent.Metadata.TryGetValue("Duration", out var value) || value == null)
        {
            return null;
        }

        try
        {
            var strValue = value.ToString();

            return strValue == null
                ? null
                : TimeSpan.Parse(strValue);
        }
        catch (RuntimeBinderException)
        {
            return null;
        }
    }

    internal static TranscribedSegment[] GetSegments(this TextContent textContent)
    {
        if (textContent == null)
            throw new ArgumentNullException(nameof(textContent));

        var segments = textContent.InnerContent?
            .TryGetEnumerableProperty("Segments");

        if (segments != null)
        {
            var words = textContent.InnerContent
                .TryGetEnumerableProperty("Words")
                .Select((x, i) =>
                {
                    var word = x
                        .TryGetPropertyValue<string>("Word");

                    var startTime = x
                        .TryGetPropertyValue<TimeSpan?>("StartTime");

                    var endTime = x
                        .TryGetPropertyValue<TimeSpan?>("EndTime");

                    return new TranscribedWord
                    {
                        Content = word.Trim(),
                        StartTime = startTime,
                        EndTime = endTime,
                        Order = i
                    };
                })
                .ToArray();

            return segments
                .Select((x, i) =>
                {
                    var content = x
                        .TryGetPropertyValue<string>("Text");

                    var startTime = x
                        .TryGetPropertyValue<TimeSpan?>("StartTime");

                    var endTime = x
                        .TryGetPropertyValue<TimeSpan?>("EndTime");

                    var compressionRatio = x
                        .TryGetPropertyValue<double?>("CompressionRatio");

                    return new TranscribedSegment
                    {
                        Content = content.Trim(),
                        StartTime = startTime,
                        EndTime = endTime,
                        CompressionRatio = compressionRatio,
                        Order = i,
                        Words = words
                            .Where(y => y.StartTime >= startTime && y.EndTime <= endTime)
                    };
                })
                .ToArray();
        }

        if (textContent.Metadata == null)
        {
            return [];
        }

        try
        {
            if (textContent.Metadata != null && textContent.Metadata.TryGetValue("segments", out var rawSegments) && rawSegments is IEnumerable<object> segmentList)
            {
                return segmentList
                    .Select((x, i) =>
                    {
                        dynamic segment = x;

                        return new TranscribedSegment
                        {
                            Content = segment.Text,
                            StartTime = segment.StartTime,
                            EndTime = segment.EndTime,
                            CompressionRatio = segment.CompressionRatio,
                            Order = i
                        };
                    })
                    .ToArray();
            }

            var duration = textContent
                .GetDuration();

            return
            [
                new TranscribedSegment
                {
                    Content = textContent.Text,
                    StartTime = TimeSpan.Zero,
                    EndTime = duration,
                    Order = 0
                }
            ];
        }
        catch (RuntimeBinderException)
        {
            return [];
        }
    }
}