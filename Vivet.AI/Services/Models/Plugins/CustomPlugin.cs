using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Services.Models.Plugins;

/// <summary>
/// Represents a custom plugin.
/// </summary>
public class CustomPlugin
{
    /// <summary>
    /// The name of the plugin.
    /// <para>
    /// If <c>null</c>, the type name will be used as the plugin name.
    /// Ensure that the plugin name is unique and does not conflict with any 
    /// enabled and configured built-in plugins: <b>memory</b>, <b>knowledge</b>, or <b>web_search</b>.
    /// </para>
    /// <para>
    /// Also, a plugin name can contain only ASCII letters, digits, and underscores
    /// </para>
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// The custom plugin type to be added to the kernel from configuration.
    /// <para>
    /// The type must represent a valid Semantic Kernel plugin type. 
    /// A valid plugin type is a class containing at least one method annotated with 
    /// <see cref="KernelFunctionAttribute"/>. Methods can optionally include a 
    /// <see cref="DescriptionAttribute"/>, providing the LLM with detailed guidance about the 
    /// function’s purpose, expected parameters, and usage.
    /// </para>
    /// <para>
    /// <b>Requirements and best practices:</b>
    /// <list type="bullet">
    /// <item>
    /// Register any dependencies required by a plugin in <see cref="IServiceCollection"/>.
    /// At runtime, plugin types are resolved to <see cref="System.Type"/> instances, and constructors 
    /// are invoked with dependencies from the application service provider. If a type cannot be found 
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
    /// </summary>
    [Required]
    public virtual Type Type { get; set; }

    /// <summary>
    /// Defines the context required for a custom plugin, represented as a 
    /// collection of name–value pairs.
    /// <para>
    /// Values may be complex objects, but it is recommended to keep them simple 
    /// to minimize the risk of misinterpretation by the LLM.
    /// </para>
    /// <para>
    /// Parameter names should use <c>snake_case</c>, since most LLMs are trained 
    /// on Python-style function signatures and handle this convention more reliably.
    /// </para>
    /// </summary>
    [Required]
    public virtual IDictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
}