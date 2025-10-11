namespace Vivet.AI.Services.Requests.Agents;

/// <summary>
/// In sequential orchestration, agents are organized in a pipeline. Each agent processes the task in turn,
/// passing its output to the next agent in the sequence. This is ideal for workflows where each step builds upon the previous one,
/// such as document review, data processing pipelines, or multi-stage reasoning.
/// </summary>
public class SequentialAgentsRequest : BaseAgentsRequest;