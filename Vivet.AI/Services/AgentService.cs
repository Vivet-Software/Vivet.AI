using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Orchestration.Handoff;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Agent;
using Vivet.AI.Services.Requests.Agent.Enums;
using Vivet.AI.Services.Responses.Agent;

namespace Vivet.AI.Services;

// TODO: Does AgentId belong in ChatRequest?

/// <inheritdoc cref="IAgentService"/>
public class AgentService(AgentOptions options, IKernelBuilder kernelBuilder)
    : BaseService, IAgentService  
{
    private readonly InProcessRuntime agenticProcess = new();

    /// <inheritdoc />
    public virtual async Task<AgentResponse> InvokeAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        await this.agenticProcess // TODO: Inspect additional methods, what are they for? how are they used
            .StartAsync(cancellationToken);

        var kernel = kernelBuilder
            .Build();

        var agentOrchestration = this.GetAgentOrchestration(request, kernel);

        // BUG: Structured Outputs, and more, Read this: https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-orchestration/advanced-topics?pivots=programming-language-csharp

        var orchestrationResult = await agentOrchestration
            .InvokeAsync(request.Input, this.agenticProcess, cancellationToken);

        var chatMessageContents = await orchestrationResult
            .GetValueAsync(options.Timeout, cancellationToken);

        await this.agenticProcess
            .RunUntilIdleAsync();

        var tokenUsage = chatMessageContents
            .Aggregate(new TokenUsage(), (current, x) => current + x.GetTokenUsage());

        stopwatch
            .Stop();

        return new AgentResponse
        {
            // BUG: RESPONSE: Agent Response
            ElapsedTime = stopwatch.Elapsed,
            TokenUsage = tokenUsage,
            //ErrorMessage = 
        };
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerable<string> InvokeStreamingAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (this.agenticProcess != null)
        {
            return this.agenticProcess
                .DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }


    private Agent[] GetAgents(Kernel kernel, params Agent2[] requestAgents)
    {
        if (kernel == null) 
            throw new ArgumentNullException(nameof(kernel));

        // BUG: 111: Plugins: Should we use different Kernel, so Plugins can be different?
        // Memory, Knowledge and Web Search plugins should they derive from Chat or how, otherwise we need duplicate settings for those under AgentOptions
        // We could also move them to it's own section: Ai.Plugins, and then enable/disable under Chat, Agent, etc.

        var agents = requestAgents
            .Select(x => new ChatCompletionAgent
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Instructions = x.Instructions,
                InstructionsRole = x.Role,
                Kernel = kernel,
                Arguments = null,
                LoggerFactory = kernel.LoggerFactory,
                HistoryReducer = null, // BUG: HISTORY: Figure out if we should use this. Also because we are using orchestrations.
                Template = null, // BUG: Should we support this? https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-templates?pivots=programming-language-csharp#yaml-template
                UseImmutableKernel = false
            })
            .ToArray<Agent>();

        return agents;
    }
    private AgentOrchestration<string, ChatMessageContent[]> GetAgentOrchestration(AgentRequest request, Kernel kernel)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));
        
        var agents = this.GetAgents(kernel, request.Agents.ToArray());

        AgentOrchestration<string, ChatMessageContent[]> agentOrchestration = request.OrchestrationType switch
        {
            AgentOrchestrationType.Sequential => new SequentialOrchestration<string, ChatMessageContent[]>(agents)
            {
                Name = request.Name,
                Description = request.Description,
                ResultTransform = ResultTransform,
                LoggerFactory = kernel.LoggerFactory,
                ResponseCallback = response =>
                {
                    // BUG: HISTORY: ResponseCallback
                    // Chat history: This is interesting, bceause we keep current conversation in memory, and I don't think I can look this up through embedding matching
                    // So maybe ChatService (or AgentService) should just keep an in memory (or persisted, having Id's from Memory vector store) and that way we can support agentic threads until user closes a thread?
                    // Then as chat-gpt says we probably need a way to summarize history entries when it gets big.

                    // We need some way of maintaining the ChatHistory
                    // How long should chat history be kept, probably just on a single Agentic execution.

                    return ValueTask.CompletedTask;
                }
            },
            AgentOrchestrationType.Concurrent => new ConcurrentOrchestration<string, ChatMessageContent[]>(agents) 
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = kernel.LoggerFactory,
                ResultTransform = ResultTransform,
                ResponseCallback = response =>
                {
                    return ValueTask.CompletedTask;
                }
            },
            // TODO: Group chat Manager Override. https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-orchestration/group-chat?pivots=programming-language-csharp#customize-the-group-chat-manager
            AgentOrchestrationType.GroupChat => new GroupChatOrchestration<string, ChatMessageContent[]>(null, agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = kernel.LoggerFactory,
                ResultTransform = ResultTransform,
                ResponseCallback = response =>
                {
                    return ValueTask.CompletedTask;
                }
            },
            AgentOrchestrationType.HandOff => new HandoffOrchestration<string, ChatMessageContent[]>(OrchestrationHandoffs.StartWith(agents.First()), agents) 
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = kernel.LoggerFactory,
                ResultTransform = ResultTransform,
                ResponseCallback = response =>
                {
                    return ValueTask.CompletedTask;
                },
            },
            AgentOrchestrationType.Magnetic => new MagenticOrchestration<string, ChatMessageContent[]>(null, agents) 
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = kernel.LoggerFactory,
                ResultTransform = ResultTransform,
                ResponseCallback = response =>
                {
                    return ValueTask.CompletedTask;
                },
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request.OrchestrationType), request.OrchestrationType, $"Orchestration type {request.OrchestrationType} not supported")
        };

        return agentOrchestration;
    }

    private static ValueTask<ChatMessageContent[]> ResultTransform(IList<ChatMessageContent> contents, CancellationToken cancellationToken = default)
    {
        if (contents == null) 
            throw new ArgumentNullException(nameof(contents));

        var result = contents.ToArray();

        return ValueTask.FromResult(result);
    }
}