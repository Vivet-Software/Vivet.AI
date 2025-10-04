using System;
using System.Collections.Generic;

namespace Vivet.AI.Services.Models;

/// <summary>
/// A function call.
/// </summary>
public class FunctionCall
{
    /// <summary>
    /// The unique identifier of the function call.
    /// </summary>
    public virtual string Id { get; set; }

    /// <summary>
    /// The name of the plugin that the function belongs to.
    /// </summary>
    public virtual string PluginName { get; set; }

    /// <summary>
    /// The name of the function.
    /// </summary>
    public virtual string FunctionName { get; set; }

    /// <summary>
    /// The result of the functiona call.
    /// </summary>
    public virtual FunctionCallResult Result { get; set; }

    /// <summary>
    /// The prompt rendered and injected as result of the function call.
    /// </summary>
    public virtual string RenderedPrompt { get; set; }

    /// <summary>
    /// The arguments that was passed to the function.
    /// </summary>
    public virtual IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// An exception if one ocurred during the function invocation.
    /// </summary>
    public virtual Exception Exception { get; set; }

    /// <summary>
    /// When the function call was created.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}