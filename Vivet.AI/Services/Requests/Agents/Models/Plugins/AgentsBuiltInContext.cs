using Vivet.AI.Services.Models.Plugins.Contexts;
using Vivet.AI.Services.Requests.Agents.Models.Plugins.Contexts;

namespace Vivet.AI.Services.Requests.Agents.Models.Plugins;

/// <summary>
/// Represents the context for built-in plugins.
/// Only plugins that are enabled and configured should have their context included in the request.
/// </summary>
public class AgentsBuiltInContext : BaseContext<AgentsMemoryContext, KnowledgeContext, WebSearchContext>;