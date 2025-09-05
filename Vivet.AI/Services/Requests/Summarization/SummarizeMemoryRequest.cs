using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Services.Requests.Summarization;

/// <summary>
/// Represents a request to summarize memory with a question and its corresponding answer.
/// </summary>
public class SummarizeMemoryRequest : BaseSummarizeRequest
{
    /// <summary>
    /// Gets or sets the question associated with the memory to summarize.
    /// </summary>
    [Required]
    public virtual string Question { get; set; }

    /// <summary>
    /// Gets or sets the answer corresponding to the question for memory summarization.
    /// </summary>
    [Required]
    public virtual string Answer { get; set; }
}