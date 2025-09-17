using Vivet.AI.Services.Requests.Embedding.Knowledge.Models;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge;

/// <summary>
/// Represents a request to search knowledge entries with specific criteria.
/// </summary>
public class SearchKnowledgeRequest : BaseSearchRequest<Data.Models.Knowledge, KnowledgeCriteria>;
