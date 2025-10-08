using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Agent.Models.Plugins.Context;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Requests.Embedding.Memory.Models;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

namespace Vivet.AI.Services.Plugins;

/// <summary>
/// Memory Plugin
/// </summary>
public sealed class AgentsMemoryPlugin : BaseMemoryPlugin
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="embeddingMemoryService">The <see cref="IEmbeddingMemoryService"/>.</param>
    public AgentsMemoryPlugin(IEmbeddingMemoryService embeddingMemoryService)
        : base(embeddingMemoryService)
    {
    }

    /// <summary>
    /// Retrieve and inject relevant memory entries into the current chat history.
    /// </summary>
    /// <param name="question">The current user question or message.</param>
    /// <param name="context">The context related to memory.</param>
    /// <param name="configOverrides">The config overrides from the request.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The memory chat prompt snippet.</returns>
    [KernelFunction("memory")]
    [Description(@"Retrieve relevant user-specific memories, including past questions, answers, notes, or uploaded content, 
and inject them into the current chat context to support personalized and consistent responses.")]
    public async Task<string> GetMemoriesAsync([Description("The current user question or message")]string question, AgentsMemoryContext context, MemorySearchConfigOverrides configOverrides, CancellationToken cancellationToken = default)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));

        if (configOverrides == null) 
            throw new ArgumentNullException(nameof(configOverrides));

        if (string.IsNullOrEmpty(question))
        {
            return null;
        }

        if (context.AgentId == Guid.Empty)
        {
            return null;
        }

        try
        {
            var request = new SearchMemoryRequest
            {
                Query = question,
                CurrentThreadId = context.CurrentThreadId,
                Criteria = new MemorySearchCriteria
                {
                    UserId = context.UserId,
                    AgentId = context.AgentId,
                    ScopeId = context.ScopeId
                },
                ConfigOverrides = configOverrides
            };

            return await base.GetMemoriesAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return $"An error occurred. {ex.Message}";
        }
    }
}