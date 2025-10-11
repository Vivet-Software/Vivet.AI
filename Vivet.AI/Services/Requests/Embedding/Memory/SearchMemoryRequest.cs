using System;
using Vivet.AI.Config;
using Vivet.AI.Services.Requests.Embedding.Memory.Models;
using Vivet.AI.Services.Requests.Embedding.Memory.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Memory;

/// <summary>
/// Represents a request to search memory with specific criteria and optional thread context.
/// </summary>
public class SearchMemoryRequest : BaseSearchRequest<MemorySearchCriteria, MemorySearchConfigOverrides>
{
    /// <summary>
    /// The current thread (conversation).
    /// If this matches the <see cref="Data.Models.Memory.ThreadId"/> the score will be increased by
    /// the configured <see cref="MemoryScoringOptions.ThreadMatchBoost"/>.
    /// </summary>
    public virtual Guid? CurrentThreadId { get; set; }
}