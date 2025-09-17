using System;
using Vivet.AI.Services.Responses.Embeddings.Memory;

namespace Vivet.AI.Services.Responses.Chat;

/// <summary>
/// Represents a index memory response returned after a chat request when memory is saved asynchronously.
/// </summary>
public class ChatIndexMemoryResponse
{
    /// <summary>
    /// The result of the memory indexing.
    /// <b>null</b> on task failed.
    /// </summary>
    public virtual IndexMemoryResponse Result { get; set; }

    /// <summary>
    /// The exception thrown in case the memory indexing task fails.
    /// </summary>
    public virtual Exception Exception { get; set; }
}