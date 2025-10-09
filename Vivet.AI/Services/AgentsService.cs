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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions;
using Vivet.AI.Services.Consts;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Agents;
using Vivet.AI.Services.Requests.Agents.Models;
using Vivet.AI.Services.Requests.Agents.Models.ConfigOverrides;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Responses.Agent;
using Vivet.AI.Services.Responses.Agent.Models;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services;



// BUG: Plugins
// Should plugin config be Ai.Plugins?
// Should request.CustomPlugins.Type be a generic parameters instead

// BUG: Readme:
// PLUGINS:
// Link to config options in chat settings table
// Move plugins to own section, re-use in Chat and Agents

// Emphasize the the build-in plugins must have context variables in request or an exception is thrown
// Same type of custom plugins is allowed, as long as they have different names. Mention the built-in plugin names (memory, knowledge, web_search)
// a plugin name can contain only ASCII letters, digits, and underscores
// Plugins must have seperate context variables even when they are re-used among several plugins

// PLUGIN EXAMPLES
// Make request examples with plugins (built-in / Custom)

// Document built in filter (PII Detection and PromptCache (coming features)

// If using complex tyoes in plugins, ensure to pass the context as json (parameterName={json})

/// <inheritdoc cref="IAgentsService"/>
public class AgentsService(AgentsOptions options, IServiceProvider serviceProvider, IKernelBuilder kernelBuilder, PromptExecutionSettings promptExecutionSettings, IEmbeddingMemoryService embeddingMemoryService = null)
    : BaseService, IAgentsService
{
    private bool isAgenticProcessStarted;
    private readonly SemaphoreSlim startLock = new(1, 1);
    private readonly InProcessRuntime agenticProcess = new(); // TODO: Advamced Agent Process Runtime Features (States, Subscriptions, Change Agents, Singlenton DI)

    /// <inheritdoc />
    public virtual async Task<AgentsResponse> InvokeAsync(BaseAgentsRequest request, Func<IList<AgentIndexMemoryResponse>, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        var stopwatch = new Stopwatch();
        stopwatch
            .Start();

        request
            .Validate();

        var executionSettings = this.GetPromptExecutionSettingsOverrrides(request.ConfigOverrides);
        var agents = this.GetAgents(request, executionSettings);
        var inputPrompt = await this.GetInputPrompt(request, cancellationToken);
        var agentOrchestration = this.GetAgentOrchestration(request, inputPrompt, agents);

        var inputPromptAsText = inputPrompt
            .GetPromptAsText(true);

        await this.StartAgentProcessAsync(cancellationToken);

        var orchestrationResult = await agentOrchestration
            .InvokeAsync(inputPromptAsText, this.agenticProcess, cancellationToken);

        await orchestrationResult
            .GetValueAsync(options.Timeout, cancellationToken);

        stopwatch
            .Stop();

        var response = AgentsService.GetResponse(inputPrompt, agents, stopwatch.Elapsed);

        _ = this.SaveMemory(request, response.Results, onMemoryIndexed, cancellationToken)
            .ConfigureAwait(false);

        return response;
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerable<string> InvokeStreamingAsync(BaseAgentsRequest request, Func<IList<AgentIndexMemoryResponse>, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default)
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

    private Kernel GetKernel(BaseAgentsRequest request, AgentDescriptor agent, BaseChatConfigOverrides parentConfigOverrides)
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
            .Add(KernelData.FUNCTION_CALLS, new List<AutoFunctionInvocationContext>());

        kernel.Data
            .Add(KernelData.AGENT_ID, agent.Id);

        kernel.Data
            .Add(KernelData.AGENT_RESPONSE_CALLBACK, new AgentResponseCallback());

        kernel
            .AddDefaultFilters()
            .RemoveSkippedBuiltInPlugins(agent.ConfigOverrides, parentConfigOverrides)
            .AddCustomPlugins(serviceProvider, agent.Plugins.CustomPlugins);

        kernel.Plugins
            .ValidateContext(agent.Plugins.Context, request.Plugins.Context);

        return kernel;
    }
    private PromptExecutionSettings GetPromptExecutionSettingsOverrrides(AgentsConfigOverrides configOverrides)
    {
        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        var executionSettings = promptExecutionSettings
            .GetOverridePromptExecutionSettings(configOverrides.ModelParameters);

        executionSettings.ModelId = configOverrides.ModelName;

        return executionSettings;
    }
    private async Task<ChatHistory> GetInputPrompt(BaseAgentsRequest request, CancellationToken cancellationToken = default)
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
    private Agent[] GetAgents(BaseAgentsRequest request, PromptExecutionSettings executionSettings)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (promptExecutionSettings == null)
            throw new ArgumentNullException(nameof(promptExecutionSettings));

        var agents = new List<Agent>();
        foreach (var agentDescriptor in request.Agents)
        {
            var kernel = this.GetKernel(request, agentDescriptor, request.ConfigOverrides);

            var chatHistory = new ChatHistory();

            chatHistory
                .AddChatSystemPrompt<string>(agentDescriptor.Instructions)
                .AddAgentPluginsContextPrompt(kernel, agentDescriptor, request);

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
                    HistoryReducer = null, 
                    Template = null,
                    UseImmutableKernel = false
                });
        }

        return agents
            .ToArray();
    }
    private AgentOrchestration<string, ChatMessageContent[]> GetAgentOrchestration(BaseAgentsRequest request, ChatHistory inputPrompt, Agent[] agents)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (inputPrompt == null) 
            throw new ArgumentNullException(nameof(inputPrompt));

        if (agents == null)
            throw new ArgumentNullException(nameof(agents));

        var loggerFactory = agents.FirstOrDefault()?.LoggerFactory ?? serviceProvider.GetService<ILoggerFactory>();

        AgentOrchestration<string, ChatMessageContent[]> agentOrchestration = request switch
        {
            SequentialAgentsRequest => new SequentialOrchestration<string, ChatMessageContent[]>(agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, agents)
            },
            ConcurrentAgentsRequest => new ConcurrentOrchestration<string, ChatMessageContent[]>(agents)
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, agents)
            },
            GroupChatAgentsRequest => new GroupChatOrchestration<string, ChatMessageContent[]>(null, agents) // TODO: Orchestration: Group Chat
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, agents)
            },
            HandOffAgentsRequest => new HandoffOrchestration<string, ChatMessageContent[]>(null, agents) // TODO: Orchestration: Hand Off
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, agents)
            },
            MagneticAgentsRequest => new MagenticOrchestration<string, ChatMessageContent[]>(null, agents) // TODO: Orchestration: Magnetic
            {
                Name = request.Name,
                Description = request.Description,
                LoggerFactory = loggerFactory,
                InputTransform = (_, _) => InputTransform(inputPrompt),
                ResultTransform = ResultTransform,
                ResponseCallback = chatMessageContent => ResponseCallback(chatMessageContent, agents)
            },

            _ => throw new ArgumentOutOfRangeException(nameof(request), request.GetType(), $"Orchestration type {request.GetType()} not supported")
        };

        return agentOrchestration;
    }
   
    private Task SaveMemory<T>(BaseAgentsRequest request, IEnumerable<AgentResult<T>> results, Func<IList<AgentIndexMemoryResponse>, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default)
        where T : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (results == null)
            throw new ArgumentNullException(nameof(results));

        if (embeddingMemoryService == null)
        {
            return Task.CompletedTask;
        }

        return Task.Run(async () =>
        {
            IList<AgentIndexMemoryResponse> indexResponses = new List<AgentIndexMemoryResponse>();

            foreach (var result in results)
            {
                try
                {
                    var indexMemoryResponse = await embeddingMemoryService
                        .IndexAsync(new IndexMemoryRequest<T>
                        {
                            Question = request.Input,
                            Answer = result.Answer,
                            AgentId = result.AgentId,
                            UserId = request.Plugins.Context.Memory.UserId,
                            ThreadId = request.Plugins.Context.Memory.CurrentThreadId,
                            ScopeId = request.Plugins.Context.Memory.ScopeId,
                            Language = result.Language,
                            Blobs = request.Blobs,
                            ConfigOverrides = request.ConfigOverrides.Plugins.Memory.Indexing
                        }, cancellationToken)
                        .ConfigureAwait(false);

                    indexResponses
                        .Add(new AgentIndexMemoryResponse
                        {
                            Result = indexMemoryResponse
                        });
                }
                catch (Exception ex)
                {
                    indexResponses
                        .Add(new AgentIndexMemoryResponse
                        {
                            Exception = ex
                        });
                }
            }

            if (onMemoryIndexed != null)
            {
                await onMemoryIndexed(indexResponses)
                    .ConfigureAwait(false);
            }
        }, cancellationToken);
    }

    private static AgentsResponse GetResponse(ChatHistory inputPrompt, Agent[] agents, TimeSpan elapsedTime)
    {
        if (inputPrompt == null) 
            throw new ArgumentNullException(nameof(inputPrompt));

        if (agents == null)
            throw new ArgumentNullException(nameof(agents));

        var results = agents
            .Select(AgentsService.GetAgentResult)
            .ToArray();

        var inputPromptAsText = inputPrompt
            .GetPromptAsText();

        var tokenUsage = results
            .Select(x => x.TokenUsage)
            .Aggregate(new TokenUsage(), (current, x) => current + x);

        return new AgentsResponse
        {
            InputPrompt = inputPromptAsText,
            Results = results,
            TokenUsage = tokenUsage,
            ElapsedTime = elapsedTime
        };
    }
    private static AgentResult GetAgentResult(Agent agent)
    {
        if (agent == null) 
            throw new ArgumentNullException(nameof(agent));

        var agentId = Guid.Parse(agent.Id);
        var agentResponseCallback = (AgentResponseCallback)agent.Kernel.Data[KernelData.AGENT_RESPONSE_CALLBACK];
        var chatMessageContent = agentResponseCallback.ChatMessageContent;
        var elapsedTime = agentResponseCallback.ElapsedTime;

        var functionCalls = BaseService.GetResponseFunctionCalls(agent.Kernel);
        
        var instructionsPrompt = functionCalls
            .Aggregate(agent.Instructions, (current, agentInstruction) => current + $"{agentInstruction.RenderedPrompt}{Environment.NewLine}");

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

        AgentsService.SetAgentResultAnswer(result, answer);

        var thinking = chatMessageContent.Content
            .GetChatResponseThinking();

        var exception = BaseService.GetResponseExceptionOrDefault(result.ErrorMessage);

        result.AgentId = agentId;
        result.Thinking = thinking;
        result.RawResponse = chatMessageContent.Content;
        result.InstructionsPrompt = instructionsPrompt;
        result.ElapsedTime = elapsedTime; 
        result.TokenUsage = tokenUsage;
        result.ExternalId = externalId;
        result.FunctionCalls = functionCalls;
        result.Exception = exception;

        return result;
    }
    private static void SetAgentResultAnswer<T>(AgentResult<T> response, string responseAnswer)
        where T : class
    {
        if (response == null)
            throw new ArgumentNullException(nameof(response));

        if (responseAnswer == null)
            throw new ArgumentNullException(nameof(responseAnswer));

        var responseType = response
            .GetType();

        var jObject = JsonConvert.DeserializeObject<JObject>(responseAnswer);

        var answerToken = jObject[nameof(AgentResult.Answer)];

        if (response is AgentResult<string> stringResponse)
        {
            if (answerToken != null)
            {
                stringResponse.Answer = answerToken.Type == JTokenType.String
                    ? answerToken.Value<string>()
                    : answerToken.ToString();

                stringResponse.Answer = string.IsNullOrEmpty(stringResponse.Answer)
                    ? null
                    : stringResponse.Answer;
            }
        }
        else if (responseType is { IsGenericType: true } && responseType.GetGenericTypeDefinition() == typeof(AgentResult<>))
        {
            if (answerToken != null)
            {
                var answerType = responseType
                    .GetGenericArguments()[0];

                var answerValue = answerToken.Type == JTokenType.String
                    ? JsonConvert.DeserializeObject(answerToken.Value<string>(), answerType)
                    : answerToken.ToObject(answerType);

                var propertyInfo = responseType
                    .GetProperty(nameof(AgentResult<object>.Answer));

                propertyInfo?
                    .SetValue(response, answerValue);
            }
        }
    }

    private static ValueTask ResponseCallback(ChatMessageContent chatMessageContent, IEnumerable<Agent> agents)
    {
        if (chatMessageContent == null) 
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (agents == null) 
            throw new ArgumentNullException(nameof(agents));

        var finishReason = chatMessageContent
            .GetFinishReason();

        if (finishReason == "Stop")
        {
            var agentId = chatMessageContent
                .GetAgentId();

            var agent = agents
                .FirstOrDefault(x =>
                {
                    var value = x.Kernel.Data[KernelData.AGENT_ID];

                    var strValue = value?
                        .ToString();

                    Guid? kernelAgentId = strValue == null
                        ? null
                        : Guid.Parse(strValue);

                    return kernelAgentId == agentId;
                });

            if (agent != null)
            {
                var createdAt = chatMessageContent
                    .GetCreatedAt();

                var elapsedTime = DateTimeOffset.UtcNow - createdAt ?? TimeSpan.Zero;
                var agentResponseCallback = (AgentResponseCallback)agent.Kernel.Data[KernelData.AGENT_RESPONSE_CALLBACK];

                agentResponseCallback.ChatMessageContent = chatMessageContent;
                agentResponseCallback.ElapsedTime = elapsedTime;
            }
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