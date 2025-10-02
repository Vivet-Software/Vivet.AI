using Vivet.AI.Services.Collectors.Models;

namespace Vivet.AI.Services.Collectors;

internal static class ChatCollectorContext
{
    internal static FunctionCallCollector Functions => FunctionsCollectorContext.Functions;

    internal static void Initialize()
    {
        FunctionsCollectorContext.Initialize();
    }

    internal static void Dispose()
    {
        FunctionsCollectorContext.Dispose();
    }
}