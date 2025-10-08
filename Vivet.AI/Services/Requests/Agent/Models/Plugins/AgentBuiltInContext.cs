using Vivet.AI.Services.Models.Plugins;
using Vivet.AI.Services.Models.Plugins.Contexts;
using Vivet.AI.Services.Requests.Agent.Models.Plugins.Context;

namespace Vivet.AI.Services.Requests.Agent.Models.Plugins;

/// <summary>
/// Represents the context for built-in plugins.
/// Only plugins that are enabled and configured should have their context included in the request.
/// </summary>
public class AgentBuiltInContext : BaseBuiltInContext<AgentsMemoryContext, KnowledgeContext, WebSearchContext>;