using System;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Services.Collectors.Models;

internal class ResponseCallback
{
    internal virtual ChatMessageContent ChatMessageContent { get; set; }

    internal virtual TimeSpan ElapsedTime { get; set; }

    /// <summary>
    /// An exception if one ocurred during the function invocation.
    /// </summary>
    public virtual Exception Exception { get; set; }
}