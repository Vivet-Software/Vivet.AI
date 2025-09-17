using FuzzySharp;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Vivet.AI.Services.Extensions;

internal static class StringExtensions
{
    private static readonly Regex answerRegex = new(@"(?:<think>)?[\s\S]*?</think>\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex answerRegexOrphaned = new(@"\A[\s\S]*?</think>\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex thinkingRegex = new(@"(?:<think>)?([\s\S]*?)</think>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    internal static string GetChatResponseAnswer(this string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        var result = StringExtensions.answerRegex
            .Replace(content, string.Empty);

        result = StringExtensions.answerRegexOrphaned
            .Replace(result, string.Empty);

        return result;
    }

    internal static string GetChatResponseThinking(this string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        var values = thinkingRegex
            .Matches(content)
            .Select(x => x.Groups[1].Value.Trim());

        var thinking = string.Join(" ", values);

        return thinking == string.Empty 
            ? null 
            : thinking;
    }

    internal static string GetContentHash(this string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        using var sha256 = SHA256.Create();

        var bytes = Encoding.UTF8
            .GetBytes(text);

        var hashBytes = sha256
            .ComputeHash(bytes);

        return Convert.ToHexString(hashBytes);
    }

    internal static long GetUtf8ByteCount(this string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        return Encoding.UTF8.GetByteCount(text);
    }

    internal static bool AreSimilar(this string value, string other = null, double threshold = 0.95)
    {
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        if (other == null)
        {
            return false;
        }

        if (value.Length == 0 || other.Length == 0)
        {
            return false;
        }

        value = value
            .ToLowerInvariant()
            .Trim();
        
        other = other
            .ToLowerInvariant()
            .Trim();

        if (value == other)
        {
            return true;
        }

        var score = Fuzz.Ratio(value, other) / 100.0;

        return score >= threshold;
    }
}