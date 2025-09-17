using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AI.Services.Requests.Chat;
using Vivet.AI.Services.Responses.Chat;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Service for handling chat interactions, including retrieving relevant memory and knowledge,
/// executing prompts, and generating chat responses.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Sends a chat request and returns a <see cref="ChatResponse"/> containing the answer, reasoning, and additional metadata.
    /// </summary>
    /// <param name="request">The chat request containing the user's question, system message, and other context.</param>
    /// <param name="onMemoryIndexed">
    /// Optional callback that is invoked after the response has been saved to memory.
    /// This callback is awaited, meaning the <see cref="Task"/> returned by this method will not
    /// complete until the callback itself has finished. If you need the callback work to run
    /// in the background without delaying the chat result, you must offload that work explicitly
    /// (e.g. <c>Task.Run</c> or a background queue).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the <see cref="ChatResponse"/>.</returns>
    Task<ChatResponse> ChatAsync(ChatRequest request, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a chat request and returns a typed <see cref="ChatResponse{T}"/> containing
    /// the answer, reasoning, and related metadata.
    /// </summary>
    /// <typeparam name="T">The type to cast the chat response answer to. Can be <see cref="string"/> for plain text or a user-defined type when the response is structured JSON.
    /// </typeparam>
    /// <param name="request">The chat request containing the user's question, system message, and other context.</param>
    /// <param name="onMemoryIndexed">
    /// Optional callback that is invoked after the response has been saved to memory.
    /// This callback is awaited, meaning the <see cref="Task"/> returned by this method will not
    /// complete until the callback itself has finished. If you need the callback work to run
    /// in the background without delaying the chat result, you must offload that work explicitly
    /// (e.g. <c>Task.Run</c> or a background queue).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the typed<see cref="ChatResponse{T}"/>.</returns>
    Task<ChatResponse<T>> ChatAsync<T>(ChatRequest request, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Streams chat responses asynchronously, yielding partial results as they are received.
    /// </summary>
    /// <param name="request">The chat request containing the user's question, system message, and other context.</param>
    /// <param name="onMemoryIndexed">
    /// Optional callback that is invoked after the response has been saved to memory.
    /// This callback is awaited, meaning the <see cref="Task"/> returned by this method will not
    /// complete until the callback itself has finished. If you need the callback work to run
    /// in the background without delaying the chat result, you must offload that work explicitly
    /// (e.g. <c>Task.Run</c> or a background queue).
    /// </param>
    /// <param name="onChatStreamingComplete">Optional callback invoked when streaming is complete with the full <see cref="ChatResponse"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of partial response strings.</returns>
    IAsyncEnumerable<string> ChatStreamingAsync(ChatRequest request, Func<ChatIndexMemoryResponse, Task> onMemoryIndexed = null, Func<ChatResponse, Task> onChatStreamingComplete = null, CancellationToken cancellationToken = default);
}