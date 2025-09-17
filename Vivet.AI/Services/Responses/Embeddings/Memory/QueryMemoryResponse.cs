using System.Collections.Generic;
using Vivet.AI.Services.Responses.Embeddings.Memory.Models;

namespace Vivet.AI.Services.Responses.Embeddings.Memory;

/// <summary>
/// Represents the response of a memory query operation.
/// </summary>
public class QueryMemoryResponse : BaseResponse
{
    /// <summary>
    /// The collection of query results for memory entries.
    /// </summary>
    public virtual IEnumerable<QueryMemoryResult> Results { get; set; } = new List<QueryMemoryResult>();
}