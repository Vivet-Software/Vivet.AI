using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Embedding.Memory;

namespace Vivet.AI.Services.Plugins;

/// <summary>
/// Base class for different memory plugins.
/// </summary>
public abstract class BaseMemoryPlugin
{
    /// <summary>
    /// The embedding memory service.
    /// </summary>
    protected readonly IEmbeddingMemoryService embeddingMemoryService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="embeddingMemoryService">The <see cref="IEmbeddingMemoryService"/>.</param>
    protected BaseMemoryPlugin(IEmbeddingMemoryService embeddingMemoryService)
    {
        this.embeddingMemoryService = embeddingMemoryService ?? throw new ArgumentNullException(nameof(embeddingMemoryService));
    }

    /// <summary>
    /// Retrieve and inject relevant memory entries into the current chat history.
    /// </summary>
    /// <param name="request">The <see cref="SearchMemoryRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The memory chat prompt snippet.</returns>
    protected async Task<string> GetMemoriesAsync(SearchMemoryRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        
        var response = await this.embeddingMemoryService
            .SearchAsync(request, cancellationToken);

        var memoryResults = response.Results
            .Select(x => x.Result)
            .ToArray();

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

                    foreach (var counterPartContext in memoryResult.CounterpartContext)
                    {
                        stringBuilder
                            .AppendLine($"Assistant: A: {counterPartContext}");
                    }
                }
                else if (memoryResult.IsAnswer)
                {
                    foreach (var counterpartContext in memoryResult.CounterpartContext)
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
}