using System;
using System.ComponentModel.DataAnnotations;
using Vivet.AI.Config.Enums;

namespace Vivet.AI.Config;

/// <summary>
/// Vector Database Options (nested class).
/// </summary>
public class VectorStoreOptions
{
    /// <summary>
    /// The vector database provider.
    /// Choose one of the supported providers in <see cref="VectorProvider"/>.
    /// </summary>
    [Required]
    public virtual VectorProvider Provider { get; set; } = VectorProvider.None;

    /// <summary>
    /// The host of your vector database.
    /// </summary>
    [Required]
    public virtual string Host { get; set; } = "localhost";

    /// <summary>
    /// The Port of your vector database.
    /// </summary>
    [Required]
    [Range(1, 65535)]
    public virtual int Port { get; set; } = 6334;

    /// <summary>
    /// The username for the vector store.
    /// Optional, as only some vector store providers require a username.
    /// </summary>
    public virtual string Username { get; set; }

    /// <summary>
    /// The api key of your vector database.
    /// This may be null if they is no authentication setup on your vector database.
    /// </summary>
    [Required]
    public virtual string ApiKey { get; set; }

    /// <summary>
    /// The query timout.
    /// </summary>
    [Required]
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Whether to enable health-check for the vector store.
    /// </summary>
    [Required]
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Whether the scheme of the <see cref="Host"/> is http or https.
    /// </summary>
    internal bool UseSsl => this.Host.StartsWith("https");
}