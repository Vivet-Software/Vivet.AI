using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SemanticKernel;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Services.Collectors.Models;

internal class AgentInstructionsCollector
{
    private readonly List<AgentInstruction> results = [];

    internal virtual IEnumerable<AgentInstruction> GetByAgent(string agentId)
    {
        if (agentId == null) 
            throw new ArgumentNullException(nameof(agentId));
        
        return this.results
            .Where(x => x.AgentId == agentId)
            .OrderBy(x => x.CreatedAt);
    }

    internal virtual void Add(ChatMessageContent chatMessageContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        var agentId = chatMessageContent
            .GetAgentId();

        this.results
            .Add(new AgentInstruction
            {
                AgentId = agentId,
                Input = chatMessageContent.Content,
                Role = chatMessageContent.Role.Label
            });
    }
}