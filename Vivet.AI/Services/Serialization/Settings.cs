using Newtonsoft.Json;

namespace Vivet.AI.Services.Serialization;

internal static class Settings
{
    internal static JsonSerializerSettings SerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    internal static JsonSerializerSettings ResponseSerializerSettings = new()
    {
        ContractResolver = new InternalContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
}