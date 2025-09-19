using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Requests.Agent;
using Vivet.AI.Services.Responses.Agent;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// The agent service interface used to invoke agentic orchestrations.
/// </summary>
public interface IAgentService : IAsyncDisposable
{
    /// <summary>
    /// Invokes a set of agents against a type of orchestration. 
    /// </summary>
    /// <param name="request">The agent request containing the input, orchestration, agents and other context.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns></returns>
    Task<AgentResponse> InvokeAsync(AgentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes a set of agents against a type of orchestration.
    /// The response is streamed back to the caller. 
    /// </summary>
    /// <param name="request">The agent request containing the input, orchestration, agents and other context.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns></returns>
    IAsyncEnumerable<string> InvokeStreamingAsync(AgentRequest request, CancellationToken cancellationToken = default);
}