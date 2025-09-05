using Vivet.AI.Services.Requests.Embedding.Knowledge.Models;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge;

/// <summary>
/// Represents a request to query knowledge entries with specific criteria, supporting pagination via limit and skip.
/// </summary>
public class QueryKnowledgeRequest : BaseQueryRequest<Data.Models.Knowledge, KnowledgeCriteria>;
