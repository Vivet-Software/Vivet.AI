using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Models.MimeTypes;
using Vivet.AI.Services.Serialization;

namespace Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;

/// <summary>
/// Represents the result of a knowledge entry, including metadata, blob, and tags.
/// </summary>
public class KnowledgeResult : BaseResult
{
    /// <summary>
    /// The tenant identifier associated with the knowledge entry.
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// The sub-tenant identifier associated with the knowledge entry.
    /// </summary>
    public string SubTenantId { get; set; }

    /// <summary>
    /// The scope identifier of the knowledge entry.
    /// </summary>
    public string ScopeId { get; set; }

    /// <summary>
    /// The user identifier associated with the knowledge entry.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// The source of the knowledge entry.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// The creator of the knowledge entry.
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// The tags associated with the knowledge entry.
    /// </summary>
    public string[] Tags { get; set; }

    /// <summary>
    /// The blob associated with the knowledge entry.
    /// </summary>
    public BlobResponse Blob { get; set; }

    /// <summary>
    /// The deserialized blob metadata.
    /// </summary>
    public JObject BlobMetadata { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public KnowledgeResult()
    {
    }

    /// <summary>
    /// Constructs a <see cref="KnowledgeResult"/> from a <see cref="Data.Models.Knowledge"/> instance.
    /// </summary>
    /// <param name="knowledge">The knowledge model to map from.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="knowledge"/> is null.</exception>
    public KnowledgeResult(Data.Models.Knowledge knowledge)
        : base(knowledge)
    {
        if (knowledge == null)
            throw new ArgumentNullException(nameof(knowledge));

        this.TenantId = knowledge.TenantId;
        this.SubTenantId = knowledge.SubTenantId;
        this.ScopeId = knowledge.ScopeId;
        this.UserId = knowledge.UserId;
        this.Source = knowledge.Source;
        this.CreatedBy = knowledge.CreatedBy;
        this.Tags = knowledge.Tags;
        this.Blob = knowledge.BlobBase64 == null
            ? null
            : new BlobResponse
            {
                MimeType = MimeType.FromValue(knowledge.BlobMimeType),
                Base64 = knowledge.BlobBase64,
                Hash = knowledge.ContentHash
            };

        this.BlobMetadata = knowledge.BlobMetadata == null 
            ? null
            : JsonConvert.DeserializeObject<JObject>(knowledge.BlobMetadata, Settings.SerializerSettings);
    }
}