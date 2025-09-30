namespace Vivet.AI.Filters.Models;

internal class FunctionCallResult
{
    internal string PluginName { get; set; }

    internal string FunctionName { get; set; }

    internal object Result { get; set; }
}