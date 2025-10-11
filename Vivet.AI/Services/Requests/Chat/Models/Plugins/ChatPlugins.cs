using Vivet.AI.Services.Models.Plugins;

namespace Vivet.AI.Services.Requests.Chat.Models.Plugins;

/// <summary>
/// Represents plugins and their associated context for both built-in and custom plugins.
/// </summary>
public class ChatPlugins : BasePlugins<ChatBuiltInContext>;