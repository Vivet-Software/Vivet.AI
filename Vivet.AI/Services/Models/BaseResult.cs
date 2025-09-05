using System;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Services.Models;

/// <summary>
/// Serves as an abstract base class for result objects produced from embeddings or related operations.
/// Provides common metadata such as identifiers, content, language, and model information.
/// </summary>
public abstract class BaseResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the result.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the primary content associated with the result.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the full context in which the content appears.
    /// Useful for providing surrounding information.
    /// </summary>
    public string FullContext { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of when the result was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the ordering index of the result.
    /// Can be used to preserve original sequence or ranking.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the language of the content, if available.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the name or identifier of the embedding model
    /// that produced this result.
    /// </summary>
    public string EmbeddingModel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseResult"/> class.
    /// </summary>
    protected BaseResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseResult"/> class
    /// using values from the specified <see cref="BaseEmbedding"/>.
    /// </summary>
    /// <param name="baseEmbedding">The embedding instance to copy values from.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="baseEmbedding"/> is <c>null</c>.
    /// </exception>
    protected BaseResult(BaseEmbedding baseEmbedding)
    {
        if (baseEmbedding == null)
            throw new ArgumentNullException(nameof(baseEmbedding));

        this.Id = baseEmbedding.Id;
        this.Content = baseEmbedding.Content;
        this.FullContext = baseEmbedding.FullContext;
        this.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(baseEmbedding.UnixTimestamp);
        this.Order = baseEmbedding.Order;
        this.Language = baseEmbedding.Language;
        this.EmbeddingModel = baseEmbedding.EmbeddingModel;
    }
}