using System.Collections.Generic;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;

namespace Vivet.AI.Services.Responses.Embeddings.Knowledge;

/// <summary>
/// Represents the response of a knowledge query operation.
/// </summary>
public class QueryKnowledgeResponse : BaseResponse
{
    /// <summary>
    /// The collection of query results for knowledge entries.
    /// </summary>
    public virtual IEnumerable<QueryKnowledgeResult> Results { get; set; } = new List<QueryKnowledgeResult>();
}