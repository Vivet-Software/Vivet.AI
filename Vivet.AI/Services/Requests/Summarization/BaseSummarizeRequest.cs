using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Summarization.Models;

namespace Vivet.AI.Services.Requests.Summarization;

/// <summary>
/// Represents the base request for a memory summarization operation.
/// </summary>
public abstract class BaseSummarizeRequest
{
    /// <summary>
    /// A collection of plugins to be added to the kernel for this request.
    /// <para>
    /// Each object in this collection must be a valid Semantic Kernel plugin. 
    /// A valid plugin is any object that contains at least one method annotated with 
    /// <see cref="KernelFunctionAttribute"/>. Methods can optionally include a 
    /// <see cref="DescriptionAttribute"/>, which provides the LLM with detailed guidance 
    /// about the function’s purpose, expected parameters, and usage.
    /// </para>
    /// <para>
    /// <b>Tips for plugin authors:</b>
    /// <list type="bullet">
    /// <item>
    /// Since most LLMs have been trained with Python for function calling,
    /// it is recommended to use <c>snake_case</c> for function names and property names.
    /// </item>
    /// <item>
    /// Provide detailed descriptions for your functions if the AI has trouble calling them. 
    /// Few-shot examples, recommendations for when to use (or not use) the function, and guidance 
    /// on where to get required parameters can all improve AI behavior.
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// The kernel will automatically reflect on these objects and expose all annotated methods 
    /// as callable functions within the chat context. Invalid objects (without any annotated methods) 
    /// will cause Semantic Kernel to throw an exception at runtime.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// Plugins = new List&lt;object&gt;
    /// {
    ///     new MyPlugin() // must have at least one [KernelFunction] method
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public virtual IEnumerable<object> Plugins { get; set; } = [];

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    [Required]
    public virtual SummarizationConfigOverrides ConfigOverrides { get; set; } = new();
}