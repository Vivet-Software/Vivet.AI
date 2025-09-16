using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Chat;
using Vivet.AI.Services.Responses;
using Vivet.AI.Services.Responses.Metadata;

namespace Vivet.AI.Services.Extensions;

internal static class ChatHistoryExtensions
{
    internal static ChatHistory AddChatSystemPrompt<T>(this ChatHistory chatHistory, string additionalSystemMessage = null)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        const string BASE_PROMPT = @$"
You are an assistant that always responds in strict JSON format.
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
containing a meaningful error message, describing why the request could not be completed."";
";

        chatHistory
            .AddSystemMessage(BASE_PROMPT);

        if (additionalSystemMessage != null)
        {
            chatHistory
                .AddSystemMessage(additionalSystemMessage);
        }

        if (typeof(T) != typeof(string))
        {
            var schema = typeof(T).GenerateJsonMap();
            var serializedSchema = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });

            chatHistory
                .AddUserMessage($"Please respond using the following JSON schema: {serializedSchema}");
        }

        return chatHistory;
    }
    internal static ChatHistory AddChatPluginContextPrompt(this ChatHistory chatHistory, ChatRequest request)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        chatHistory
            .AddSystemMessage(@$"Context: 
UserId={request.UserId}, 
ScopeId={request.ScopeId}, 
AgentId={request.AgentId}, 
ThreadId={request.CurrentThreadId}, 
TenantId={request.TenantId}, 
SubTenantId={request.SubTenantId}");

        return chatHistory;
    }
    // BUG: Remove
    //internal static ChatHistory AddChatMemoryPrompt(this ChatHistory chatHistory, MemoryResult[] memoryResults, int counterpartContextQueryLimit)
    //{
    //    if (chatHistory == null)
    //        throw new ArgumentNullException(nameof(chatHistory));

    //    if (memoryResults == null)
    //        throw new ArgumentNullException(nameof(memoryResults));

    //    if (memoryResults.Any())
    //    {
    //        chatHistory
    //            .AddSystemMessage("[MEMORY]");
    //    }

    //    foreach (var memoryResult in memoryResults)
    //    {
    //        if (memoryResult.IsQuestion)
    //        {
    //            chatHistory
    //                .AddUserMessage($"Q: {memoryResult.FullContext}");

    //            if (memoryResult.Blob != null)
    //            {
    //                var dataUri = memoryResult.Blob
    //                    .GetDataUri();

    //                chatHistory
    //                    .AddUserMessage([new BinaryContent(dataUri)]);
    //            }

    //            var counterpartContexts = memoryResult.CounterpartContext
    //                .Take(counterpartContextQueryLimit);
                
    //            foreach (var counterPartContext in counterpartContexts)
    //            {
    //                chatHistory
    //                    .AddAssistantMessage($"A: {counterPartContext}");
    //            }

    //            chatHistory
    //                .AddSystemMessage($"(Date: {memoryResult.CreatedAt:D})");
    //        }
    //        else if (memoryResult.IsAnswer)
    //        {
    //            var counterpartContexts = memoryResult.CounterpartContext
    //                .Take(counterpartContextQueryLimit);
                
    //            foreach (var counterpartContext in counterpartContexts)
    //            {
    //                chatHistory
    //                    .AddUserMessage($"Q: {counterpartContext}");
    //            }

    //            chatHistory
    //                .AddAssistantMessage($"A: {memoryResult.FullContext}");

    //            if (memoryResult.Blob != null)
    //            {
    //                var dataUri = memoryResult.Blob
    //                    .GetDataUri();

    //                chatHistory
    //                    .AddUserMessage([new BinaryContent(dataUri)]);
    //            }

    //            chatHistory
    //                .AddSystemMessage($"(Date: {memoryResult.CreatedAt:D})");
    //        }
    //    }

    //    return chatHistory;
    //}
    //internal static ChatHistory AddChatKnowledgePrompt(this ChatHistory chatHistory, KnowledgeResult[] knowledgeResults)
    //{
    //    if (chatHistory == null) 
    //        throw new ArgumentNullException(nameof(chatHistory));
        
    //    if (knowledgeResults == null)
    //        throw new ArgumentNullException(nameof(knowledgeResults));

    //    if (knowledgeResults.Any())
    //    {
    //        chatHistory
    //            .AddSystemMessage("[KNOWLEDGE]");
    //    }

    //    foreach (var knowledgeResult in knowledgeResults)
    //    {
    //        if (knowledgeResult.Source != null)
    //        {
    //            chatHistory
    //                .AddSystemMessage($"{knowledgeResult.Source}");
    //        }

    //        if (knowledgeResult.Blob == null)
    //        {
    //            chatHistory
    //                .AddAssistantMessage(knowledgeResult.FullContext);
    //        }
    //        else
    //        {
    //            chatHistory
    //                .AddAssistantMessage(knowledgeResult.FullContext);

    //            var dataUri = knowledgeResult.Blob
    //                .GetDataUri();

    //            chatHistory
    //                .AddUserMessage([new BinaryContent(dataUri)]);

    //            if (knowledgeResult.BlobMetadata != null)
    //            {
    //                var metadataProperties = knowledgeResult.BlobMetadata
    //                    .GetType()
    //                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                    .Select(x => new
    //                    {
    //                        Key = x.Name,
    //                        Value = x.GetValue(knowledgeResult.BlobMetadata)
    //                    })
    //                    .Select(x => $"{x.Key}={x.Value ?? "N/A"}");

    //                var metadataContent = string.Join(", ", metadataProperties);

    //                chatHistory
    //                    .AddAssistantMessage(metadataContent);
    //            }
    //        }

    //        chatHistory
    //            .AddSystemMessage($"{knowledgeResult.CreatedAt:D}");
    //    }

    //    return chatHistory;
    //}
    internal static ChatHistory AddChatUserPrompt(this ChatHistory chatHistory, string question, IEnumerable<string> dataUris)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (question == null) 
            throw new ArgumentNullException(nameof(question));

        if (dataUris == null)
            throw new ArgumentNullException(nameof(dataUris));

        chatHistory
            .AddUserMessage($"question: {question}");

        var messageContentItemCollection = new ChatMessageContentItemCollection();

        var binaryContents = dataUris
            .Select(x => new BinaryContent(x));

        foreach (var binaryContent in binaryContents)
        {
            messageContentItemCollection
                .Add(binaryContent);
        }

        if (messageContentItemCollection.Any())
        {
            chatHistory
                .AddUserMessage(messageContentItemCollection);
        }

        return chatHistory;
    }
    internal static ChatHistory AddMetadataPrompt<T>(this ChatHistory chatHistory, string dataUri, int summaryMaxWords, int descriptionMaxWords)
        where T : class, new()
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (dataUri == null)
            throw new ArgumentNullException(nameof(dataUri));

        chatHistory
            .AddSystemMessage("You are a metadata extraction assistant.");

        chatHistory
            .AddUserMessage(
                "Analyze the binary content provided and respond strictly in JSON format with extracted metadata. " +
                "Don't include ```json or any other code fences, and don't add explanations or extra text. " +
                $"Return a JSON object with a property called {nameof(MetadataResponse<T>.Metadata)}, containing these nested properties: " +
                $"{nameof(Metadata.Summary)} (max {summaryMaxWords} words), {nameof(Metadata.Description)} (max {descriptionMaxWords} words).");

        if (typeof(T) != typeof(object))
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name)
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

        chatHistory
            .AddUserMessage([new BinaryContent(dataUri)]);

        chatHistory
            .AddUserMessage(
                $"If you are unable to read or understand the binary content, add a property called {nameof(BaseResponse.ErrorMessage)}, " +
                "containing a meaningful error message describing why the metadata retrieval could not be completed.");

        return chatHistory;
    }
    internal static ChatHistory AddSuummarizationMemoryPrompt(this ChatHistory chatHistory, string question, string answer, int summarizationDegree)
    {
        if (chatHistory == null)
            throw new ArgumentNullException(nameof(chatHistory));

        if (question == null)
            throw new ArgumentNullException(nameof(question));

        if (answer == null)
            throw new ArgumentNullException(nameof(answer));

        chatHistory
            .AddSystemMessage("You are a text summarization assistant.");

        chatHistory
            .AddUserMessage($@"
Summarize the following question-and-answer pair using the summarization level provided. 
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
Any inline JSON in QuestionSummarized or AnswerSummarized must be properly escaped.
Don't include ```json or any other code fences, and don't add explanations or extra text.

If you are unable to complete the request, add a property called {nameof(BaseResponse.ErrorMessage)}, containing a meaningful error message describing 
why the summarization could not be completed.

IMPORTANT: DO NOT change, summarize, or remove any JSON or XML in the Question or Answer.
- JSON is any text between `{{` and `}}`.
- XML is any text between `<` and `>`.
- Copy JSON/XML exactly as it appears.
- Only summarize the natural language outside these snippets.

[Q&A]
Q: {question}
A: {answer}
");

        return chatHistory;
    }
    internal static string GetPromptAsText(this ChatHistory chatHistory)
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
                switch (item)
                {
                    case TextContent text:
                        stringBuilder
                            .AppendLine(text.Text);
                        break;

                    case AudioContent:
                    case ImageContent:
                    case BinaryContent:
                        stringBuilder
                            .AppendLine("[BinaryContent]");
                        break;

                    default:
                        stringBuilder
                            .AppendLine($"[{item.GetType().Name}]");
                        break;
                }
            }

            stringBuilder
                .AppendLine();
        }

        return stringBuilder
            .ToString();
    }
}