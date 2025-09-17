using Vivet.AI.Services.Models.ConfigOverrides;

namespace Vivet.AI.Services.Requests.Embedding;

/// <summary>
/// Represents the base request for indexing operations with configurable overrides.
/// </summary>
/// <typeparam name="TOverrides">The type of configuration overrides. Must inherit from <see cref="BaseConfigOverrides"/> and have a parameterless constructor.</typeparam>
public abstract class BaseIndexRequest<TOverrides>
    where TOverrides : BaseConfigOverrides, new()
{
    /// <summary>
    /// Gets or sets the language associated with the request.
    /// </summary>
    public virtual string Language { get; set; }

    /// <summary>
    /// Gets or sets the configuration overrides for the request.
    /// </summary>
    public virtual TOverrides ConfigOverrides { get; set; } = new();
}