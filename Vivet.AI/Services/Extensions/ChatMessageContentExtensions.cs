using Microsoft.CSharp.RuntimeBinder;
using Microsoft.SemanticKernel;
using System;

namespace Vivet.AI.Services.Extensions;

internal static class ChatMessageContentExtensions
{
    internal static string GetExternalId(this ChatMessageContent chatMessageContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        var innerContentId = chatMessageContent.InnerContent
            .TryGetPropertyValue<string>("Id");

        if (innerContentId != null)
        {
            return innerContentId;
        }

        if (chatMessageContent.Metadata == null)
        {
            return null;
        }

        if (!chatMessageContent.Metadata.TryGetValue("Id", out var value) || value == null)
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

    internal static Guid? GetAgentId(this ChatMessageContent chatMessageContent)
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
                    var value = chatMessageContent.AuthorName.Substring(indexOfBracketStart + 1, len);

                    return Guid.Parse(value);
                }
            }
        }

        return null;
    }

    internal static DateTimeOffset? GetAgentCreatedAt(this ChatMessageContent chatMessageContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        var innerContentCreated = chatMessageContent.InnerContent
            .TryGetPropertyValue<DateTimeOffset?>("Created");

        if (innerContentCreated != null)
        {
            return innerContentCreated;
        }

        if (chatMessageContent.Metadata == null)
        {
            return null;
        }

        if (!chatMessageContent.Metadata.TryGetValue("CreatedAt", out var value) || value == null)
        {
            return null;
        }

        try
        {
            var strValue = value.ToString();

            return strValue == null
                ? null
                : DateTimeOffset.Parse(strValue);
        }
        catch (RuntimeBinderException)
        {
            return null;
        }
    }

    internal static string GetFinishReason(this ChatMessageContent chatMessageContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (chatMessageContent.Metadata == null)
        {
            return null;
        }

        if (!chatMessageContent.Metadata.TryGetValue("FinishReason", out var value) || value == null)
        {
            return null;
        }

        try
        {
            return value.ToString();
        }
        catch (RuntimeBinderException)
        {
            return null;
        }
    }
}