using System;

namespace Vivet.AI.Config;

/// <summary>
/// Agent Options
/// </summary>
public class AgentOptions
{
    /// <summary>
    /// Timeout before requests are aborted.
    /// Defailt to 60 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
}