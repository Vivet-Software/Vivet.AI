using Vivet.AI.Services.Collectors.Models;

namespace Vivet.AI.Services.Collectors;

internal static class AgentCollectorContext
{
    internal static FunctionCallCollector Functions => FunctionsCollectorContext.Functions;

    internal static AgentInstructionsCollector Instructions => AgentInstructionsCollectorContext.Instructions;

    internal static ResponseCallbackCollector ResponseCallbacks => ResponseCallbackCollectorContext.ResponseCallbacks;

    internal static void Initialize()
    {
        FunctionsCollectorContext.Initialize();
        AgentInstructionsCollectorContext.Initialize();
        ResponseCallbackCollectorContext.Initialize();
    }
}
