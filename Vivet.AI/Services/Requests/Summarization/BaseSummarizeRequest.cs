using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Summarization.Models;

namespace Vivet.AI.Services.Requests.Summarization;

/// <summary>
/// Represents the base request for a memory summarization operation.
/// </summary>
public abstract class BaseSummarizeRequest
{
    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    public virtual SummarizationConfigOverrides ConfigOverrides { get; set; }
}