namespace Vivet.AI.Services.Models.Plugins.Contexts;

/// <summary>
/// Represents the base contexts for built-in plugins.
/// </summary>
public abstract class BaseBuiltInContext;

/// <summary>
/// Represents the base contexts for built-in plugins with generic parameters for each plugin.
/// </summary>
public abstract class BaseContext<TMemory, TKnowledge, TWebSearch> : BaseBuiltInContext
    where TMemory : class
    where TKnowledge : class
    where TWebSearch : class
{
    /// <summary>s
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