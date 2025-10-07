using Vivet.AI.Services.Requests.Embedding.Memory.Models;

namespace Vivet.AI.Services.Requests.Embedding.Memory;

/// <summary>
/// Represents a request to query memory with specific criteria, supporting pagination via limit and skip.
/// </summary>
public class QueryMemoryRequest : BaseQueryRequest<Data.Models.Memory, MemoryQueryCriteria>;