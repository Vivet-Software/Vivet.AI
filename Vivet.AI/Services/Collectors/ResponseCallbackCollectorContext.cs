using System.Threading;
using Vivet.AI.Services.Collectors.Models;

namespace Vivet.AI.Services.Collectors;

internal static class ResponseCallbackCollectorContext
{
    private static readonly AsyncLocal<ResponseCallbackCollector> responseCallbacks = new();

    internal static ResponseCallbackCollector ResponseCallbacks => ResponseCallbackCollectorContext.responseCallbacks.Value ?? (ResponseCallbackCollectorContext.responseCallbacks.Value = new());

    internal static void Initialize()
    {
        ResponseCallbackCollectorContext.responseCallbacks.Value = new();
    }
}