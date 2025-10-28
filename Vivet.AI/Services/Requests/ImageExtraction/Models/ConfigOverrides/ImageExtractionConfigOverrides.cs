using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.ImageExtraction.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to image extraction operations.
/// </summary>
public class ImageExtractionConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the name of the model to use for this request, overriding the default configured model.
    /// The specified model must be supported by the registered orchestration; otherwise, the request may fail.
    /// </summary>
    public virtual string ModelName { get; set; }
}