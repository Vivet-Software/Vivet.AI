using System.ComponentModel.DataAnnotations;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge;

/// <summary>
/// Represents a request to index textual knowledge with optional configuration overrides.
/// </summary>
public class IndexTextRequest : IndexTextRequest<string>;

/// <summary>
/// Represents a request to index textual knowledge with optional configuration overrides.
/// </summary>
public class IndexTextRequest<T> : BaseIndexKnowledgeRequst<KnowledgeIndexConfigOverrides>
    where T : class
{
    /// <summary>
    /// The text content to be indexed.
    /// Plain text, JSON and XML is supported.
    /// if  <inheritdoc cref="Text"/> is different than a string the type will be serialized as json
    /// and the resulting text will be indexed.
    /// </summary>
    [Required]
    public virtual T Text { get; set; }
}