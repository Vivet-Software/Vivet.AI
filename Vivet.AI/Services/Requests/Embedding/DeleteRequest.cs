using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vivet.AI.Services.Requests.Embedding;

/// <summary>
/// Represents a request to delete one or more items by their unique identifiers.
/// </summary>
public class DeleteRequest
{
    /// <summary>
    /// Gets or sets the collection of unique identifiers (GUIDs) of the items to delete.
    /// </summary>
    [Required]
    public virtual IEnumerable<Guid> Ids { get; set; } = [];
}