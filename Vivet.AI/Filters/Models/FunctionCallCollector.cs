using System.Collections.Generic;

namespace Vivet.AI.Filters.Models;

internal class FunctionCallCollector
{
    internal List<FunctionCallResult> Results { get; } = [];
}