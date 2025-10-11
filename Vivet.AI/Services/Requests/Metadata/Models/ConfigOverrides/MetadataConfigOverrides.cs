using Vivet.AI.Models;
using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Metadata.Models.ConfigOverrides;

/// <summary>
/// Represents configuration overrides specific to metadata operations.
/// </summary>
public class MetadataConfigOverrides : BaseConfigOverrides
{
    /// <summary>
    /// Gets or sets the name of the model to use for this request, overriding the default configured model.
    /// The specified model must be supported by the registered orchestration; otherwise, the request may fail.
    /// </summary>
    public virtual string ModelName { get; set; }
 
    /// <summary>
    /// Gets or sets the model parameters to use for the metadata retrieval.
    /// </summary>
    public virtual ChatModelParameters ModelParameters { get; set; }

    /// <summary>
    /// The max word count for the metadata summary.
    /// The summary is vectorized and later used for searching blobs in the vector store.
    /// </summary>
    public virtual int? SummaryMaxWords { get; set; }

    /// <summary>
    /// The max word count for the metadata description.
    /// The description is later passed to the chat model when the found by searching the vector store.
    /// </summary>
    public virtual int? DescriptionMaxWords { get; set; }
}