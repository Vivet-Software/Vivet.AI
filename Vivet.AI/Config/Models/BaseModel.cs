using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config.Models;

/// <summary>
/// Base Model.
/// </summary>
public abstract class BaseModel
{
    /// <summary>
    /// The model name.
    /// Make sure the model is configured in the choosen AI provider (e.g. Azure AI, Azure OpenAU, Ollama, etc).
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// Whether to enable health-check for the model.
    /// Only one health-check will be configured for each distinct model used between all services. So if the same model is used my multiple services,
    /// only one health-check will be configured and invoked for that model.
    /// </summary>
    [Required]
    public virtual bool UseHealthCheck { get; set; } = true;
}