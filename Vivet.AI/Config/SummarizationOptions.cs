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
}