using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Services.Consts;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Services.Collectors.Models;

internal class FunctionCallCollector
{
    private readonly List<FunctionCall> results = [];

    internal virtual IEnumerable<FunctionCall> GetAll()
    {
        return this.results;
    }

    internal virtual IEnumerable<FunctionCall> GetByAgentId(string agentId)
    {
        if (agentId == null) 
            throw new ArgumentNullException(nameof(agentId));
        
        return this.results
            .Where(x => x.AgentId == agentId);
    }

    internal virtual void Add(FunctionInvocationContext functionInvocationContext)
    {
        if (functionInvocationContext == null) 
            throw new ArgumentNullException(nameof(functionInvocationContext));

        var agentId = functionInvocationContext.Kernel.Data[KernelData.AGENT_ID]?
            .ToString();

        var pluginName = functionInvocationContext.Function.PluginName;
        var functionName = functionInvocationContext.Function.Name;

        var result = new FunctionCallResult
        {
            Type = functionInvocationContext.Result.ValueType,
            Result = functionInvocationContext.Result
                .GetValue<object>()
        };

        var arguments = functionInvocationContext.Arguments
            .ToDictionary();

        this.results
            .Add(new FunctionCall
            {
                AgentId = agentId,
                PluginName = pluginName,
                FunctionName = functionName,
                Result = result,
                Arguments = arguments
            });
    }

    internal virtual void AddOrUpdate(ChatMessageContent chatMessageContent, FunctionCallContent functionCallContent)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        if (functionCallContent == null) 
            throw new ArgumentNullException(nameof(functionCallContent));

        var id = functionCallContent.Id;
        var pluginName = functionCallContent.PluginName;
        var functionName = functionCallContent.FunctionName;
        var exception = functionCallContent.Exception;

        var agentId = chatMessageContent
            .GetAgentId();

        var functionCall = this.results
            .FirstOrDefault(x => x.Id == null && x.PluginName == pluginName && x.FunctionName == functionName && x.AgentId == agentId);

        if (functionCall == null)
        {
            var arguments = functionCallContent.Arguments?
                .ToDictionary() ?? [];

            this.results
                .Add(new FunctionCall
                {
                    Id = id,
                    AgentId = agentId,
                    PluginName = pluginName,
                    FunctionName = functionName,
                    Result = null,
                    Arguments = arguments,
                    Exception = exception
                });
        }
        else
        {
            functionCall.Id = id;
            functionCall.Exception = exception;
        }
    }
}