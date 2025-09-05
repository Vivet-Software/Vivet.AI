using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Responses.Embeddings.Memory;

/// <summary>
/// Represents the response for an index memory operation, including token usage.
/// </summary>
public class IndexMemoryResponse : BaseIndexResponse
{
    /// <summary>
    /// The token usage associated with summarization operations.
    /// </summary>
    public virtual TokenUsage SummarizationTokenUsage { get; set; }
}