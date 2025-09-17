using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vivet.AI.Services.Helpers;

internal static class TextChunkingRegex
{
    private const int MIN_SENTENCE_WORD_COUNT = 5;

    private const string PUNCTUATION_PATTERN = "^[.!?]$";
    private const string PARAGRAPH_PATTERN = @"\r?\n\s*\r?\n";
    private const string SENTENCE_PATTERN = @"(?<=[.!?])\s+";
    private const string TOKEN_COUNT_PATTERN = @"\w+";
    private const string DECIMAL_NUMBER_PATTERN = @"^\d+\.\d+$";
    private const string LETTER_NUMBER_PATTERN = @"^[\p{L}]\.\d+$";
    private const string SHORT_ABBREVIATION_PATTERN = @"^[\p{L}]{1,4}\.$";
    private const string MULTIPLE_SPACED_INITIALS_PATTERN = @"(\b[\p{L}]\.\s+){1,}[\p{L}]\.$";
    private const string MULTIPLE_INITIALS_NO_SPACES_PATTERN = @"^(?:[\p{L}]\.){2,}$";
    private const string SINGLE_LETTER_INITIAL_PATTERN = @"^[\p{L}]\.$";
    private const string XML_PATTERN = @"<([A-Za-z_][\w\.\-:]*)(\s[^<>]*?)?>[\s\S]*?</\1\s*>|<([A-Za-z_][\w\.\-:]*)(\s[^<>]*?)?\s*/>";

