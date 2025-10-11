using System;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Services.Responses.Agent.Models;

internal class AgentResponseCallback
{
    internal virtual ChatMessageContent ChatMessageContent { get; set; }

    internal virtual TimeSpan ElapsedTime { get; set; }
}