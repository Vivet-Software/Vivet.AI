using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Config;

/// <summary>
/// Represents a custom plugin.
/// </summary>
public class CustomPluginOptions
{
    /// <summary>
    /// The name of the plugin.
    /// <para>
    /// If <c>null</c>, the type name will be used as the plugin name.
    /// Ensure that the plugin name is unique and does not conflict with any 
    /// enabled and configured built-in plugins: <b>Memory</b>, <b>Knowledge</b>, or <b>Web Search</b>.
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
    /// <para>
    /// <b>AppSettings.json example:</b>
    /// <code>
    /// {
    ///   "Plugins": [
    ///     {
    ///       "Name: "My Plugin)
    ///       "Type": "MyNamespace.MyPlugin, MyAssembly"
    ///     }
    ///     
    ///   ]
    /// }
    /// </code>
    /// </para>
    /// </summary>
    [Required]
    public virtual string Type { get; set; }
}