    private static readonly Regex punctuationRegex = new(TextChunkingRegex.PUNCTUATION_PATTERN, RegexOptions.Compiled);
    private static readonly Regex paragraphRegex = new(TextChunkingRegex.PARAGRAPH_PATTERN, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    private static readonly Regex sentenceRegex = new(TextChunkingRegex.SENTENCE_PATTERN, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    private static readonly Regex tokenCountRegex = new(TextChunkingRegex.TOKEN_COUNT_PATTERN, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    private static readonly Regex decimalNumberRegex = new(TextChunkingRegex.DECIMAL_NUMBER_PATTERN, RegexOptions.Compiled);
    private static readonly Regex letterNumberRegex = new(TextChunkingRegex.LETTER_NUMBER_PATTERN, RegexOptions.Compiled);
    private static readonly Regex shortAbbreviationRegex = new(TextChunkingRegex.SHORT_ABBREVIATION_PATTERN, RegexOptions.Compiled);
    private static readonly Regex multipleSpacedInitialsRegex = new(TextChunkingRegex.MULTIPLE_SPACED_INITIALS_PATTERN, RegexOptions.Compiled);
    private static readonly Regex multipleInitialsNoSpacesRegex = new(TextChunkingRegex.MULTIPLE_INITIALS_NO_SPACES_PATTERN, RegexOptions.Compiled);
    private static readonly Regex singleLetterInitialRegex = new(TextChunkingRegex.SINGLE_LETTER_INITIAL_PATTERN, RegexOptions.Compiled);
    private static readonly Regex xmlRegex = new(TextChunkingRegex.XML_PATTERN, RegexOptions.Compiled | RegexOptions.Singleline);

    internal static IEnumerable<string> GetParagraphs(string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        var paragraphs = TextChunkingRegex.paragraphRegex
            .Split(text.Trim())
            .Select(x => x.Trim())
            .Where(x => x.Length > 0);

        return paragraphs;
    }

    internal static List<string> GetSentences(string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        var result = new List<string>();

        // Find JSON and XML matches inside text
        var matches = GetJsonMatches(text)
            .Concat(GetXmlMatches(text))
            .OrderBy(m => m.Index)
            .ToList();

        var lastIndex = 0;

        foreach (var match in matches)
        {
            // Text before the JSON/XML match
            if (match.Index > lastIndex)
            {
                var before = text.Substring(lastIndex, match.Index - lastIndex).Trim();
                if (!string.IsNullOrEmpty(before))
                    result.AddRange(GetSentencesRegular(before));
            }

            // Add JSON/XML match as its own sentence
            result.Add(match.Value);

            lastIndex = match.Index + match.Length;
        }

        // Remaining text after the last match
        if (lastIndex < text.Length)
        {
            var after = text[lastIndex..].Trim();

            if (!string.IsNullOrEmpty(after))
            {
                result
                    .AddRange(TextChunkingRegex.GetSentencesRegular(after));
            }
        }

        return TextChunkingRegex.NormalizeSentences(result).ToList();
    }

    internal static IEnumerable<Match> GetJsonMatches(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }

        var stack = new Stack<(char open, int index)>();

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            switch (c)
            {
                case '{' or '[':
                    stack
                        .Push((c, i));
                    break;

                case '}' when stack.Count > 0 && stack.Peek().open == '{':
                case ']' when stack.Count > 0 && stack.Peek().open == '[':
                {
                    var (_, startIndex) = stack.Pop();
                    if (stack.Count == 0)
                    {
                        yield return new Regex(Regex.Escape(text.Substring(startIndex, i - startIndex + 1)))
                            .Match(text, startIndex, i - startIndex + 1);
                    }

                    break;
                }
            }
        }
    }

    internal static IEnumerable<Match> GetXmlMatches(string text)
    {
        if (text == null) 
            throw new ArgumentNullException(nameof(text));

        return TextChunkingRegex.xmlRegex
            .Matches(text);
    }

    internal static int GetTokenCount(string sentence)
    {
        if (sentence == null)
            throw new ArgumentNullException(nameof(sentence));

        var sentenceTokens = TextChunkingRegex.tokenCountRegex
            .Matches(sentence).Count;

        return sentenceTokens;
    }


    private static List<string> GetSentencesRegular(string text)
    {
        var sentences = new List<string>();

        var parts = sentenceRegex.Split(text.Trim()).ToList();

        foreach (var part in parts)
        {
            if (sentences.Count == 0)
            {
                sentences.Add(part);
                continue;
            }

            var prevSentence = sentences.Last();

            var prevTokens = prevSentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var lastToken = prevTokens.LastOrDefault() ?? string.Empty;

            var nextWords = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var nextStartsUpper = part.Length > 0 && char.IsUpper(part[0]);
            var nextWordCount = nextWords.Length;

            var isAbbreviationEnding = IsAbbreviationEnding(prevSentence, lastToken);

            if (part.Length > 0 && char.IsLower(part[0]))
            {
                sentences[^1] += $" {part}";
                continue;
            }

            if (isAbbreviationEnding)
            {
                if (nextStartsUpper && nextWordCount > MIN_SENTENCE_WORD_COUNT)
                    sentences.Add(part);
                else
                    sentences[^1] += $" {part}";
            }
            else if (nextStartsUpper)
            {
                sentences.Add(part);
            }
            else
            {
                sentences[^1] += $" {part}";
            }
        }

        return sentences.Select(x => x.Trim()).ToList();
    }
    private static IEnumerable<string> NormalizeSentences(IEnumerable<string> sentences)
    {
        var list = new List<string>();

        foreach (var sentence in sentences)
        {
            if (list.Count > 0)
            {
                // If last sentence is JSON/XML and current is punctuation-only, merge them
                if ((IsJsonBlock(list[^1]) || IsXmlBlock(list[^1])) && TextChunkingRegex.punctuationRegex.IsMatch(sentence))
                {
                    list[^1] += sentence; // attach punctuation
                    continue;
                }
            }

            list.Add(sentence);
        }

        return list;
    }
    private static bool IsJsonBlock(string text)
    {
        if (text == null) 
            throw new ArgumentNullException(nameof(text));
        
        return text.StartsWith("{") && text.EndsWith("}");
    }
    private static bool IsXmlBlock(string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        return text.StartsWith("<") && text.EndsWith(">");
    }
    private static bool IsAbbreviationEnding(string sentence, string lastToken)
    {
        if (sentence == null)
            throw new ArgumentNullException(nameof(sentence));

        if (lastToken == null)
            throw new ArgumentNullException(nameof(lastToken));

        // Ignore decimals and letter+number combos as they end sentences
        if (TextChunkingRegex.decimalNumberRegex.IsMatch(lastToken))
        {
            return false;
        }

        if (TextChunkingRegex.letterNumberRegex.IsMatch(lastToken))
        {
            return false;
        }

        // Check if sentence ENDS WITH multiple spaced initials (e.g. "U. S.")
        if (TextChunkingRegex.multipleSpacedInitialsRegex.IsMatch(sentence.Trim()))
        {
            return true;
        }

        // Check if lastToken is multiple initials with no spaces (e.g. "U.S.")
        if (TextChunkingRegex.multipleInitialsNoSpacesRegex.IsMatch(lastToken))
        {
            return true;
        }

        // Check single-letter or short abbreviations (1-4 letters)
        if (TextChunkingRegex.singleLetterInitialRegex.IsMatch(lastToken))
        {
            return true;
        }

        if (TextChunkingRegex.shortAbbreviationRegex.IsMatch(lastToken))
        {
            return true;
        }

        return false;
    }
}