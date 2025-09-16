using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.SemanticKernel;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Requests.Chat.Models;

namespace Vivet.AI.Services.Requests.Chat;

/// <summary>
/// Represents a chat request, including system message, user question,
/// conversation context, and optional blob data for reference.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// A system message that provides context or instructions to the chat model.
    /// </summary>
    public virtual string SystemMessage { get; set; }

    /// <summary>
    /// The user's question to be answered in the chat.
    /// </summary>
    [Required]
    public virtual string Question { get; set; }

    /// <summary>
    /// Identifier of the tenant associated with this request.
    /// Used when looking up relevant knowledge entries.
    /// </summary>
    public virtual string TenantId { get; set; }

    /// <summary>
    /// Identifier of the sub-tenant associated with this request.
    /// Used when looking up relevant knowledge entries.
    /// </summary>
    public virtual string SubTenantId { get; set; }

    /// <summary>
    /// Scope identifier for the request.
    /// Used when looking up knowledge or memories entries.
    /// </summary>
    public virtual string ScopeId { get; set; }

    /// <summary>
    /// Identifier of the agent processing the request.
    /// Used for memory retrieval and context matching.
    /// </summary>
    public virtual string AgentId { get; set; }

    /// <summary>
    /// Identifier of the user making the request.
    /// Used for personalizing memory lookups.
    /// </summary>
    [Required]
    public virtual string UserId { get; set; }

    /// <summary>
    /// Identifier of the current conversation thread.
    /// Used to boost the relevance of memory entries in the same thread.
    /// </summary>
    [Required]
    public virtual string CurrentThreadId { get; set; }

    /// <summary>
    /// Collection of optional blobs associated with the request.
    /// These may provide additional context for answering the question.
    /// </summary>
    [Required]
    public virtual IEnumerable<BaseBlobMetadata> Blobs { get; set; } = [];

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
    public virtual ChatConfigOverrides ConfigOverrides { get; set; } = new();
}