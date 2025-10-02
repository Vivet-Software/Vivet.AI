using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Plugins.Consts;
using Vivet.AI.Services.Requests.Agent.Models.Plugins;
using Vivet.AI.Services.Requests.Chat.Models.Plugins;
using Vivet.AI.Services.Responses;
using Vivet.AI.Services.Responses.Metadata;

namespace Vivet.AI.Services.Extensions;

internal static class ChatHistoryExtensions
{
    internal static ChatHistory AddChatSystemPrompt<T>(this ChatHistory chatHistory, string additionalSystemMessage = null)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        var stringBuilder = new StringBuilder();

        if (additionalSystemMessage != null)
        {
            stringBuilder
                .AppendLine(additionalSystemMessage);
        }

        stringBuilder
            .AppendLine(@$"You always respond in strict JSON format.
The JSON response must contain:
{{
  ""Reasoning"": ""Internal reasoning, thinking or planning"",
  ""Answer"": ""Final user-facing answer"",
  ""Language"": ""The language of the prompt in ISO 639-1""
}}

Rules:
- Do not include code fences (```json).
- Do not add extra commentary or text outside of the JSON.
- Inline JSON inside the ""Answer"" must be properly escaped.
- if you are unable complete the request, add a property called {nameof(BaseResponse.ErrorMessage)}, 
containing a meaningful error message, describing why the request could not be completed.");

        if (typeof(T) != typeof(string))
        {
            var schema = typeof(T).GenerateJsonMap();
            var serializedSchema = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });

            stringBuilder
                .AppendLine($"Please respond using the following JSON schema: {serializedSchema}");
        }

        var content = stringBuilder
            .ToString();

        chatHistory
            .AddSystemMessage(content);

        return chatHistory;
    }

    internal static ChatHistory AddChatPluginsContextPrompt(this ChatHistory chatHistory, ChatPlugins plugins)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (plugins == null)
            throw new ArgumentNullException(nameof(plugins));

        var stringBuilder = new StringBuilder();

        stringBuilder
            .AppendBuiltInPluginContext(plugins.Context.Memory, BuiltInPluginNames.MEMORY_PLUGIN)
            .AppendBuiltInPluginContext(plugins.Context.Knowledge, BuiltInPluginNames.KNOWLEDGE_PLUGIN)
            .AppendBuiltInPluginContext(plugins.Context.WebSearch, BuiltInPluginNames.WEB_SEARCH_PLUGIN)
            .AppendCustomPluginsContext(plugins.CustomPlugins);

        var content = stringBuilder
            .ToString();

        if (!string.IsNullOrEmpty(content))
        {
            content = @$"[PLUGIN CONTEXT]
{content}";

            chatHistory
                .AddSystemMessage(content);
        }

        return chatHistory;
    }

    internal static ChatHistory AddAgentPluginsContextPrompt(this ChatHistory chatHistory, AgentPlugins plugins, AgentPlugins parentPlugins)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));
        
        if (plugins == null) 
            throw new ArgumentNullException(nameof(plugins));
        
        if (parentPlugins == null) 
            throw new ArgumentNullException(nameof(parentPlugins));

        var stringBuilder = new StringBuilder();

        var contextMemory = plugins.Context.Memory ?? parentPlugins.Context?.Memory;
        var contextKnowledge = plugins.Context.Knowledge ?? parentPlugins.Context?.Knowledge;
        var contextWebSearch = plugins.Context.WebSearch ?? parentPlugins.Context?.WebSearch;

        stringBuilder
            .AppendBuiltInPluginContext(contextMemory, BuiltInPluginNames.MEMORY_PLUGIN)
            .AppendBuiltInPluginContext(contextKnowledge, BuiltInPluginNames.KNOWLEDGE_PLUGIN)
            .AppendBuiltInPluginContext(contextWebSearch, BuiltInPluginNames.WEB_SEARCH_PLUGIN);

        var customPlugins = plugins.CustomPlugins
            .Concat(parentPlugins.CustomPlugins)
            .DistinctBy(x => x.Name);

        stringBuilder
            .AppendCustomPluginsContext(customPlugins);

        var content = stringBuilder
            .ToString();

        if (!string.IsNullOrEmpty(content))
        {
            content = @$"[PLUGIN CONTEXT]
{content}";

            chatHistory
                .AddSystemMessage(content);
        }

        return chatHistory;
    }

    internal static ChatHistory AddChatUserPrompt(this ChatHistory chatHistory, string question, IEnumerable<KernelContent> blobContents)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (question == null) 
            throw new ArgumentNullException(nameof(question));

        if (blobContents == null)
            throw new ArgumentNullException(nameof(blobContents));

        var textContent = new TextContent(@$"[QUESTION]
{question}");

        var messageContentItemCollection = new ChatMessageContentItemCollection
        {
            textContent
        };

        foreach (var binaryContent in blobContents)
        {
            messageContentItemCollection
                .Add(binaryContent);
        }

        chatHistory
            .AddUserMessage(messageContentItemCollection);

        return chatHistory;
    }

    internal static ChatHistory AddMetadataPrompt<T>(this ChatHistory chatHistory, KernelContent blobContent, int summaryMaxWords, int descriptionMaxWords)
        where T : class, new()
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (blobContent == null)
            throw new ArgumentNullException(nameof(blobContent));

        var stringBuilder = new StringBuilder();

        stringBuilder
            .AppendLine(@$"You are a metadata extraction assistant.
You always respond in strict JSON format.
Return a JSON object with a property called {nameof(MetadataResponse<T>.Metadata)}, containing these nested properties:
{nameof(Metadata.Summary)} (max {summaryMaxWords} words), {nameof(Metadata.Description)} (max {descriptionMaxWords} words).");

        if (typeof(T) != typeof(object))
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.Name)
                .ToArray();

            if (properties.Any())
            {
                var additionalProperties = string.Join(", ", properties);

                chatHistory
                    .AddUserMessage(
                        $"Also add a property called {nameof(MetadataResponse<T>.AdditionalMetadata)} containing the following additional " +
                        $"properties in the JSON output: {additionalProperties}.");
            }

            var propertiesDictionary = properties
                .ToDictionary(x => x, object (_) => null);

            var metadataResponseTemplate = new Dictionary<string, object>
            {
                [nameof(MetadataResponse.Metadata)] = new Metadata(),
                [nameof(MetadataResponse<T>.AdditionalMetadata)] = propertiesDictionary
            };

            var serializedTemplate = JsonSerializer.Serialize(metadataResponseTemplate, new JsonSerializerOptions { WriteIndented = true });

            chatHistory
                .AddSystemMessage(serializedTemplate);
        }

        stringBuilder
            .AppendLine($@"Rules:
-Do not include code fences(```json).
-Do not add extra commentary or text outside of the JSON.
-If you are unable to read or understand the binary content, add a property called {nameof(BaseResponse.ErrorMessage)},
containing a meaningful error message describing why the metadata retrieval could not be completed.");

        chatHistory
            .AddSystemMessage(stringBuilder.ToString());

        var textContent = new TextContent("Analyze the binary content provided and respond with extracted metadata");

        var messageContentItemCollection = new ChatMessageContentItemCollection
        {
            textContent,
            blobContent
        };

        chatHistory
            .AddUserMessage(messageContentItemCollection);

        return chatHistory;
    }
    
