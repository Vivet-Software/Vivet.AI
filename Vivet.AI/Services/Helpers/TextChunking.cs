using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Services.Helpers.Enums;
using Vivet.AI.Services.Helpers.Models;

namespace Vivet.AI.Services.Helpers;

// TODO: Text Chunking: Real tokenizer integration
// TODO: Text Chunking: Streaming chunking for huge texts

internal static class TextChunking
{
    internal static TextChunk[] GetTextChunks(string text, int minTokensPerChunk, int maxTokensPerChunk)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        var paragraphId = 0;
        var allChunks = new List<TextChunk>();

        // Keep your original paragraph splitting
        var paragraphs = TextChunkingRegex.GetParagraphs(text);

        foreach (var paragraph in paragraphs)
        {
            paragraphId++;

            // NEW: extract mixed segments *inside* the paragraph
            var segments = TextChunking.ExtractSegmentsWithinParagraph(paragraph);

            // Build a single sentence list for this paragraph:
            // - text segments → run your sentence splitter
            // - json/xml segments → treat as one atomic "sentence"
            var sentencesForParagraph = new List<string>();
            foreach (var seg in segments)
            {
                if (seg.Type == SegmentType.Text)
                {
                    var sents = TextChunkingRegex.GetSentences(seg.Content);

                    sentencesForParagraph
                        .AddRange(sents);
                }
                else
                {
                    sentencesForParagraph
                        .Add(seg.Content);
                }
            }

            // Your original merge logic (unchanged)
            var paragraphChunks = TextChunking.MergeSentencesByDynamicTokenLimit(sentencesForParagraph, paragraphId, minTokensPerChunk, maxTokensPerChunk);

            allChunks
                .AddRange(paragraphChunks);
        }

        return allChunks.ToArray();
    }

    internal static string GetTextChunkNeighboringContext(TextChunk[] chunks, int index, int neighborCount, bool restrictToSameParagraph)
    {
        if (chunks == null)
            throw new ArgumentNullException(nameof(chunks));

        if (index < 0 || index >= chunks.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        var targetParagraphId = chunks[index].ParagraphId;
        var contextChunks = new List<string>();

        for (var offset = -neighborCount; offset <= neighborCount; offset++)
        {
            var neighborIndex = index + offset;

            if (neighborIndex < 0 || neighborIndex >= chunks.Length)
                continue;

            var neighbor = chunks[neighborIndex];

            if (restrictToSameParagraph && neighbor.ParagraphId != targetParagraphId)
                continue;

            contextChunks.Add(neighbor.Text);
        }

        return string.Join(" ", contextChunks);
    }


    private static List<Segment> ExtractSegmentsWithinParagraph(string paragraph)
    {
        if (paragraph == null) 
            throw new ArgumentNullException(nameof(paragraph));

        var lastIndex = 0;
        var segments = new List<Segment>();

        // Find JSON + XML blocks and order by position
        var matches = TextChunkingRegex.GetJsonMatches(paragraph)
            .Concat(TextChunkingRegex.GetXmlMatches(paragraph))
            .OrderBy(m => m.Index)
            .ToList();

        foreach (var match in matches)
        {
            // Text between lastIndex and this match → TEXT segment
            if (match.Index > lastIndex)
            {
                var between = paragraph
                    .Substring(lastIndex, match.Index - lastIndex);
                
                if (!string.IsNullOrWhiteSpace(between))
                {
                    segments
                        .Add(new Segment
                        {
                            Type = SegmentType.Text,
                            Content = between.Trim()
                        });
                }
            }

            // The matched block → JSON or XML segment
            var raw = match.Value;
            var type = raw.TrimStart().StartsWith("<") 
                ? SegmentType.Xml 
                : SegmentType.Json;
            
            segments
                .Add(new Segment
                {
                    Type = type,
                    Content = raw
                });

            lastIndex = match.Index + match.Length;
        }

        // Trailing text after the last match
        if (lastIndex < paragraph.Length)
        {
            var tail = paragraph[lastIndex..];
           
            if (!string.IsNullOrWhiteSpace(tail))
            {
                segments
                    .Add(new Segment
                    {
                        Type = SegmentType.Text,
                        Content = tail.Trim()
                    });
            }
        }

        // If no matches at all, the whole paragraph is TEXT
        if (segments.Count == 0)
        {
            segments
                .Add(new Segment
                {
                    Type = SegmentType.Text,
                    Content = paragraph.Trim()
                });
        }

        return segments;
    }
    private static List<TextChunk> MergeSentencesByDynamicTokenLimit(IEnumerable<string> sentences, int paragraphId, int minTokens, int maxTokens)
    {
        if (sentences == null)
            throw new ArgumentNullException(nameof(sentences));

        var currentTokenCount = 0;
        var chunks = new List<TextChunk>();
        var currentSentences = new List<string>();

        foreach (var sentence in sentences)
        {
            var sentenceTokens = TextChunkingRegex.GetTokenCount(sentence);

            // If current chunk is too small, keep adding sentences but do not exceed maxTokens unless chunk is empty
            var chunkTooSmall = currentTokenCount < minTokens;
            var addingFits = currentTokenCount + sentenceTokens <= maxTokens;

            if (!chunkTooSmall && !addingFits)
            {
                // Close off current chunk
                chunks.Add(new TextChunk
                {
                    Text = string.Join(" ", currentSentences),
                    TokenCount = currentTokenCount,
                    ParagraphId = paragraphId
                });

                currentSentences.Clear();
                currentTokenCount = 0;
            }

            // Handle very long "sentence" (e.g., a big JSON/XML block)
            if (sentenceTokens > maxTokens)
            {
                if (currentSentences.Count > 0)
                {
                    chunks.Add(new TextChunk
                    {
                        Text = string.Join(" ", currentSentences),
                        TokenCount = currentTokenCount,
                        ParagraphId = paragraphId
                    });

                    currentSentences.Clear();
                    currentTokenCount = 0;
                }

                // Add the long block as its own chunk (even if > maxTokens)
                chunks.Add(new TextChunk
                {
                    Text = sentence,
                    TokenCount = sentenceTokens,
                    ParagraphId = paragraphId
                });

                continue;
            }

            // Add sentence to current chunk
            currentSentences.Add(sentence);
            currentTokenCount += sentenceTokens;
        }

        // Add leftover chunk
        if (currentSentences.Count > 0)
        {
            chunks.Add(new TextChunk
            {
                Text = string.Join(" ", currentSentences),
                TokenCount = currentTokenCount,
                ParagraphId = paragraphId
            });
        }

        return chunks;
    }
}