using System;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.SemanticKernel;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Extensions;

internal static class KernelContentExtensions
{
    internal static TokenUsage GetTokenUsage(this KernelContent kernelContent)
    {
        if (kernelContent == null)
            throw new ArgumentNullException(nameof(kernelContent));

        if (kernelContent.Metadata == null)
        {
            return null;
        }

        if (!kernelContent.Metadata.TryGetValue("Usage", out var value) || value == null)
        {
            return null;
        }

        try
        {
            dynamic usage = value;
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