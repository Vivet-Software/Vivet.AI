using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Summarization.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Summarization;

/// <summary>
/// Represents the base request for a memory summarization operation.
/// </summary>
public abstract class BaseSummarizeRequest
{
    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual SummarizationConfigOverrides ConfigOverrides { get; internal set; } = new();
}