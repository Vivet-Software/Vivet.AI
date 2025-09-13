using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Metadata Options.
/// </summary>
public class MetadataOptions
{
    /// <summary>
    /// Chat model to use for getting blob metadata.
    /// The provide chat model must support the binary content, otherwise an error message is returned.
    /// </summary>
    [Required]
    public virtual ChatModel Model { get; set; } = new();

    /// <summary>
    /// The max word count for the metadata summary.
    /// The summary is vectorized and later used for searching blobs in the vector store.
    /// </summary>
    public virtual int SummaryMaxWords { get; set; } = 30;

    /// <summary>
    /// The max word count for the metadata description.
    /// The description is later passed to the chat model when the found by searching the vector store.
    /// </summary>
    public virtual int DescriptionMaxWords { get; set; } = 90;

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