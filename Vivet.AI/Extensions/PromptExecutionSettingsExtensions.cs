using Microsoft.SemanticKernel;
using System;
using Vivet.AI.Models;

namespace Vivet.AI.Extensions;

internal static class PromptExecutionSettingsExtensions
{
    internal static PromptExecutionSettings GetOverridePromptExecutionSettings(this PromptExecutionSettings promptExecutionSettings, ChatModelParameters chatModelParameters = null)
    {
        if (promptExecutionSettings == null) 
            throw new ArgumentNullException(nameof(promptExecutionSettings));

        if (chatModelParameters == null)
        {
            return promptExecutionSettings;
        }

        return chatModelParameters
            .GetPromptExecutionSettings(promptExecutionSettings.GetType());
    }
}