namespace Vivet.AI.Services.Requests.Agents;

/// <summary>
/// Concurrent orchestration enables multiple agents to work on the same task in parallel. Each agent processes the input independently,
/// and their results are collected and aggregated. This approach is well-suited for scenarios where diverse perspectives or solutions are valuable,
/// such as brainstorming, ensemble reasoning, or voting systems..
/// </summary>
public class ConcurrentAgentsRequest : BaseAgentsRequest;