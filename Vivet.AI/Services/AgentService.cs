using Azure.Core;
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
using Vivet.AI.Extensions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Agent;
using Vivet.AI.Services.Requests.Agent.Enums;
using Vivet.AI.Services.Requests.Agent.Models;
using Vivet.AI.Services.Requests.Agent.Models.ConfigOverrides;
using Vivet.AI.Services.Responses.Agent;

namespace Vivet.AI.Services;

// TODO: 333: Consider Move Plugins to Ai.Plugins, and then enable/disable under Chat, Agent, etc.
// TODO: Config agents

// TODO: readme:
// Change Request plugins to Types instead of objects
// Emphasize the the build-in plugins must have context variables in request or an exception is thrown
// Check if we still writing that plugin dependencies must be registered beforehand, that isn't necessary anymore. Remove it.
// Same type of custom plugins is allowed, as long as they have different names. Mention the built-in plugin names (memory, knowledge, web_search)
// Update Custom plugins options configuration to have Type + Name (see CustomPluginOptions)
// a plugin name can contain only ASCII letters, digits, and underscores
// Plugins must have seperate context variables even when they are re-used among several plugins
// update web search plugin, config etc. (Limit removed from config)

/// <inheritdoc cref="IAgentService"/>
public class AgentService(AgentOptions options, IServiceProvider serviceProvider, IKernelBuilder kernelBuilder, PromptExecutionSettings promptExecutionSettings)
    : BaseService, IAgentService
{
    private bool isAgenticProcessStarted;
    private readonly SemaphoreSlim startLock = new(1, 1);
    private readonly InProcessRuntime agenticProcess = new(); // TODO: Advamced Agent Process Runtime Features (States, Subscriptions, Change Agents, Singlenton DI)

    /// <inheritdoc />
    public virtual async Task<AgentResponse> InvokeAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        request
            .Validate();

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        // TODO: Blobs

        // TODO: Add context prompt (We can't use ChatHistory, so make make private extensoons used in GetPluginsContext for Chat
        // We need to add this in context for the prompt / instructiions. How is that done???
        // var context = @$"Context: 
        //UserId={request.UserId}, 
        //ScopeId={request.ScopeId}, 
        //ThreadId={request.CurrentThreadId}, 
        //TenantId={request.TenantId}, 
        //SubTenantId={request.SubTenantId}";

        // TODO: Check inheritance of Kernel (Request, Agent) and maybe allow for override when same name and inherit of context variables
        // maybe that latter doesn't make sense, 
        // Evaluate this thoroughly
        var kernel = this.GetKernel(request); 

        var executionSettings = this.GetPromptExecutionSettingsOverrridesOrDefault(request.ConfigOverrides);
        var agents = this.GetAgents(kernel, executionSettings, request.Agents);
        var agentOrchestration = this.GetAgentOrchestration(request, kernel, agents);

        await this.StartAgentProcessAsync(cancellationToken);

        var orchestrationResult = await agentOrchestration
            .InvokeAsync(request.Input, this.agenticProcess, cancellationToken);

        var chatMessageContents = await orchestrationResult
            .GetValueAsync(options.Timeout, cancellationToken);

        var tokenUsage = chatMessageContents
            .Aggregate(new TokenUsage(), (current, x) => current + x.GetTokenUsage());

        stopwatch
            .Stop();

        return new AgentResponse
        {
            // TODO: HISTORY: Agent Response
            ElapsedTime = stopwatch.Elapsed,
            TokenUsage = tokenUsage,
            //ErrorMessage = 
        };
    }

    /// <inheritdoc />
    public virtual Task<AgentResponse> InvokeAsync<T>(AgentRequest request, CancellationToken cancellationToken = default)
    {
        // Structured Outputs, and more,
        // https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-orchestration/advanced-topics?pivots=programming-language-csharp
        
        throw new NotImplementedException();
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
            this.agenticProcess
                .StopAsync();

            return this.agenticProcess
                .DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }


    private async Task StartAgentProcessAsync(CancellationToken cancellationToken = default)
    {
        await this.startLock
            .WaitAsync(cancellationToken);

        try
        {
            if (this.isAgenticProcessStarted)
            {
                return;
            }

            await this.agenticProcess
                .StartAsync(cancellationToken);

            this.isAgenticProcessStarted = true;
        }
        finally
        {
            this.startLock
                .Release();
        }
    }
    private Kernel GetKernel(AgentRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var kernel = kernelBuilder
            .Build();

        kernel.Plugins
            .ValidateContext(request.Plugins.Context, request.ConfigOverrides.Plugins);

        kernel
            .AddPluginConfigOverrides(request.ConfigOverrides)
            .AddCustomPlugins(serviceProvider, request.Plugins.CustomPlugins);

        return kernel;
    }
    private Kernel GetAgentKernelOrDefault(AgentDescriptor agent)
    {
        if (agent == null)
            throw new ArgumentNullException(nameof(agent));

        if (agent.ConfigOverrides?.Plugins == null)
        {
            return null;
        }

        var kernel = kernelBuilder
            .Build();

        kernel.Plugins
            .ValidateContext(agent.Plugins.Context, agent.ConfigOverrides.Plugins);

        kernel
            .AddPluginConfigOverrides(agent.ConfigOverrides)
            .AddCustomPlugins(serviceProvider, agent.Plugins.CustomPlugins);

        return kernel;
    }
    private PromptExecutionSettings GetPromptExecutionSettingsOverrridesOrDefault(AgentConfigOverrides configOverrides)
    {
        if (configOverrides == null)
        {
            return promptExecutionSettings;
        }

        var executionSettings = promptExecutionSettings
            .GetOverridePromptExecutionSettings(configOverrides.ModelParameters);

        executionSettings.ModelId = configOverrides.ModelName;

        return executionSettings;
    }
    private Agent[] GetAgents(Kernel kernel, PromptExecutionSettings executionSettings, IEnumerable<AgentDescriptor> requestAgents)
    {
        if (kernel == null) 
            throw new ArgumentNullException(nameof(kernel));
        
        if (promptExecutionSettings == null)
            throw new ArgumentNullException(nameof(promptExecutionSettings));

        if (requestAgents == null)
            throw new ArgumentNullException(nameof(requestAgents));

        var agents = requestAgents
            .Select(x =>
            {
                var agentKernel = this.GetAgentKernelOrDefault(x) ?? kernel;

                return new ChatCompletionAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Instructions = x.Instructions,
                    InstructionsRole = x.Role,
                    Kernel = agentKernel,
                    Arguments = new KernelArguments(executionSettings),
                    LoggerFactory = agentKernel.LoggerFactory,
                    HistoryReducer = null, // TODO: HISTORY: Figure out if we should use this. Also because we are using orchestrations.
                    Template = null,
                    UseImmutableKernel = false
                };
            })
            .ToArray<Agent>();

        return agents;
    }
    private AgentOrchestration<string, ChatMessageContent[]> GetAgentOrchestration(AgentRequest request, Kernel kernel, Agent[] agents)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (kernel == null) 
            throw new ArgumentNullException(nameof(kernel));

        if (agents == null)
            throw new ArgumentNullException(nameof(agents));

        AgentOrchestration<string, ChatMessageContent[]> agentOrchestration = request.OrchestrationType switch
        {
            AgentOrchestrationType.Sequential => new SequentialOrchestration<string, ChatMessageContent[]>(agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = kernel.LoggerFactory,
                ResultTransform = ResultTransform,
                ResponseCallback = response =>
                {
                    // TODO: HISTORY: ResponseCallback
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