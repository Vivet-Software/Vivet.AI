using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Embedding Options (nested class).
/// </summary>
public class EmbeddingOptions
{
    /// <summary>
    /// The embedding model name.
    /// Make sure the model is configured in the choosen AI provider (e.g. Azure AI, Azure OpenAU, Ollama, etc).
    /// </summary>
    [Required]
    public virtual EmbeddingModel Model { get; set; } = new();

    /// <summary>
    /// Vector size (embedding dimension) depends entirely on the embedding model you're using.
    /// Check the documentation of your model.
    /// </summary>
    [Required]
    public virtual int VectorSize { get; set; } = 1536;

    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    [Required]
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Options to configure the memory recall from past current or conversations.
    /// </summary>
    public virtual EmbeddingMemoryOptions Memory { get; set; }

    /// <summary>
    /// Options to configure persistant knowledge.
    /// </summary>
    public virtual EmbeddingKnowledgeOptions Knowledge { get; set; }
}