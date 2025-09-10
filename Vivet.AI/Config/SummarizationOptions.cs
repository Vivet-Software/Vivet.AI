using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Summarization Options.
/// </summary>
public class SummarizationOptions
{
    /// <summary>
    /// The chat model to use for summarization.
    /// </summary>
    [Required]
    public virtual ChatModel Model { get; set; } = new();

    /// <summary>
    /// The degree of summarization (0 - 100).
    /// Higher values means higher compression and less precision.
    /// 0: No summarization.
    /// 25: Preserve nearly all details, only remove fluff.,
    /// 50: Keep core meaning but make it more concise.,
    /// 75: Summarize concisely and remove non-essential details.,
    /// 100: Compress the content to its most essential ideas only.
    /// </summary>
    [Required]
    [Range(0, 100)]
    public virtual int SummarizationDegree { get; set; } = 25;

    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

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
}