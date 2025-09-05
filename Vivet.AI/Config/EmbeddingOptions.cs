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
    /// The threshold for cosinus similarity marching.
    /// The threshold value highly depends on the chosen embedding model and preference.
    /// A higher match score mean a greater semantic match.
    /// 0.00 - 0.70: Often noise, unless your domain is very narrow.
    /// 0.70 - 0.80: Related but not identical. (useful for brainstorming or looser recall).
    /// 0.80 – 0.85: Good semantic match (typical retrieval threshold).
    /// 0.90+: Very strong / near-duplicate matches.
    /// </summary>
    [Required]
    public virtual float MatchScoreThreashold { get; set; } = 0.86F; 

    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    [Required]
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Options to configure the memory recall from past current or conversations.
    /// </summary>
    public virtual MemoryOptions Memory { get; set; }

    /// <summary>
    /// Options to configure persistant knowledge.
    /// </summary>
    public virtual KnowledgeOptions Knowledge { get; set; }

    /// <summary>
    /// Memory Options (nested class).
    /// </summary>
    public class MemoryOptions
    {
        /// <summary>
        /// Use extended context when retrieving memories.
        /// This will enable counterpart lookups of questions and answers. When the current question matches a previously memorized question,
        /// the corresponding memorized answer is searched. If a relevant match is found between the memorized question and its answer,
        /// the matching part of the answer is included in the context.
        /// Basically, the LLM will know what answer it gave last time you asked a similar question, and the other way around for answers.
        /// Enabling this will inrich the chat context and imrove accuracy and precision, but will also use more input tokens. 
        /// </summary>
        [Required]
        public virtual bool UseExtendedMemoryContext { get; set; } = true;

        /// <summary>
        /// Configuration for automatically to retrieve metadata for blobs when saving to memory.
        /// This will use the configured metadata chat model and incur costs.
        /// It's recommended to enable this, in order to ensure meaningful data for similarity comparison when the memory is later queried.
        /// If disabled metadata must be passed alongisde the blob when invoking the index request.
        /// </summary>
        [Required]
        public virtual bool UseAutomaticMetadataRetrieval { get; set; } = true;

        /// <summary>
        /// Automatically summarize questions and answers.
        /// This will use the configured summarization chat model and incur costs.
        /// </summary>
        [Required]
        public virtual bool UseAutomaticSummarization { get; set; } = false;

        /// <summary>
        /// Options for text chunking.
        /// </summary>
        [Required]
        public virtual TextChunkingOptions TextChunking { get; set; } = new();

        /// <summary>
        /// Search Result scoring configuration. 
        /// </summary>
        public virtual MemoryScoringOptions Scoring { get; set; } = new();

        /// <summary>
        /// Vector store configuration.
        /// </summary>
        [Required]
        public virtual VectorStoreOptions VectorStore { get; set; }
    }

    /// <summary>
    /// Knowledge Options (nested class).
    /// </summary>
    public class KnowledgeOptions
    {
        /// <summary>
        /// Configuration for automatically to retrieve metadata for blobs when saving to knowledge.
        /// This will use the configured metadata chat model and incur costs.
        /// It's recommended to enable this, in order to ensure meaningful data for similarity comparison when the memory is later queried.
        /// If disabled metadata must be passed alongisde the blob when invoking the index request.
        /// </summary>
        [Required]
        public virtual bool UseAutomaticMetadataRetrieval { get; set; } = true;

        /// <summary>
        /// Options for text chunking.
        /// </summary>
        [Required]
        public virtual TextChunkingOptions TextChunking { get; set; } = new();

        /// <summary>
        /// Search Result scoring configuration. 
        /// </summary>
        public virtual KnowledgeScoringOptions Scoring { get; set; } = new();

        /// <summary>
        /// Vector store configuration.
        /// </summary>
        [Required]
        public virtual VectorStoreOptions VectorStore { get; set; }
    }
}