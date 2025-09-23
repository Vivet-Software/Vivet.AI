using Vivet.AI.Models;

namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents the base class for chat configuration overrides.
/// </summary>
public abstract class BaseChatConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the name of the model to use for this request, overriding the default configured model.
    /// The specified model must be supported by the registered orchestration; otherwise, the request may fail.
    /// </summary>
    public virtual string ModelName { get; set; }

    /// <summary>
    /// Optional parameters for configuring the behavior of the chat model.
    /// </summary>
    public virtual ChatModelParameters ModelParameters { get; set; }

    /// <summary>
    /// Plugin config overrides.
    /// </summary>
    public virtual BuiltInPluginsOverrides Plugins { get; set; }
}