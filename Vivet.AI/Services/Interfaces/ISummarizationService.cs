using System.Threading.Tasks;
using System.Threading;
using Vivet.AI.Services.Requests.Summarization;
using Vivet.AI.Services.Responses.Summarization;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Provides summarization services for content using an LLM chat completion service.
/// </summary>
public interface ISummarizationService
{
    /// <summary>
    /// Summarizes the provided memory request into a concise representation using the configured LLM.
    /// Any inline JSON or XML will not be summarized, but left as is.
    /// </summary>
    /// <param name="request">The summarization request containing input data and model parameters.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="SummarizationMemoryResponse"/> containing the summarization result, elapsed time, token usage, and any error information./// </returns>
    Task<SummarizationMemoryResponse> SummarizeMemoryAsync(SummarizeMemoryRequest request, CancellationToken cancellationToken = default);
}