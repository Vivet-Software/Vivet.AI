using System;

namespace Vivet.AI.Services.Collectors.Models;

internal class AgentInstruction
{
    internal virtual string AgentId { get; set; }

    internal virtual string Input { get; set; }

    internal virtual string Role { get; set; }

    internal virtual DateTime CreatedAt { get; } = DateTime.UtcNow;
}