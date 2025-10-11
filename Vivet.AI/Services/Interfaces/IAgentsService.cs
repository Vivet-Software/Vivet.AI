using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Requests.Agents;
using Vivet.AI.Services.Responses.Agent;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// The agents service interface used to invoke agentic orchestrations.
/// </summary>
public interface IAgentsService : IAsyncDisposable
{
    /// <summary>
    /// Invokes a set of agents against a type of orchestration. 
    /// </summary>
    /// <param name="request">The agent request containing the input, orchestration, agents and other context.</param>
    /// <param name="onMemoryIndexed">
    /// Optional callback that is invoked after the response has been saved to memory.
    /// This callback is awaited, meaning the <see cref="Task"/> returned by this method will not
    /// complete until the callback itself has finished. If you need the callback work to run
    /// in the background without delaying the chat result, you must offload that work explicitly
    /// (e.g. <c>Task.Run</c> or a background queue).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>The agents response.</returns>
    Task<AgentsResponse> InvokeAsync(BaseAgentsRequest request, Func<IList<AgentIndexMemoryResponse>, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes a set of agents against a type of orchestration.
    /// The response is streamed back to the caller. 
    /// </summary>
    /// <param name="request">The agent request containing the input, orchestration, agents and other context.</param>
    /// <param name="onMemoryIndexed">
    /// Optional callback that is invoked after the response has been saved to memory.
    /// This callback is awaited, meaning the <see cref="Task"/> returned by this method will not
    /// complete until the callback itself has finished. If you need the callback work to run
    /// in the background without delaying the chat result, you must offload that work explicitly
    /// (e.g. <c>Task.Run</c> or a background queue).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>The streaming result.</returns>
    IAsyncEnumerable<string> InvokeStreamingAsync(BaseAgentsRequest request, Func<IList<AgentIndexMemoryResponse>, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default);
}