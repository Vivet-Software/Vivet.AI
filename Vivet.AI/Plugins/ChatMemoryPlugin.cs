using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Requests.Embedding.Memory.Models;

namespace Vivet.AI.Plugins;

/// <summary>
/// Memory Plugin
/// </summary>
public sealed class ChatMemoryPlugin
{
    private readonly ChatMemoryPluginOptions options;
    private readonly IEmbeddingMemoryService embeddingMemoryService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">See <see cref="ChatMemoryPluginOptions"/>.</param>
    /// <param name="embeddingMemoryService">The <see cref="IEmbeddingMemoryService"/>.</param>
    public ChatMemoryPlugin(ChatMemoryPluginOptions options, IEmbeddingMemoryService embeddingMemoryService)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.embeddingMemoryService = embeddingMemoryService ?? throw new ArgumentNullException(nameof(embeddingMemoryService));
    }

    /// <summary>
    /// Retrieve and inject relevant memory entries into the current chat history.
    /// </summary>
    /// <param name="question">The current user question or message.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="scopeId">The scope id.</param>
    /// <param name="agentId">The agent id.</param>
    /// <param name="threadId">The thread id.</param>
    /// <returns>The memory chat prompt snippet.</returns>
    [KernelFunction]
    [Description("Retrieve and inject relevant memory entries into the current chat history.")]
    public async Task<string> GetMemoriesAsync([Description("The current user question or message")]string question, string userId, string scopeId, string agentId, string threadId) 
    {
        if (string.IsNullOrEmpty(question))
        {
            return null;
        }

        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        scopeId = string.IsNullOrEmpty(scopeId) ? null : scopeId;
        agentId = string.IsNullOrEmpty(agentId) ? null : agentId;
        threadId = string.IsNullOrEmpty(threadId) ? null : threadId;

        var from = DateTimeOffset.UtcNow
            .AddDays(-this.options.RetentionInDays);

        var limit = this.options.UseQueryDeduplication
            ? this.options.ContextQueryLimit * 2
            : this.options.ContextQueryLimit;

        var request = new SearchMemoryRequest
        {
            Query = question,
            CurrentThreadId = threadId,
            Criteria = new MemoryCriteria
            {
                UserId = userId,
                ScopeId = scopeId,
                AgentId = agentId,
                DateRange = new DateRange
                {
                    From = from
                }
            },
            Limit = limit
        };

        var response = await this.embeddingMemoryService
            .SearchAsync(request);

        var memoryResults = response.Results
            .Select(x => x.Result)
            .ToArray();

        if (this.options.UseQueryDeduplication)
        {
            var deduplicatedResults = ContextDeduplicator.DeduplicateMemoryResults(memoryResults, this.options.DeduplicationMatchScoreThreshold);

            memoryResults = deduplicatedResults
                .Take(this.options.ContextQueryLimit)
                .ToArray();
        }

        var stringBuilder = new StringBuilder();

        if (memoryResults.Any())
        {
            stringBuilder
                .AppendLine("[MEMORY]");
        }

        foreach (var memoryResult in memoryResults)
        {
            if (memoryResult.IsQuestion)
            {
                stringBuilder
                    .AppendLine($"user: Q: {memoryResult.FullContext}");

                var counterpartContexts = memoryResult.CounterpartContext
                    .Take(this.options.CounterpartContextQueryLimit);

                foreach (var counterPartContext in counterpartContexts)
                {
                    stringBuilder
                        .AppendLine($"Assistant: A: {counterPartContext}");
                }
            }
            else if (memoryResult.IsAnswer)
            {
                var counterpartContexts = memoryResult.CounterpartContext
                    .Take(this.options.CounterpartContextQueryLimit);

                foreach (var counterpartContext in counterpartContexts)
                {
                    stringBuilder
                        .AppendLine($"user: Q: {counterpartContext}");
                }

                stringBuilder
                    .AppendLine($"assistant: A: {memoryResult.FullContext}");
            }

            if (memoryResult.Blob is not null)
            {
                var dataUri = memoryResult.Blob
                    .GetDataUri();

                stringBuilder
                    .AppendLine($"user: [Blob: {dataUri}]");
            }

            stringBuilder
                .AppendLine($"system: (Date: {memoryResult.CreatedAt:D})");

            stringBuilder
                .AppendLine();
        }

        return stringBuilder
            .ToString();
    }
}