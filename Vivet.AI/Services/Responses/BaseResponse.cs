using System;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Responses;

/// <summary>
/// Represents the base response type for service operations,
/// providing common properties such as execution time,
/// token usage, and error information.
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// The total time elapsed while processing the request.
    /// </summary>
    public virtual TimeSpan ElapsedTime { get; set; }

    /// <summary>
    /// Information about token usage for the request, including input and output token counts.
    /// Not supported for streaming responses.
    /// </summary>
    public virtual TokenUsage TokenUsage { get; set; }

    /// <summary>
    /// An exception describing the failure, if one occurred.
    /// </summary>
    public virtual Exception Exception { get; set; }

    /// <summary>
    /// An error message describing the failure, if one occurred.
    /// Intended for internal use only.
    /// </summary>
    internal virtual string ErrorMessage { get; set; }
}