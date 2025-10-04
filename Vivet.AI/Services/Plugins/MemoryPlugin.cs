using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Vivet.AI.Config;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Embedding.Memory;
using Vivet.AI.Services.Requests.Embedding.Memory.Models;

namespace Vivet.AI.Services.Plugins;

/// <summary>
/// Memory Plugin
/// </summary>
public sealed class MemoryPlugin
{
    private readonly MemoryPluginOptions options;
    private readonly IEmbeddingMemoryService embeddingMemoryService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">See <see cref="MemoryPluginOptions"/>.</param>
    /// <param name="embeddingMemoryService">The <see cref="IEmbeddingMemoryService"/>.</param>
    public MemoryPlugin(MemoryPluginOptions options, IEmbeddingMemoryService embeddingMemoryService)
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
    /// <param name="currentThreadId">The thread id of the current conversation.</param>
    /// <returns>The memory chat prompt snippet.</returns>
    [KernelFunction("memory")]
    [Description(@"Retrieve relevant user-specific memories, including past questions, answers, notes, or uploaded content, 
and inject them into the current chat context to support personalized and consistent responses.")]
    public async Task<string> GetMemoriesAsync([Description("The current user question or message")]string question, Guid? userId, Guid? agentId, Guid? scopeId, Guid? currentThreadId) 
    {
        if (string.IsNullOrEmpty(question))
        {
            return null;
        }

        if (userId == null && agentId == null)
        {
            return null;
        }

        try
        {
            var from = DateTimeOffset.UtcNow
                .AddDays(-this.options.RetentionInDays);

            var limit = this.options.UseQueryDeduplication
                ? this.options.ContextQueryLimit * 2
                : this.options.ContextQueryLimit;

            var request = new SearchMemoryRequest
            {
                Query = question,
                CurrentThreadId = currentThreadId,
                Criteria = new MemoryCriteria
                {
                    UserId = userId,
                    AgentId = agentId,
                    ScopeId = scopeId,
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

            stringBuilder
                .AppendLine("[MEMORY]");

            if (memoryResults.Any())
            {
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
            }
            else
            {
                stringBuilder
                    .AppendLine("None found.");
            }

            return stringBuilder
                .ToString();
        }
        catch (Exception ex)
        {
            return $"An error occurred. {ex.Message}";
        }
    }
}