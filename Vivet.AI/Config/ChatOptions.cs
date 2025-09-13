using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

// BUG: Built-in Plugins: chat, but also metadata, summarization???

// BUG: Maybe merge Chat.Plugins and Chat.BuiltInPlugins

// BUG: Common Text Search Options
///// <summary>
///// Options which can be applied when using <see cref="ITextSearch"/>.
///// </summary>
//public sealed class TextSearchOptions
//{

//    /// <summary>
//    /// The filter expression to apply to the search query.
//    /// </summary>
//    public TextSearchFilter? Filter { get; init; }
//    /// <summary>
//    /// Number of search results to return.
//    /// </summary>
//    public int Top { get; init; } = 5;
//}


/// <summary>
/// 
/// </summary>
public class GoogleSearchOptions
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual string ApiKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual string SearchEngineId { get; set; }
}

/// <summary>
/// 
/// </summary>
public class BingSearchOptions
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual string ApiKey { get; set; }
}

/// <summary>
/// 
/// </summary>
public class BuiltInPluginsOptions
{
    /// <summary>
    /// 
    /// </summary>
    public virtual BingSearchOptions BingSearch { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual GoogleSearchOptions GoogleSearch { get; set; }
}

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
    /// A valid plugin type is a class containing at least one method annotated with 
    /// <see cref="KernelFunctionAttribute"/>. Methods can optionally include a 
    /// <see cref="DescriptionAttribute"/>, providing the LLM with detailed guidance about the 
    /// function’s purpose, expected parameters, and usage.
    /// </para>
    /// <para>
    /// <b>Requirements and best practices:</b>
    /// <list type="bullet">
    /// <item>
    /// Register any dependencies required by a plugin in <see cref="IServiceCollection"/> 
    /// <b>before</b> registering this library. Plugin types must also be registered beforehand. 
    /// At runtime, plugin types are resolved to <see cref="Type"/> instances, and constructors 
    /// are invoked with dependencies from the kernel's service provider. If a type cannot be found 
    /// or dependencies cannot be resolved, an exception will be thrown.
    /// </item>
    /// <item>
    /// Define a single constructor per plugin or rely on the constructor with the fewest parameters 
    /// if multiple exist.
    /// </item>
    /// <item>
    /// Provide detailed method descriptions using <see cref="DescriptionAttribute"/>. Few-shot 
    /// examples, parameter guidance, and usage recommendations help the LLM understand and 
    /// call functions effectively.
    /// </item>
    /// <item>
    /// Use <c>snake_case</c> for function and property names, as most LLMs have been trained 
    /// with Python conventions for function calling.
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
    /// </summary>
    public virtual IEnumerable<string> Plugins { get; set; } = [];

    // BUG: new
    /// <summary>
    /// 
    /// </summary>
    public virtual BuiltInPluginsOptions BuiltInPlugins { get; set; } = new();


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