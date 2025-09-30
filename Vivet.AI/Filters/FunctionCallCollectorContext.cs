using System.Threading;
using Vivet.AI.Filters.Models;

namespace Vivet.AI.Filters;

internal static class FunctionCallCollectorContext
{
    internal static AsyncLocal<FunctionCallCollector> Current = new();
}