    internal static ChatHistory AddSummarizationMemoryPrompt(this ChatHistory chatHistory, string question, string answer, int summarizationDegree)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (question == null)
            throw new ArgumentNullException(nameof(question));

        if (answer == null)
            throw new ArgumentNullException(nameof(answer));

        chatHistory
            .AddSystemMessage(@$"You are a text summarization assistant.
Summarization Level: {summarizationDegree}.

Use the scale to control how much detail is preserved.
- 0 → No summarization (return full question and answer).
- 25 → Remove fluff, retain full detail.
- 50 → Preserve core meaning, make concise.
- 75 → Keep only essential ideas, drop minor points.
- 100 → Only the most important concepts, heavily compressed.
Treat the Summarization Level as the percentage by which the original text should be shortened. 
For example, a level of 40 means the summarized output should be about 60% of the original length (a 40% reduction)

Return the output in the following JSON format:
{{
  ""QuestionSummarized"": ""summarized question here"",
  ""AnswerSummarized"": ""summarized answer here""
}}

Rules:
- Do not include code fences (```json).
- Do not add extra commentary or text outside of the JSON.
- Inline JSON inside the """"QuestionSummarized"""" or """"AnswerSummarized"""" must be properly escaped.
- if you are unable complete the request, add a property called {nameof(BaseResponse.ErrorMessage)}, 
containing a meaningful error message, describing why the request could not be completed."");

IMPORTANT: DO NOT change, summarize, or remove any JSON or XML in the Question or Answer.
- JSON is any text between `{{` and `}}`.
- XML is any text between `<` and `>`.
- Copy JSON/XML exactly as it appears.
- Only summarize the natural language outside these snippets.");

        chatHistory
            .AddUserMessage(@$"Summarize the following question-and-answer pair using the summarization level provided.
[Q&A]
Q: {question}
A: {answer}
");

        return chatHistory;
    }
    
    internal static string GetPromptAsText(this ChatHistory chatHistory, bool outputBinary = false)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        var stringBuilder = new StringBuilder();

        foreach (var message in chatHistory)
        {
            stringBuilder
                .AppendLine($"{message.Role}:");

            foreach (var item in message.Items)
            {
                var value = item switch
                {
                    TextContent text => text.Text,
                    AudioContent audioContent => outputBinary
                        ? $"[{audioContent.DataUri}]"
                        : $"[{audioContent.GetType().Name}]",
                    ImageContent imageContent => outputBinary
                        ? $"[{imageContent.DataUri}]"
                        : $"[{imageContent.GetType().Name}]",
                    BinaryContent binaryContent => outputBinary
                        ? $"[{binaryContent.DataUri}]"
                        : $"[{binaryContent.GetType().Name}]",
                    _ => $"[{item.GetType().Name}]"
                };

                stringBuilder
                    .AppendLine(value);
            }
        }

        return stringBuilder
            .ToString();
    }
}