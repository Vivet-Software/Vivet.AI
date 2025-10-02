using System;
using System.Threading;
using Vivet.AI.Services.Collectors.Models;

namespace Vivet.AI.Services.Collectors;

internal static class FunctionsCollectorContext
{
    private static readonly AsyncLocal<FunctionCallCollector> functions = new();

    internal static FunctionCallCollector Functions => FunctionsCollectorContext.functions.Value ?? throw new ArgumentNullException(nameof(functions.Value));

    internal static void Initialize()
    {
        FunctionsCollectorContext.functions.Value = new();
    }

    internal static void Dispose()
    {
        FunctionsCollectorContext.functions.Value = null;
    }
}