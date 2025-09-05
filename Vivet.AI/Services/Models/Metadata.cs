namespace Vivet.AI.Services.Models;

/// <summary>
/// Meta data.
/// </summary>
public class Metadata
{
    /// <summary>
    /// The contents that will be vectorized.
    /// </summary>
    public virtual string Summary { get; set; }

    /// <summary>
    /// The text that will be sent to the chat when summary is matched.
    /// </summary>
    public virtual string Description { get; set; }
}