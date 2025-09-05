using System;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.SemanticKernel;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Extensions;

internal static class ChatMessageContentExtensions
{
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