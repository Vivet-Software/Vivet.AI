namespace Vivet.AI.Services.Requests.Agents;

/// <summary>
/// Handoff orchestration allows agents to transfer control to one another based on the context or user request.
/// Each agent can "handoff" the conversation to another agent with the appropriate expertise,
/// ensuring that the right agent handles each part of the task.
/// This is particularly useful in customer support, expert systems, or any scenario requiring dynamic delegation.
/// </summary>
public class HandOffAgentsRequest : BaseAgentsRequest;