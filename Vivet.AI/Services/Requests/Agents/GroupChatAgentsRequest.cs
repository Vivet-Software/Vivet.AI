namespace Vivet.AI.Services.Requests.Agents;

/// <summary>
/// Group chat orchestration models a collaborative conversation among agents, optionally including a human participant.
/// A group chat manager coordinates the flow, determining which agent should respond next and when to request human input.
/// This pattern is powerful for simulating meetings, debates, or collaborative problem-solving sessions.
/// </summary>
public class GroupChatAgentsRequest : BaseAgentsRequest;