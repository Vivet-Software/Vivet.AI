using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Orchestration.Handoff;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Agent;
using Vivet.AI.Services.Requests.Agent.Enums;
using Vivet.AI.Services.Requests.Agent.Models;
using Vivet.AI.Services.Requests.Agent.Models.ConfigOverrides;
using Vivet.AI.Services.Responses.Agent;
using Vivet.AI.Services.Responses.Agent.Models;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;

// TODO: I still don't like the plugin context validation, isn't there a better way
// Also the whole plugins and context seems confusing - final check of config/overerrides/etc.

// TODO: 111: Consider Move Plugins to Ai.Plugins, and then enable/disable under Chat, Agent, etc.
// TODO: 111: Config agents
// TODO: 111: readme:
// Change Request plugins to Types instead of objects
// Emphasize the the build-in plugins must have context variables in request or an exception is thrown
// Check if we still writing that plugin dependencies must be registered beforehand, that isn't necessary anymore. Remove it.
// Same type of custom plugins is allowed, as long as they have different names. Mention the built-in plugin names (memory, knowledge, web_search)
// Update Custom plugins options configuration to have Type + Name (see CustomPluginOptions)
// a plugin name can contain only ASCII letters, digits, and underscores
// Plugins must have seperate context variables even when they are re-used among several plugins
// update web search plugin, config etc. (Limit removed from config)
// Check documentation for Response.ErrorMessage, we actual throw and Exception and the property is internal. 

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

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var agentResponses = new ConcurrentBag<AgentResult>();

        var executionSettings = this.GetPromptExecutionSettingsOverrrides(request.ConfigOverrides);
        var agents = this.GetAgents(request, executionSettings);
        var inputPrompt = await this.GetInputPrompt(request, cancellationToken);
        var agentOrchestration = this.GetAgentOrchestration(request, agents, inputPrompt, stopwatch, agentResponses);

        await this.StartAgentProcessAsync(cancellationToken);

        var orchestrationResult = await agentOrchestration
            .InvokeAsync(request.Input, this.agenticProcess, cancellationToken);

        await orchestrationResult
            .GetValueAsync(options.Timeout, cancellationToken);

        stopwatch
            .Stop();

        var response = AgentService.GetResponse(inputPrompt, agentResponses, stopwatch.Elapsed);

        // BUG: HISTORY: Save memory, SkipSaveMemory
        // Use Agent.Id for the orchestration, or the individual agents

        return response;
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
    private PromptExecutionSettings GetPromptExecutionSettingsOverrrides(AgentConfigOverrides configOverrides)
    {
        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        var executionSettings = promptExecutionSettings
            .GetOverridePromptExecutionSettings(configOverrides.ModelParameters);

        executionSettings.ModelId = configOverrides.ModelName;

        return executionSettings;
    }
    private async Task<ChatHistory> GetInputPrompt(AgentRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var chatHistory = new ChatHistory();

        var binaryContents = await Task.WhenAll(request.Blobs
                .Select(y => y
                    .GetBinaryContent(cancellationToken)))
            .ConfigureAwait(false);

        chatHistory
            .AddChatUserPrompt(request.Input, binaryContents);

        return chatHistory;
    }
    private Agent[] GetAgents(AgentRequest request, PromptExecutionSettings executionSettings)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (promptExecutionSettings == null)
            throw new ArgumentNullException(nameof(promptExecutionSettings));

        var agents = new List<Agent>();
        foreach (var agentDescriptor in request.Agents)
        {
            var kernel = this.GetKernel(request, agentDescriptor, request.ConfigOverrides.Plugins);

            var chatHistory = new ChatHistory();

            // BUG: 000: How can we get this as part of the Response.InputPrompt (or an AgentResult.InputPrompt) 
            chatHistory
                .AddChatSystemPrompt<string>(agentDescriptor.Instructions)
                .AddAgentPluginsContextPrompt(agentDescriptor.Plugins, request.Plugins);

            var instructions = chatHistory
                .GetPromptAsText(true);

            agents
                .Add(new ChatCompletionAgent
                {
                    Id = agentDescriptor.Id,
                    Name = agentDescriptor.Id,
                    Description = agentDescriptor.Description,
                    Instructions = instructions,
                    InstructionsRole = agentDescriptor.Role,
                    Kernel = kernel,
                    Arguments = new KernelArguments(executionSettings),
                    LoggerFactory = kernel.LoggerFactory,
                    // BUG: HISTORY: setting? ChatHistoryTruncationReducer / ChatHistorySummarizationReducer / LastMessage / None
                    // make seetting to only pass the latest agent messagage along
                    // BUG: 000: Test if tools and other stuff that is injected into the prompt is visible when calling reducer. We need some way getting that back to the user similar to Chat
                    // try some web-search an see....
                    HistoryReducer = null,
                    Template = null,
                    UseImmutableKernel = false
                });
        }

        return agents
            .ToArray();
    }
    private Kernel GetKernel(AgentRequest request, AgentDescriptor agent, BuiltInPluginsConfigOverrides parentConfigOverrides)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        if (agent == null)
            throw new ArgumentNullException(nameof(agent));

        if (parentConfigOverrides == null)
            throw new ArgumentNullException(nameof(parentConfigOverrides));

        var kernel = kernelBuilder
            .Build();

        kernel
            .AddBuiltInPluginConfigOverrides(agent.ConfigOverrides.Plugins, parentConfigOverrides)
            .AddCustomPlugins(serviceProvider, agent.Plugins.CustomPlugins);

        kernel.Plugins
            .ValidateContext(agent.Plugins.Context, request.Plugins.Context);

        return kernel;
    }
    private AgentOrchestration<string, ChatMessageContent[]> GetAgentOrchestration(AgentRequest request, Agent[] agents, ChatHistory inputPrompt, Stopwatch stopWatch, ConcurrentBag<AgentResult> agentResponses)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (agents == null)
            throw new ArgumentNullException(nameof(agents));

        if (stopWatch == null) 
            throw new ArgumentNullException(nameof(stopWatch));

        if (agentResponses == null) 
            throw new ArgumentNullException(nameof(agentResponses));

        var loggerFactory = agents.FirstOrDefault()?.LoggerFactory ?? serviceProvider.GetService<ILoggerFactory>();

        AgentOrchestration<string, ChatMessageContent[]> agentOrchestration = request.OrchestrationType switch
        {
            AgentOrchestrationType.Sequential => new SequentialOrchestration<string, ChatMessageContent[]>(agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = response => ResponseCallback(response, stopWatch, agentResponses)
            },
            AgentOrchestrationType.Concurrent => new ConcurrentOrchestration<string, ChatMessageContent[]>(agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = response => ResponseCallback(response, stopWatch, agentResponses)
            },
            AgentOrchestrationType.GroupChat => new GroupChatOrchestration<string, ChatMessageContent[]>(null, agents) // TODO: Group chat Orchestration. Manager Override. https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-orchestration/group-chat?pivots=programming-language-csharp#customize-the-group-chat-manager
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = response => ResponseCallback(response, stopWatch, agentResponses)
            },
            AgentOrchestrationType.HandOff => new HandoffOrchestration<string, ChatMessageContent[]>(OrchestrationHandoffs.StartWith(agents.First()), agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = response => ResponseCallback(response, stopWatch, agentResponses)
            },
            AgentOrchestrationType.Magnetic => new MagenticOrchestration<string, ChatMessageContent[]>(null, agents) // TODO: Magnetic Orchestration
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = response => ResponseCallback(response, stopWatch, agentResponses)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request.OrchestrationType), request.OrchestrationType, $"Orchestration type {request.OrchestrationType} not supported")
        };

        return agentOrchestration;
    }


    private static AgentResponse GetResponse(ChatHistory inputPrompt, ConcurrentBag<AgentResult> agentResponses, TimeSpan elapsedTime)
    {
        if (inputPrompt == null) 
            throw new ArgumentNullException(nameof(inputPrompt));
        
        if (agentResponses == null) 
            throw new ArgumentNullException(nameof(agentResponses));

        var inputPromptAsText = inputPrompt
            .GetPromptAsText();

        var agentResults = agentResponses
            .ToArray();

        var tokenUsage = agentResponses
            .Select(x => x.TokenUsage)
            .Aggregate(new TokenUsage(), (current, x) => current + x);

        return new AgentResponse
        {
            InputPrompt = inputPromptAsText,
            Results = agentResults,
            TokenUsage = tokenUsage,
            ElapsedTime = elapsedTime
        };
    }
    private static AgentResult GetAgentResult(ChatMessageContent chatMessageContent, TimeSpan elapsedTime)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (string.IsNullOrEmpty(chatMessageContent.Content))
        {
            throw new AiException("No Content returned by the request.");
        }

        var answer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var response = JsonConvert.DeserializeObject<AgentResult>(answer, Settings.ResponseSerializerSettings);

        if (response.ErrorMessage != null)
        {
            throw new AiException(response.ErrorMessage);
        }

        var thinking = chatMessageContent.Content
            .GetChatResponseThinking();

        var tokenUsage = chatMessageContent
            .GetTokenUsage();

        var externalId = chatMessageContent
            .GetExternalId();

        response.AgentId = chatMessageContent.AuthorName;
        response.Thinking = thinking;
        response.RawResponse = chatMessageContent.Content;
        response.TokenUsage = tokenUsage;
        response.ExternalId = externalId;
        response.ElapsedTime = elapsedTime;

        return response;
    }

    private static ValueTask ResponseCallback(ChatMessageContent chatMessageContent, Stopwatch stopwatch, ConcurrentBag<AgentResult> agentResponses)
    {
        if (chatMessageContent == null) 
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (agentResponses == null) 
            throw new ArgumentNullException(nameof(agentResponses));

        if (stopwatch == null) 
            throw new ArgumentNullException(nameof(stopwatch));

        var response = AgentService.GetAgentResult(chatMessageContent, stopwatch.Elapsed);

        agentResponses
            .Add(response);

        return ValueTask.CompletedTask;
    }
    private static ValueTask<IEnumerable<ChatMessageContent>> InputTransform(ChatHistory inputPrompt)
    {
        if (inputPrompt == null)
            throw new ArgumentNullException(nameof(inputPrompt));

        return new ValueTask<IEnumerable<ChatMessageContent>>(inputPrompt);
    }
    private static ValueTask<ChatMessageContent[]> ResultTransform(IList<ChatMessageContent> contents, CancellationToken cancellationToken = default)
    {
        if (contents == null) 
            throw new ArgumentNullException(nameof(contents));

        var result = contents
            .ToArray();

        return ValueTask.FromResult(result);
    }
}