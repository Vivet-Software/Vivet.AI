using System.Collections.Generic;
using Vivet.AI.Services.Responses.Embeddings.Memory.Models;

namespace Vivet.AI.Services.Responses.Embeddings.Memory;

/// <summary>
/// Represents the response of a memory search operation.
/// </summary>
public class SearchMemoryResponse : BaseResponse
{
    /// <summary>
    /// The collection of search results for memory entries.
    /// </summary>
    public virtual IEnumerable<SearchMemoryResult> Results { get; set; } = new List<SearchMemoryResult>();
}