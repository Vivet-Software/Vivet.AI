namespace Vivet.AI.Services.Requests.Agent.Enums;

/// <summary>
/// The type of agent orchestration.
/// </summary>
public enum AgentOrchestrationType
{
    /// <summary>
    /// In sequential orchestration, agents are organized in a pipeline. Each agent processes the task in turn,
    /// passing its output to the next agent in the sequence. This is ideal for workflows where each step builds upon the previous one,
    /// such as document review, data processing pipelines, or multi-stage reasoning.
    /// </summary>
    Sequential,

    /// <summary>
    /// Concurrent orchestration enables multiple agents to work on the same task in parallel. Each agent processes the input independently,
    /// and their results are collected and aggregated. This approach is well-suited for scenarios where diverse perspectives or solutions are valuable,
    /// such as brainstorming, ensemble reasoning, or voting systems..
    /// </summary>
    Concurrent,

    /// <summary>
    /// Group chat orchestration models a collaborative conversation among agents, optionally including a human participant.
    /// A group chat manager coordinates the flow, determining which agent should respond next and when to request human input.
    /// This pattern is powerful for simulating meetings, debates, or collaborative problem-solving sessions.
    /// </summary>
    GroupChat,

    /// <summary>
    /// Handoff orchestration allows agents to transfer control to one another based on the context or user request.
    /// Each agent can "handoff" the conversation to another agent with the appropriate expertise,
    /// ensuring that the right agent handles each part of the task.
    /// This is particularly useful in customer support, expert systems, or any scenario requiring dynamic delegation.
    /// </summary>
    HandOff,

    /// <summary>
    /// agentic orchestration is designed based on the Magentic-One system invented by AutoGen.
    /// It is a flexible, general-purpose multi-agent pattern designed for complex, open-ended tasks that require dynamic collaboration.
    /// In this pattern, a dedicated Magentic manager coordinates a team of specialized agents,
    /// selecting which agent should act next based on the evolving context, task progress, and agent capabilities.
    /// </summary>
    Magnetic
}