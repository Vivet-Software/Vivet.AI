using System;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.SemanticKernel;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Extensions;

internal static class ChatMessageContentExtensions
{
    internal static string GetAgentId(this ChatMessageContent chatMessageContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (string.IsNullOrEmpty(chatMessageContent.AuthorName))
        {
            return null;
        }

        var indexOfBracketStart = chatMessageContent.AuthorName
            .LastIndexOf('[');

        if (indexOfBracketStart >= 0)
        {
            var indexOfBracketEnd = chatMessageContent.AuthorName
                .LastIndexOf(']');

            if (indexOfBracketEnd >= 0)
            {
                var len = indexOfBracketEnd - indexOfBracketStart - 1;

                if (len > 0)
                {
                    return chatMessageContent.AuthorName.Substring(indexOfBracketStart + 1, len);
                }
            }
        }

        return null;
    }

    internal static string GetExternalId(this ChatMessageContent chatMessageContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (chatMessageContent.Metadata == null)
        {
            return null;
        }

        if (!chatMessageContent.Metadata.TryGetValue("Id", out var idObj) || idObj == null)
        {
            return null;
        }

        try
        {
            return idObj as string;
        }
        catch (RuntimeBinderException)
        {
            return null;
        }
    }

    internal static TokenUsage GetTokenUsage(this ChatMessageContent chatMessageContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (chatMessageContent.Metadata == null)
        {
            return null;
        }

        if (!chatMessageContent.Metadata.TryGetValue("Usage", out var usageObj) || usageObj == null)
        {
            return null;
        }

        try
        {
            dynamic usage = usageObj;
            long inputTokens = usage.InputTokenCount;
            long outputTokens = usage.OutputTokenCount;

            return new TokenUsage
            {
                InputTokens = inputTokens,
                OutputTokens = outputTokens
            };
        }
        catch (RuntimeBinderException)
        {
            return null;
        }
    }
}