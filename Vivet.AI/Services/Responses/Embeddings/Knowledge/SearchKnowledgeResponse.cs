using System.Collections.Generic;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;

namespace Vivet.AI.Services.Responses.Embeddings.Knowledge;

/// <summary>
/// Represents the response of a knowledge search operation.
/// </summary>
public class SearchKnowledgeResponse : BaseResponse
{
    /// <summary>
    /// The collection of search results for knowledge entries.
    /// </summary>
    public virtual IEnumerable<SearchKnowledgeResult> Results { get; set; } = new List<SearchKnowledgeResult>();
}