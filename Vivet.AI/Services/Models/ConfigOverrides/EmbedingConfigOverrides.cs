namespace Vivet.AI.Services.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to embedding operations.
/// </summary>
public class EmbedingConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the name of the model to use for this request, overriding the default configured model.
    /// The specified model must be supported by the registered orchestration; otherwise, the request may fail.
    /// </summary>
    public virtual string ModelName { get; set; }
}