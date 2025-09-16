using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Config;

/// <summary>
/// Plugin options for chat model.
/// </summary>
public class ChatPluginsOptions
{
    /// <summary>
    /// Built-in plugins that can be enabled for the chat model.
    /// </summary>
    [Required]
    public virtual ChatBuiltInPluginsOptions BuiltInPlugins { get; set; } = new();

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
    ///   "Plugins": [
    ///     "MyNamespace.MyPlugin, MyAssembly"
    ///   ]
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public virtual IEnumerable<string> CustomPlugins { get; set; } = [];
}