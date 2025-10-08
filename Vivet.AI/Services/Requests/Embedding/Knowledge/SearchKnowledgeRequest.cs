using Vivet.AI.Services.Requests.Embedding.Knowledge.Models;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge;

/// <summary>
/// Represents a request to search knowledge entries with specific criteria.
/// </summary>
public class SearchKnowledgeRequest : BaseSearchRequest<KnowledgeQueryCriteria, KnowledgeSearchConfigOverrides>;