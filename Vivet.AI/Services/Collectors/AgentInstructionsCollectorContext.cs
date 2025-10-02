using System.Threading;
using Vivet.AI.Services.Collectors.Models;

namespace Vivet.AI.Services.Collectors;

internal static class AgentInstructionsCollectorContext
{
    private static readonly AsyncLocal<AgentInstructionsCollector> instructions = new();

    internal static AgentInstructionsCollector Instructions => AgentInstructionsCollectorContext.instructions.Value ?? (AgentInstructionsCollectorContext.instructions.Value = new());

    internal static void Initialize()
    {
        AgentInstructionsCollectorContext.instructions.Value = new();
    }
}