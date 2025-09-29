namespace Vivet.AI.Services.Models.Plugins;

/// <summary>
/// Represents the base contexts for built-in plugins.
/// </summary>
public abstract class BaseBuiltInContext<TMemory, TKnowledge, TWebSearch>
    where TMemory : class
    where TKnowledge : class
    where TWebSearch : class
{
    /// <summary>
    /// Context for the built-in memory plugin.
    /// </summary>
    public virtual TMemory Memory { get; set; }

    /// <summary>
    /// Context for the built-in knowledge plugin.
    /// </summary>
    public virtual TKnowledge Knowledge { get; set; }

    /// <summary>
    /// Context for the built-in web search plugin.
    /// </summary>
    public virtual TWebSearch WebSearch { get; set; }
}