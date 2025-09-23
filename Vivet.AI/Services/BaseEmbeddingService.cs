using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.AI;
using Vivet.AI.Config;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services;

/// <summary>
/// Provides a base class for embedding services that generate embeddings from text chunks using a configured embedding generator.
/// </summary>
public abstract class BaseEmbeddingService(EmbeddingOptions options, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IMetadataService metadataService = null)
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

    /// <summary>
    /// The embedding options that configure how embeddings are generated.
    /// </summary>
    protected readonly EmbeddingOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// The metadata service used to extract or manage metadata associated with embeddings.
    /// </summary>
    protected readonly IMetadataService metadataService = metadataService;

    /// <summary>
    /// Generates embeddings for a collection of text chunks asynchronously.
    /// </summary>
    /// <param name="textChunks">The array of text chunks to generate embeddings for.</param>
    /// <param name="embedingConfigOverrides">Embedding Cconfig overrides.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the generated embeddings./// </returns>
    protected virtual async Task<GeneratedEmbeddings<Embedding<float>>> GenerateEmbeddings(string[] textChunks, EmbedingConfigOverrides embedingConfigOverrides, CancellationToken cancellationToken = default)
    {
        if (textChunks == null)
            throw new ArgumentNullException(nameof(textChunks));

        if (!textChunks.Any())
        {
            return [];
        }

        var generationOptions = new EmbeddingGenerationOptions
        {
            ModelId = embedingConfigOverrides?.ModelName
        };

        return await this.embeddingGenerator
            .GenerateAsync(textChunks, generationOptions, cancellationToken)
            .ConfigureAwait(false);
    }
}