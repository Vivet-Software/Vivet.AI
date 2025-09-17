using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Embedding.Knowledge;

namespace Vivet.AI.Plugins;

/// <summary>
/// Chat Knowledge Plugin.
/// </summary>
public sealed class ChatKnowledgePlugin
{
    private readonly ChatKnowledgePluginOptions options;
    private readonly IEmbeddingKnowledgeService embeddingKnowledgeService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="ChatKnowledgePluginOptions"/>.</param>
    /// <param name="embeddingKnowledgeService">The <see cref="IEmbeddingKnowledgeService"/>.</param>
    public ChatKnowledgePlugin(ChatKnowledgePluginOptions options, IEmbeddingKnowledgeService embeddingKnowledgeService)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.embeddingKnowledgeService = embeddingKnowledgeService ?? throw new ArgumentNullException(nameof(embeddingKnowledgeService));
    }

    /// <summary>
    /// Retrieve and inject relevant knowledge entries into the current chat history.
    /// </summary>
    /// <param name="question">The current user question or message.</param>
    /// <param name="scopeId">The scope id.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="subTenantId">The sub-tenant id.</param>
    /// <returns>The knowledge chat prompt snippet.</returns>
    [KernelFunction("knowledge")] 
    [Description(@"Retrieve knowledge stored in private or scoped sources such as notes, documents, organizational records or similar. 
Always use this function when the user’s request may relate to these sources, even if similar information exists in public knowledge.")]
    public async Task<string> GetKnowledgeAsync([Description("The current user question or message")]string question, string scopeId, string tenantId, string subTenantId)
    {
        if (string.IsNullOrEmpty(question))
        {
            return null;
        }

        scopeId = string.IsNullOrEmpty(scopeId) ? null : scopeId;
        tenantId = string.IsNullOrEmpty(tenantId) ? null : tenantId;
        subTenantId = string.IsNullOrEmpty(subTenantId) ? null : subTenantId;

        var limit = this.options.UseQueryDeduplication
            ? this.options.ContextQueryLimit * 2
            : this.options.ContextQueryLimit;

        var response = await this.embeddingKnowledgeService
            .SearchAsync(new SearchKnowledgeRequest
            {
                Query = question,
                Criteria =
                {
                    TenantId = tenantId,
                    SubTenantId = subTenantId,
                    ScopeId = scopeId
                },
                Limit = limit
            })
            .ConfigureAwait(false);

        var knowledgeResults = response.Results
            .Select(x => x.Result)
            .ToArray();

        if (this.options.UseQueryDeduplication)
        {
            var deduplicatedResults = ContextDeduplicator.DeduplicateKnowledgeResults(knowledgeResults, this.options.DeduplicationMatchScoreThreshold);

            knowledgeResults = deduplicatedResults
                .Take(this.options.ContextQueryLimit)
                .ToArray();
        }

        var stringBuilder = new StringBuilder();

        stringBuilder
            .AppendLine("[KNOWLEDGE]");

        if (knowledgeResults.Any())
        {
            foreach (var knowledgeResult in knowledgeResults)
            {
                if (knowledgeResult.Source != null)
                {
                    stringBuilder
                        .AppendLine($"system: {knowledgeResult.Source}");
                }

                stringBuilder
                    .AppendLine($"assistant: {knowledgeResult.FullContext}");

                if (knowledgeResult.Blob != null)
                {
                    var dataUri = knowledgeResult.Blob
                        .GetDataUri();

                    stringBuilder
                        .AppendLine($"assistant: [Blob: {dataUri}]");

                    if (knowledgeResult.BlobMetadata != null)
                    {
                        var metadataProperties = knowledgeResult.BlobMetadata
                            .GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Select(x => new
                            {
                                Key = x.Name,
                                Value = x.GetValue(knowledgeResult.BlobMetadata)
                            })
                            .Select(x => $"{x.Key}={x.Value ?? "N/A"}");

                        var metadataContent = string.Join(", ", metadataProperties);

                        stringBuilder
                            .AppendLine($"assistant: {metadataContent}");
                    }
                }

                stringBuilder
                    .AppendLine($"system: {knowledgeResult.CreatedAt:D}");
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
}