using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Chat Options (nested class).
/// </summary>
public class ChatOptions
{
    /// <summary>
    /// The chat model name.
    /// Make sure the model is configured in the choosen AI provider (e.g. Azure AI, Azure OpenAU, Ollama, etc).
    /// </summary>
    [Required]
    public virtual ChatModel Model { get; set; } = new();

    /// <summary>
    /// Timeout in seconds before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Options to configure the memory recall from past current or conversations.
    /// </summary>
    public virtual MemoryOptions Memory { get; set; } = new();

    /// <summary>
    /// Options to configure persistant knowledge.
    /// </summary>
    public virtual KnowledgeOptions Knowledge { get; set; } = new();

    /// <summary>
    /// A collection of plugin type names to be added to the kernel from configuration.
    /// <para>
    /// Each string in this collection must represent a valid Semantic Kernel plugin type. 
    /// A valid plugin type is a class that contains at least one method annotated with 
    /// <see cref="KernelFunctionAttribute"/>. Methods can optionally include a 
    /// <see cref="DescriptionAttribute"/>, which provides the LLM with detailed guidance 
    /// about the function’s purpose, expected parameters, and usage.
    /// </para>
    /// <para>
    /// <b>Requirements and best practices:</b>
    /// <list type="bullet">
    /// <item>
    /// Any dependencies required by the plugin's constructor must be registered in 
    /// <see cref="IServiceCollection"/> <b>before</b> the plugin is instantiated, otherwise 
    /// registration will fail.
    /// </item>
    /// <item>
    /// If the plugin has multiple constructors, the constructor with the fewest parameters 
    /// (simplest) will be selected. Best practice is to define a single constructor.
    /// </item>
    /// <item>
    /// Provide detailed descriptions for plugin methods (using <see cref="DescriptionAttribute"/>) 
    /// to help the LLM understand their purpose and usage. Few-shot examples, parameter guidance, 
    /// and recommendations on when to use the function can improve AI behavior.
    /// </item>
    /// <item>
    /// Since most LLMs have been trained with Python for function calling, it is recommended 
    /// to use <c>snake_case</c> for function names and property names.
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>AppSettings.json example:</b>
    /// <code>
    /// {
    ///   "Chat": {
    ///     "Plugins": [
    ///       "MyNamespace.MyPlugin, MyAssembly"
    ///     ]
    ///   }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// At runtime, these type names will be resolved to <see cref="Type"/> instances. Their constructors 
    /// will be invoked with dependencies resolved from the kernel's service provider. 
    /// If a type cannot be found or dependencies cannot be resolved, an exception will be thrown.
    /// </para>
    /// </summary>
    public virtual IEnumerable<string> Plugins { get; set; } = [];

    /// <summary>
    /// Memory Options (nested class).
    /// </summary>
    public class MemoryOptions
    {
        /// <summary>
        /// How far back memories will be included in queries when chatting.
        /// </summary>
        [Required]
        public virtual int RetentionInDays { get; set; } = 180;

        /// <summary>
        /// Specifies the maximum number of results to return when searching for embeddings.
        /// Note: The vector store retrieves twice this number to ensure sufficient context after duplicate entries are removed.  
        /// Make sure the limit is set high enough when index-time deduplication is enabled.
        /// </summary>
        [Required]
        public virtual int ContextQueryLimit { get; set; } = 3;

        /// <summary>
        /// The maximum number of results to return when searching for counterpart vector matches of questions and answers.
        /// </summary>
        [Required]
        public virtual int CounterpartContextQueryLimit { get; set; } = 2;

        /// <summary>
        /// Whether to deduplicate results before building the memory context for the chat prompt.
        /// Deduplication will remove similar results, that has a 95+ similary score for Fuzzy comparison.
        /// </summary>
        [Required]
        public virtual bool UseQueryDeduplication { get; set; } = true;

        /// <summary>
        /// The matchs score threshold for deduplicating similar memory results,
        /// when building the memory part of the chat prompt.
        /// </summary>
        public virtual double DeduplicationMatchScoreThreshold { get; set; } = 0.90;
    }

    /// <summary>
    /// Knowledge Options (nested class).
    /// </summary>
    public class KnowledgeOptions
    {
        /// <summary>
        /// Specifies the maximum number of results to return when searching for embeddings.
        /// Note: The vector store retrieves twice this number to ensure sufficient context after duplicate entries are removed.  
        /// Make sure the limit is set high enough when index-time deduplication is enabled.
        /// </summary>
        [Required]
        public virtual int ContextQueryLimit { get; set; } = 3;

        /// <summary>
        /// Whether to deduplicate results before building the knoweldge context for the chat prompt.
        /// Deduplication will remove similar results, that has a 95+ similary score for Fuzzy comparison.
        /// </summary>
        [Required]
        public virtual bool UseQueryDeduplication { get; set; } = true;

        /// <summary>
        /// The matchs score threshold for deduplicating similar knowledge results,
        /// when building the knowledge part of the chat prompt.
        /// </summary>
        public virtual double DeduplicationMatchScoreThreshold { get; set; } = 0.90;
    }
}