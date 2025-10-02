using System;

namespace Vivet.AI.Services.Collectors.Models;

/// <summary>
/// The result of a function call.
/// </summary>
public class FunctionCallResult
{
    /// <summary>
    /// The type that the result.
    /// </summary>
    public virtual Type Type { get; set; } 

    /// <summary>
    /// The result of the function call.
    /// </summary>
    public virtual object Result { get; set; }
}