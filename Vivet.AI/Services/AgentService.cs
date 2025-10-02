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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Collectors;
using Vivet.AI.Services.Consts;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Filters;
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

// TODO: Update all chat tests (exception handling has changed)

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
// Document adding filters, and that they will be orderded by name. Filters are added for all kernels.
// Error handling: An exception is now set on BaseResponse if an error happens. For AgentService that is also on each agent. 
// - AIException means and error from the model.

// TODO: Add missing tests (especially extensions)

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

        AgentCollectorContext.Initialize();

        try
        {
            var executionSettings = this.GetPromptExecutionSettingsOverrrides(request.ConfigOverrides);
            var agents = this.GetAgents(request, executionSettings);
            var inputPrompt = await this.GetInputPrompt(request, cancellationToken);
            var agentOrchestration = this.GetAgentOrchestration(request, inputPrompt, agents, stopwatch);

            var inputPromptAsText = inputPrompt
                .GetPromptAsText(true);

            await this.StartAgentProcessAsync(cancellationToken);

            var orchestrationResult = await agentOrchestration
                .InvokeAsync(inputPromptAsText, this.agenticProcess, cancellationToken);

            await orchestrationResult
                .GetValueAsync(options.Timeout, cancellationToken);

            stopwatch
                .Stop();

            var response = AgentService.GetResponse(inputPrompt, agents, stopwatch.Elapsed);

            // TODO: HISTORY: Save memory, SkipSaveMemory
            // Consider that AgentId here should be the AgentDescriptor.Name, if we want memory across agent executions.

            return response;
        }
        finally
        {
            AgentCollectorContext.Dispose();
        }
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

        kernel.Data
            .Add(KernelData.AGENT_ID, agent.Id);

        kernel.FunctionInvocationFilters
            .Add(new FunctionCallCollectorFilter());
        
        kernel
            .AddBuiltInPluginConfigOverrides(agent.ConfigOverrides.Plugins, parentConfigOverrides)
            .AddCustomPlugins(serviceProvider, agent.Plugins.CustomPlugins);

        kernel.Plugins
            .ValidateContext(agent.Plugins.Context, request.Plugins.Context);

        return kernel;
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

            chatHistory
                .AddChatSystemPrompt<string>(agentDescriptor.Instructions)
                .AddAgentPluginsContextPrompt(agentDescriptor.Plugins, request.Plugins);

            var instructions = chatHistory
                .GetPromptAsText(true);

            var name = $"{agentDescriptor.Name.Replace(" ", "-")}[{agentDescriptor.Id}]";

            agents
                .Add(new ChatCompletionAgent
                {
                    Id = agentDescriptor.Id,
                    Name = name,
                    Description = agentDescriptor.Description,
                    Instructions = instructions,
                    InstructionsRole = agentDescriptor.Role,
                    Kernel = kernel,
                    Arguments = new KernelArguments(executionSettings),
                    LoggerFactory = kernel.LoggerFactory,
                    // TODO: HISTORY: setting? ChatHistoryTruncationReducer / ChatHistorySummarizationReducer / LastMessage / None
                    // make seetting to only pass the latest agent messagage along
                    HistoryReducer = null, // HISTORY: 000: What is AgentChat => DOC: The reducer is automatically applied to the history before invoking the agent, only when using an <see cref="AgentChat"/>. It must be explicitly applied via <see cref="ReduceAsync"/>. 
                    Template = null,
                    UseImmutableKernel = false
                });
        }

        return agents
            .ToArray();
    }
    private AgentOrchestration<string, ChatMessageContent[]> GetAgentOrchestration(AgentRequest request, ChatHistory inputPrompt, Agent[] agents, Stopwatch stopWatch)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (agents == null)
            throw new ArgumentNullException(nameof(agents));

        if (stopWatch == null) 
            throw new ArgumentNullException(nameof(stopWatch));

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
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, stopWatch)
            },
            AgentOrchestrationType.Concurrent => new ConcurrentOrchestration<string, ChatMessageContent[]>(agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, stopWatch)
            },
            AgentOrchestrationType.GroupChat => new GroupChatOrchestration<string, ChatMessageContent[]>(null, agents) // TODO: Group chat Orchestration. Manager Override. https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-orchestration/group-chat?pivots=programming-language-csharp#customize-the-group-chat-manager
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, stopWatch)
            },
            AgentOrchestrationType.HandOff => new HandoffOrchestration<string, ChatMessageContent[]>(OrchestrationHandoffs.StartWith(agents.First()), agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, stopWatch)
            },
            AgentOrchestrationType.Magnetic => new MagenticOrchestration<string, ChatMessageContent[]>(null, agents) // TODO: Magnetic Orchestration
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, stopWatch)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request.OrchestrationType), request.OrchestrationType, $"Orchestration type {request.OrchestrationType} not supported")
        };

        return agentOrchestration;
    }

    private static AgentResponse GetResponse(ChatHistory inputPrompt, Agent[] agents, TimeSpan elapsedTime)
    {
        if (inputPrompt == null) 
            throw new ArgumentNullException(nameof(inputPrompt));

        if (agents == null)
            throw new ArgumentNullException(nameof(agents));

        var responseCallbacks = AgentCollectorContext.ResponseCallbacks
            .GetAll();

        var results = responseCallbacks
            .Select(x =>
            {
                var agentId = x.ChatMessageContent
                    .GetAgentId();

                var agent = agents
                    .First(y => y.Id == agentId);

                return AgentService.GetAgentResult(x.ChatMessageContent, agent, x.ElapsedTime);
            })
            .ToArray();

        var inputPromptAsText = inputPrompt
            .GetPromptAsText();

        var tokenUsage = results
            .Select(x => x.TokenUsage)
            .Aggregate(new TokenUsage(), (current, x) => current + x);

        return new AgentResponse
        {
            InputPrompt = inputPromptAsText,
            Results = results,
            TokenUsage = tokenUsage,
            ElapsedTime = elapsedTime
        };
    }
    private static AgentResult GetAgentResult(ChatMessageContent chatMessageContent, Agent agent, TimeSpan elapsedTime)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (agent == null) 
            throw new ArgumentNullException(nameof(agent));

        var agentId = chatMessageContent
            .GetAgentId();

        var agentInstructions = AgentCollectorContext.Instructions
            .GetByAgent(agentId);

        var stringBuilder = new StringBuilder(agent.Instructions);

        foreach (var instruction in agentInstructions)
        {
            stringBuilder
                .AppendLine(instruction.Role)
                .AppendLine(instruction.Input);
        }

        var instructionsPrompt = stringBuilder
            .ToString();

        var tokenUsage = chatMessageContent
            .GetTokenUsage();

        var externalId = chatMessageContent
            .GetExternalId();

        if (string.IsNullOrEmpty(chatMessageContent.Content))
        {
            var noContentException = BaseService.GetResponseExceptionOrDefault("No Content returned by the request.");

            return new AgentResult
            {
                AgentId = agentId,
                InstructionsPrompt = instructionsPrompt,
                ElapsedTime = elapsedTime,
                TokenUsage = tokenUsage,
                ExternalId = externalId,
                Exception = noContentException
            };
        }

        var answer = chatMessageContent.Content
            .GetChatResponseAnswer();

        var result = JsonConvert.DeserializeObject<AgentResult>(answer, Settings.ResponseSerializerSettings);

        var thinking = chatMessageContent.Content
            .GetChatResponseThinking();

        var functionCalls = AgentCollectorContext.Functions
            .GetByAgentId(agentId);

        var exception = BaseService.GetResponseExceptionOrDefault(result.ErrorMessage);

        result.AgentId = agentId;
        result.Thinking = thinking;
        result.RawResponse = chatMessageContent.Content;
        result.InstructionsPrompt = instructionsPrompt;
        result.ElapsedTime = elapsedTime; // TODO: Test that this isn't cumulattive in sequential orchestrations
        result.TokenUsage = tokenUsage;
        result.ExternalId = externalId;
        result.FunctionCalls = functionCalls;
        result.Exception = exception;

        return result;
    }

    private static ValueTask ResponseCallback(ChatMessageContent chatMessageContent, Stopwatch stopwatch)
    {
        if (chatMessageContent == null) 
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (stopwatch == null) 
            throw new ArgumentNullException(nameof(stopwatch));

        var finishReason = chatMessageContent.Metadata?["FinishReason"].ToString()?.ToLower();

        if (finishReason == "stop")
        {
            AgentCollectorContext.ResponseCallbacks
                .AddResponseCallback(chatMessageContent, stopwatch.Elapsed);
        }
        else
        {
            switch (finishReason)
            {
                case "toolcalls":
                {
                    var kernelContent = chatMessageContent.Items
                        .FirstOrDefault();

                    if (kernelContent is FunctionCallContent functionCallContent)
                    {
                        AgentCollectorContext.Functions
                            .AddOrUpdate(chatMessageContent, functionCallContent);
                    }

                    chatMessageContent.Content ??= $"[{nameof(FunctionCallContent)}]";
                    break;
                }

                case null when chatMessageContent.Role == AuthorRole.Tool:
                    chatMessageContent.Content += $"{Environment.NewLine}{nameof(FunctionResultContent)}";
                    break;
            }

            AgentCollectorContext.Instructions
                .Add(chatMessageContent);
        }

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