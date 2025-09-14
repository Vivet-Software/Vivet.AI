using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Models;

namespace Vivet.AI.Config;

/// <summary>
/// Metadata Options.
/// </summary>
public class MetadataOptions
{
    /// <summary>
    /// Chat model to use for getting blob metadata.
    /// The provide chat model must support the binary content, otherwise an error message is returned.
    /// </summary>
    [Required]
    public virtual ChatModel Model { get; set; } = new();

    /// <summary>
    /// The max word count for the metadata summary.
    /// The summary is vectorized and later used for searching blobs in the vector store.
    /// </summary>
    public virtual int SummaryMaxWords { get; set; } = 30;

    /// <summary>
    /// The max word count for the metadata description.
    /// The description is later passed to the chat model when the found by searching the vector store.
    /// </summary>
    public virtual int DescriptionMaxWords { get; set; } = 90;

    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// A collection of plugins to be added to the kernel from configuration.
    /// </summary>
    [Required]
    public virtual MetadataPluginsOptions Plugins { get; set; } = new();
}