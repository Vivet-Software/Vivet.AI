using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Embedding.Knowledge;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

namespace Vivet.AI.Services.Plugins;

/// <summary>
/// Knowledge Plugin.
/// </summary>
public sealed class KnowledgePlugin
{
    private readonly IEmbeddingKnowledgeService embeddingKnowledgeService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="embeddingKnowledgeService">The <see cref="IEmbeddingKnowledgeService"/>.</param>
    public KnowledgePlugin(IEmbeddingKnowledgeService embeddingKnowledgeService)
    {
        this.embeddingKnowledgeService = embeddingKnowledgeService ?? throw new ArgumentNullException(nameof(embeddingKnowledgeService));
    }

    /// <summary>
    /// Retrieve and inject relevant knowledge entries into the current chat history.
    /// </summary>
    /// <param name="question">The current user question or message.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="subTenantId">The sub-tenant id.</param>
    /// <param name="scopeId">The scope id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="configOverrides">The config overrides from the request.</param>
    /// <returns>The knowledge chat prompt snippet.</returns>
    [KernelFunction("knowledge")] 
    [Description(@"Retrieve knowledge stored in private or scoped sources such as notes, documents, organizational records or similar. 
Always use this function when the user’s request may relate to these sources, even if similar information exists in public knowledge.")]
    public async Task<string> GetKnowledgeAsync([Description("The current user question or message")]string question, Guid? tenantId, Guid? subTenantId, Guid? scopeId, Guid? userId, EmbeddingKnowledgeSearchConfigOverrides configOverrides)
    {
        if (string.IsNullOrEmpty(question))
        {
            return null;
        }

        try
        {
            var response = await this.embeddingKnowledgeService
                .SearchAsync(new SearchKnowledgeRequest
                {
                    Query = question,
                    Criteria =
                    {
                        TenantId = tenantId,
                        SubTenantId = subTenantId,
                        ScopeId = scopeId,
                        UserId = userId
                    },
                    ConfigOverrides = configOverrides
                })
                .ConfigureAwait(false);

            var knowledgeResults = response.Results
                .Select(x => x.Result)
                .ToArray();

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
        catch (Exception ex)
        {
            return $"An error occurred. {ex.Message}";
        }
    }
}