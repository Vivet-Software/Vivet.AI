# Vivet.AI
[![Build and Deploy](https://github.com/Vivet-Software/Vivet.AI/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Vivet-Software/Vivet.AI/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Vivet.AI.svg)](https://www.nuget.org/packages/Vivet.AI)
[![NuGet](https://img.shields.io/nuget/v/Vivet.AI.svg)](https://www.nuget.org/packages/Vivet.AI)
![License](https://img.shields.io/badge/license-Polyform%20Noncommercial%201.0.0-blue)
![GitHub Issues](https://img.shields.io/github/issues/Vivet-Software/Vivet.AI)

Unlock the full power of AI in your .NET applications with a **comprehensive library** for chat, embeddings, memory, 
knowledge, metadata, and summarization. Instantly enrich conversations and documents with **context, structured metadata, 
and insights**, including **real-time streaming**, **multimodal content** (images, audio, video), **advanced text chunking**, 
and **context deduplication**. Track usage, override configurations on the fly, and plug in custom implementations with ease. 
Build **smarter, faster, and context-aware AI experiences** with minimal boilerplate.  

The library supports **all major orchestration frameworks** and a variety of **vector stores** for memory and knowledge management. 
Every service follows a **request/response pattern**, includes **token and performance tracking**, and allows **per-request 
configuration overrides**.  

_Based on [Microsoft.SemanticKernel](https://learn.microsoft.com/en-us/semantic-kernel/overview/)_.  

## Table of Contents
### 🎛️ [Orchestrations](#%EF%B8%8F-orchestrations-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [OpenAI](#-azure-openai)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Azure OpenAI](#-azure-openai)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Azure AI Inference](#-azure-ai-inference)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [HuggingFace](#-huggingface)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Ollama](#-ollama)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Google Gemini](#-google-gemini)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Amazon Bedrock](#-amazon-bedrock)  

### 🗄️ [Vector Stores](#%EF%B8%8F-vector-stores-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Qdrant](#-qdrant)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Pinecone](#-pinecone)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Weaviate](#-weaviate)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Postgres (pgvector)](#-postgres-pgvector)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔹 [Azure AI Search](#-azure-ai-search)  

### ✨ [Services](#-services-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🗨️ [Chat](#%EF%B8%8F-chat-service)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🧩 [Embedding](#-embedding)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🧠 [Memory](#-embedding-memory-service)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;📚 [Knowledge](#-embedding-knowledge-service)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🗂️ [Metadata](#%EF%B8%8F-metadata-service)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;✂️ [Summarization](#%EF%B8%8F-summarization-service)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🕹️ [Agents](#%EF%B8%8F-agents-service)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🗣️ [Transcription](#%EF%B8%8F-transcription-service)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;👁️ [Vision](#%EF%B8%8F-vision-service)  

### 🔌 [Plugins](#-plugins-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🧩 [Built-In Plugins](#-built-in-plugins)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🧰 [Custom Plugins](#-custom-plugins)  

### ⚡ [Core Service Concepts](#-core-service-concepts-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;📩 [Request/Response Pattern](#-requestresponse-pattern)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⚙️ [Request Configuration Overrides](#-request-configuration-overrides)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⛔ [Error Handling](#-error-handling)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;💰 [Token & Performance Tracking](#-token--performance-tracking)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🛠️ [Extensible Implementations](#%EF%B8%8F-extensible-implementations)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;💚 [Health Checks](#-health-checks)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;📈 [Observability](#-observability)  

### 💡 [Other Highlighted Features](#-other-highlighted-features-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔀 [Advanced Text Chunking](#-advanced-text-chunking)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🧹 [Context Deduplication](#-context-deduplication)  

### 📎 [Appendix](#-appendix-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🔑 [Licensing](#-licensing)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⚙️ [Complete Configuration](#%EF%B8%8F-appsettings)
<br /><br /><br />

## 🎛️ Orchestrations
The library provides a **unified orchestration layer** across multiple AI providers, allowing you to integrate, configure, and switch between them with minimal effort.  
Instead of writing provider-specific code, you work against a consistent abstraction that keeps your business logic clean and portable.  

This makes it easy to:  
- Swap between providers (e.g., OpenAI → Azure OpenAI) without refactoring.  
- Experiment with different backends to optimize cost, performance, or capability.  
- Standardize advanced features like chat parameters, streaming, and error handling across all orchestrations.  

The following sections describe each supported orchestration in detail, including how to register it and which chat model parameters are available.  

### ⚙️ Configuration
All orchestrations are configured under the top-level `Ai` section in your `appsettings.json`.
Each **service** has its own configuration subsection, and the **Plugins** section contains settings for enabling or disabling the different built-in plugins per service.
```json
{
  "Ai": {
    "Endpoint": null,
    "ApiKey": null,
    "ApiKeyId": null,
    "Chat": { },
    "Embedding": { },
    "Metadata": { },
    "Summarization": { },
    "Agents": { }
  }
}
```

### 📑 Configuration Details
This is main ```appsettings``` configuration.  
The configuration of `Chat`, `Embedding`, `Metadata`, `Summarization` and `Agents` is detailed under their respective sections.   

| Setting           | Type   | Default | Description                                                                        |
| ----------------- | ------ | ------- | ---------------------------------------------------------------------------------- |
|  `Endpoint`       | string | null    | The endpoint (or AWS region) of the AI provider. Can be `null` if not required.    |
|  `ApiKey`         | string | null    | The API key of the AI provider. Can be `null` if not required.                     |
|  `ApiKeyId`       | string | null    | The API key identifier, depending on the provider. Can be `null` if not required.  |
|  `Chat`           |        |         | See [Chat Configuration](#%EF%B8%8F-chat-service).                                 |
|  `Embedding`      |        |         | See [Embedding Configuration](#-embedding).                                        |
|  `Metadata`       |        |         | See [Metadata Configuration](#%EF%B8%8F-metadata-service).                         |
|  `Summarization`  |        |         | See [Summarization Configuration](#%EF%B8%8F-summarization-service).               |
|  `Agents`         |        |         | See [Agents Configuration](#%EF%B8%8F-agents-service).                             |
|  `Plugins`        |        |         | See [Plugins Configuration](#-plugins-1).                                            |

The table below shows the required configuration values (`Endpoint`, `ApiKey`, and `ApiKeyId`) for each supported orchestration provider.  
This helps you quickly identify which settings need to be provided for each backend before integrating it into your application.  
Use this as a reference when setting up your `Ai` section in `appsettings.json`.

| Setting      | OpenAI   | Azure OpenAI | Azure InferenceAI | HuggingFace | Ollama | Google Gemini | Amazon Bedrock   |
| ------------ | -------- | ------------ | ---------------- | ------------ | ------ | -------------- | --------------- |
| `Endpoint`   | ✅      | ✅           | ✅               | ✅          | ✅     | ❌            | ℹ️              |
| `ApiKey`     | ✅      | ✅           | ✅               | ✅          | ❌     | ✅            | ✅              |
| `ApiKeyId`   | ❌      | ❌           | ❌               | ❌          | ❌     | ❌            | ✅              |

> ℹ️ Consult the individual provider sections below for details on support and usage of the configuration values.

### 🛠️ Supported Chat Model Parameters
Chat models are used across multiple services and can be configured individually.  
The table summarizes parameter support for each provider.

| Chat Model Parameter | OpenAI | Azure OpenAI | Azure AI Inference | HuggingFace | Ollama  | Google Gemini | Amazon Bedrock |
| -------------------- | ------ | ------------ | ------------------ | ----------- | ------- | ------------- | -------------- |
| `MaxOutputTokens`    | ✅    | ✅           | ✅                | ✅          | ✅     | ✅            | ✅             |
| `Temperature`        | ✅    | ✅           | ✅                | ✅          | ✅     | ✅            | ✅             |
| `StopSequences`      | ✅    | ✅           | ✅                | ✅          | ✅     | ✅            | ✅             |
| `Seed`               | ✅    | ✅           | ✅                | ✅          | ❌     | ❌            | ℹ️             |
| `PresencePenalty`    | ✅    | ✅           | ✅                | ✅          | ❌     | ❌            | ℹ️             |
| `FrequencyPenalty`   | ✅    | ✅           | ✅                | ✅          | ❌     | ❌            | ℹ️             |
| `RepetitionPenalty`  | ❌    | ❌           | ❌                | ✅          | ❌     | ❌            | ❌             |
| `TopP`               | ✅    | ✅           | ✅                | ✅          | ✅     | ✅            | ✅             |
| `TopK`               | ❌    | ❌           | ❌                | ✅          | ✅     | ✅            | ℹ️             |
| `ReasoningEffort`    | ✅    | ✅           | ❌                | ❌          | ❌     | ❌            | ❌             |

> ℹ️ Consult the individual provider sections below for details on support for chat model parameters.

### 🔹 OpenAI
OpenAI provides access to the GPT-family models.  

##### Register using `appsettings.json`
```csharp
services
    .AddVivetOpenAi();
```
##### Register using inline configuration
```csharp
services
    .AddVivetOpenAi(options =>
    {
        options.ApiKey = "<your-api-key>";
        options.Endpoint = "<your-endpoint>";
        // Configure additional options for chat, embedding, etc
    });
```

### 🔹 Azure OpenAI
Azure OpenAI provides access to the GPT-family models through a secure, enterprise-ready platform on Azure.  

##### Register using `appsettings.json`
```csharp
services
    .AddVivetAzureOpenAi();
```
##### Register using inline configuration
```csharp
services
    .AddVivetAzureOpenAi(options =>
    {
        options.ApiKey = "<your-api-key>";
        options.Endpoint = "<your-endpoint>";
        // Configure additional options for chat, embedding, etc
    });
```

### 🔹 Azure AI Inference
Azure AI Inference allows inference on various LLMs via Azure endpoints with enterprise features.

##### Register using `appsettings.json`
```csharp
services
    .AddVivetAzureAIInference();
```
##### Register using inline configuration
```csharp
services
    .AddVivetAzureAIInference(options =>
    {
        options.ApiKey = "<your-api-key>";
        options.Endpoint = "<your-endpoint>";
        // Configure additional options for chat, embedding, etc
    });
```

### 🔹 HuggingFace
HuggingFace models can be used directly via this library for custom inference workflows.

##### Register using `appsettings.json`
```csharp
services
    .AddVivetHuggingFace();
```
##### Register using inline configuration
```csharp
services
    .AddVivetHuggingFace(options =>
    {
        options.ApiKey = "<your-api-key>";
        options.Endpoint = "<your-endpoint>";
        // Configure additional options for chat, embedding, etc
    });
```

### 🔹 Ollama
Ollama provides local model inference and supports temperature-based sampling.

##### Register using `appsettings.json`
```csharp
services
    .AddVivetHOllama();
```
##### Register using inline configuration
```csharp
services
    .AddVivetHOllama(options =>
    {
        options.Endpoint = "<your-host>";
        // Configure additional options for chat, embedding, etc
    });
```

### 🔹 Google Gemini
Google Gemini allows structured and generative responses via its LLM APIs.

##### Register using `appsettings.json`
```csharp
services
    .AddVivetGoogleGemini();
```
##### Register using inline configuration
```csharp
services
    .AddVivetGoogleGemini(options =>
    {
        options.ApiKey = "<your-api-key>";
        // Configure additional options for chat, embedding, etc
    });
```

### 🔹 Amazon Bedrock
Amazon Bedrock supports multiple models: Claude, Cohere Command, Cohere Command-R, AI21 Labs Jamba/Jurassic, Mistral, Titan, Llama.

##### Register using `appsettings.json`
```csharp
services
    .AddVivetAmazonBedrock();
```
##### Register using inline configuration
```csharp
services
    .AddVivetAmazonBedrock(options =>
    {
        options.Endpoint = "<your-aws-region>";
        options.ApiKey = "<your-access-key>";
        options.ApiKeyId = "<your-secret-key>";
        // Configure additional options for chat, embedding, etc
    });
```
> ℹ️ Specify your AWS region as the Endpoint. Amazon Bedrock maps it internally instead of using a full endpoint.

#### Amazon Bedrock Model-Specific Chat Model Parameters
Different Amazon Bedrock models support different sets of chat parameters.
The table summarizes parameter support across the available models.

| Parameter            | Claude | Cohere Command  | Cohere Command-R | AI21 Jamba  | AI21 Jurassic | Mistral  | Titan | Llama3 |
| -------------------- | ------ | --------------- | ---------------- | ----------- | ------------- | -------- | ----- | ------ |
| `MaxOutputTokens`    | ✅     | ✅             | ✅               | ✅         | ✅            | ✅      | ✅    | ✅     |
| `Temperature`        | ✅     | ✅             | ✅               | ✅         | ✅            | ✅      | ✅    | ✅     |
| `StopSequences`      | ✅     | ✅             | ✅               | ✅         | ✅            | ✅      | ✅    | ✅     |
| `Seed`               | ❌     | ❌             | ✅               | ✅         | ❌            | ❌      | ❌    | ❌     |
| `PresencePenalty`    | ❌     | ❌             | ✅               | ✅         | ❌            | ❌      | ❌    | ❌     |
| `FrequencyPenalty`   | ❌     | ❌             | ✅               | ✅         | ❌            | ❌      | ❌    | ❌     |
| `RepetitionPenalty`  | ❌     | ❌             | ❌               | ❌         | ❌            | ❌      | ❌    | ❌     |
| `TopP`               | ✅     | ✅             | ✅               | ✅         | ✅            | ✅      | ✅    | ✅     |
| `TopK`               | ✅     | ✅             | ✅               | ❌         | ❌            | ✅      | ❌    | ❌     |
| `ReasoningEffort`    | ❌     | ❌             | ❌               | ❌         | ❌            | ❌      | ❌    | ❌     |

<br /><br />

## 🗄️ Vector Stores
Vector stores are specialized databases designed for storing and searching embeddings.  
In this library, they are used with the **Embedding Memory** and **Embedding Knowledge** services to enable semantic search and context retrieval.  

### 🔹 Qdrant
[Qdrant](https://qdrant.tech) ⤴ is a high-performance open-source vector database optimized for semantic search and recommendation systems.  

#### Start with Docker
```powershell
docker run -p 6333:6333 -p 6334:6334 `
  -v qdrant_storage:/qdrant/storage `
  -e QDRANT__SERVICE__API_KEY=secret `
  qdrant/qdrant
```

#### Dashboard:
http://localhost:6333/dashboard ⤴
<br /><br />

### 🔹 Pinecone
[Pinecone](https://www.pinecone.io) ⤴ is a fully managed, cloud-native vector database with focus on scalability and production-readiness.
It does not run locally with Docker; you must create an account and use the hosted API.

#### Access
https://app.pinecone.io ⤴
<br /><br />
 
### 🔹 Weaviate
[Weaviate](https://weaviate.io) ⤴ is an open-source vector search engine with a strong plugin ecosystem and GraphQL-based API.

#### Start with Docker
```powershell
docker run -p 8080:8080 `
  -e AUTHENTICATION_ANONYMOUS_ACCESS_ENABLED=true `
  semitechnologies/weaviate
```

#### Dashboard / API Explorer
http://localhost:8080 ⤴
<br /><br />

### 🔹 Postgres (pgvector)
[pgvector](https://github.com/pgvector/pgvector) ⤴ is a PostgreSQL extension that adds vector similarity search, combining the reliability of Postgres with embedding capabilities.

#### Start with Docker
```powershell
docker run -p 5432:5432 `
  -e POSTGRES_PASSWORD=secret `
  ankane/pgvector
```

#### Admin UI
You can connect with any Postgres client or use pgAdmin
http://localhost:5050 ⤴
<br /><br />

### 🔹 Azure AI Search
[Azure AI Search](https://learn.microsoft.com/azure/search) ⤴ (formerly Cognitive Search) supports hybrid search with both text and vector embeddings, fully managed on Azure.

#### Access
Provision an Azure AI Search resource in the Azure portal.

#### Dashboard: 
https://portal.azure.com ⤴
<br /><br />

## ✨ Services
The library provides a rich set of services including **Chat**, **Embedding**, **Embedding Memory**, **Embedding Knowledge**, **Metadata**, 
**Summarization** and **Agents**. Each service is designed to be modular, configurable, and optimized for advanced AI workflows. They can be used independently or combined to build powerful orchestration pipelines. New services and AI model integrations are continuously being added to expand functionality of the library and keep pace with the AI ecosystem.  

Detailed explanations and usage examples for each service are provided in the following sections.
<br /><br />

## 🗨️ Chat Service
The `IChatService` combines LLMs, memory, knowledge bases, and multimodal context into a single conversational API. It supports **plain text and typed JSON responses**, **real-time streaming**, and automatic **memory + knowledge enrichment**. Developers can attach blobs (documents, images, audio, video), and the service automatically extracts **summary and description metadata** to ground the conversation. With built-in support for **reasoning transparency**, **token usage tracking**, and **automatic memory indexing**, `ChatService` provides everything needed to build intelligent, context-aware chat applications on .NET.

#### Methods
- `ChatAsync` returns a **plain string answer** plus metadata (reasoning, thinking trace, token usage, raw output, elapsed execution time, and reconstructed input prompt).
- `ChatAsync<T>` supports **typed responses**, where the LLM is instructed in the prompt to return JSON matching the specified type. The service automatically deserializes that JSON into your .NET type.  
  ⚠️ **Note:** The model will automatically output JSON that matches the type `T` in the response. No need to manually add the JSON schema to the system message or question of the chat request.
- `ChatStreamingAsync` allows **real-time streaming** of the model’s output, returning content token-by-token (or chunk-by-chunk) as it is generated. At the end of the stream, the service automatically saves the conversation to memory and optionally invokes a completion callback. Supports the same features as `ChatAsync`.

#### Memory & Knowledge Integration (Plugin)
- Through optional built-in plugins, requests can be enriched with **long-term memories** and **knowledge entries** retrieved using **approximate nearest neighbor (ANN) search** for efficient similarity matching.
- Both **memory** and **knowledge** support **multi-dimensional segmentation** to scope retrieval:
  - **Memory segmentation**: `ScopeId`, `UserId`, `AgentId`, and `ThreadId` ensure the most relevant user- and thread-specific context is used.  
  - **Knowledge segmentation**: `ScopeId`, `TenantId`, `SubTenantId` and `UserId` allow fine-grained retrieval from organizational knowledge bases.  
- Built-in **deduplication** ensures only the most relevant and unique context is injected into the prompt.  
- **Thread-awareness** boosts relevance by prioritizing memories from the active conversation.
- The chat model determines if and when to include memory and knowledge in the context, based on the user’s query.

#### Web Search (Plugin)
- Enables the chat model to perform external web searches through a configurable provider (Google, Bing, etc.).  
- Web search is used when additional or updated context is required that is not available in the model's training data or memory.

#### Blob Metadata Enrichment
- You can attach blobs (e.g., PDFs, images, videos, audio files) to a `ChatRequest`.
- The service automatically extracts and indexes **summary and description metadata**, making it available to the model as part of the prompt without preprocessing. This requires metadata processing to be enabled and configured in `appsettings`; otherwise, metadata must be passed alongside the blob in the `ChatRequest`.

#### Reasoning Transparency
When supported by the provider (e.g., **DeepSeek R1**), the service exposes:
- **Reasoning**: a concise explanation of why an answer was provided.
- **Thinking**: a detailed breakdown of the model’s step-by-step thought process.

#### Automatic Asynchronous Memory Indexing
- Questions and answers are persisted to memory using the `IEmbeddingMemoryService` (if memory embedding is configured in `appsettings`).
- Optional callbacks (`onMemoryIndexed`) allow you to hook into the lifecycle for logging or analytics.

#### Custom Plugins
Custom plugins extend the chat model with your own functionality. They can be added **Per request** – Passed with a specific `ChatRequest`, giving fine-grained control. Ensure that dependencies to a plugin is registered as dependencies in `IServiceCollection`.  

When plugins are available, the chat model automatically decides whether to invoke them based on the user’s query. This is by design — the model plans and decides when and how to use plugins.  
- For custom plugins, if you require a plugin to **always** be invoked, call it manually in your application and include its result in the system message of the request.  
- Custom plugin parameters should be passed in the `SystemMessage` of the `ChatRequest`, or derived from existing context in the request (`UserId`, `TenantId`, etc.).

- 📖 Read more about [Plugins](#-plugins-1)

### ⚙️ Chat Configuration
Example `appsettings.json` snippet showing how to configure `IChatService` under the `"Ai"` section:
```json
"Ai": {
  "Chat": {
    "Model": {
      "Name": "<your-chat-model>",
      "UseHealthCheck": true,
      "Parameters": {
        "MaxOutputTokens": 2048,
        "Temperature": null,
        "StopSequences": [],
        "Seed": null,
        "PresencePenalty": null,
        "FrequencyPenalty": null,
        "RepetitionPenalty": null,
        "TopP": null,
        "TopK": null,
        "ReasoningEffort": null
      }
    },
    "Timeout": "00:01:00",
    "Plugins": {
      "EnableMemoryPlugin": true,
      "EnableKnowledgePlugin": true,
      "EnableWebSearchPlugin": true
    }
  }
}
```

### 📑 Chat Configuration Details

| Setting                                    | Type              | Default    | Description                                                                                                                                                      |
| ------------------------------------------ | ----------------- | ---------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Chat`                                     |                   |            | Chat configuration.                                                                                                                                              |
| `Chat.Model`                               |                   |            | The chat model configuration.                                                                                                                                    |
| `Chat.Model.Name`                          | string            | `null`     | Specifies the chat model to use (e.g., GPT-4.1). Must be configured in the chosen AI provider. _The configured model may be overridden for individual requests._ |
| `Chat.Model.UseHealthCheck`                | bool              | `true`     | Whether to perform a health check on the model before use.                                                                                                       |
| `Chat.Model.Parameters`                    |                   |            | The chat model parameters.                                                                                                                                       |
| `Chat.Model.Parameters.MaxOutputTokens`    | int               | `2048`     | Maximum number of output tokens to generate.                                                                                                                     |
| `Chat.Model.Parameters.Temperature`        | float?            | `null`     | Sampling temperature (0–1), controlling randomness.                                                                                                              |
| `Chat.Model.Parameters.StopSequences`      | string[]          | `[]`       | Text sequences that will stop generation.                                                                                                                        |
| `Chat.Model.Parameters.Seed`               | long?             | `null`     | Optional seed for deterministic output.                                                                                                                          |
| `Chat.Model.Parameters.PresencePenalty`    | float?            | `null`     | Penalty for generating tokens already present in the text.                                                                                                       |
| `Chat.Model.Parameters.FrequencyPenalty`   | float?            | `null`     | Penalty for generating tokens repeatedly.                                                                                                                        |
| `Chat.Model.Parameters.RepetitionPenalty`  | float?            | `null`     | Penalizes repeated token usage within the generation.                                                                                                            |
| `Chat.Model.Parameters.TopP`               | float?            | `null`     | Nucleus sampling probability mass.                                                                                                                               |
| `Chat.Model.Parameters.TopK`               | int?              | `null`     | Limits candidate tokens considered per generation step.                                                                                                          |
| `Chat.Model.Parameters.ReasoningEffort`    | ReasoningEffort?  | `null`     | Effort level to reduce reasoning complexity or token usage.                                                                                                      |
| `Chat.Timeout`                             | TimeSpan          | `00:01:00` | Maximum time allowed for a chat request.                                                                                                                         |
| `Chat.Plugins`                             |                   |            | Options for configuring built-in chat plugins. See The [Plugins](#-plugins-1).                                                                                   |
| `Chat.Plugins.EnableMemoryPlugin`          | bool              |            | Enables or disables the built-in Memory plugin. The [Embedding Memory](#-embedding-memory-service) must be configured for this setting to take effect.           |
| `Chat.Plugins.EnableKnowledgePlugin`       | bool              |            | Enables or disables the built-in Knowledge plugin. The [Embedding Knowledge](#-embedding-knowledge-service) must be configured for this setting to take effect.  |
| `Chat.Plugins.EnableWebSearchPlugin`       | bool              |            | Enables or disables the built-in Web Search plugin. The [Web Search](#-web-search) must be configured for this setting to take effect.                           |

### 🚀 Example Usage
#### Resolve the service from DI
```csharp
var chatService = serviceProvider.GetService<IChatService>();
```
#### Chat request with explicit blob metadata
```csharp
var request = new ChatRequest
{
    Question = "Summarize the attached document in 3 bullet points.",
    UserId = "user-id",
    CurrentThreadId = "thread-id",
    Blobs = 
    [
        new ImageBlob
        {
            Data = new BlobDataBase64 { Base64 = "base64" }, // or File, Uri, Stream, etc. 
            MimeType = ImageMimeType.Png,
            Metadata = new Metadata  // If Metadata is null, it will be fetched from the blob when configured in appsettings
            {
                Title = "Quarterly Report Graph",
                Description = "Q2 financial summary graph"
            }
        }
    ],
    // optional: SystemMessage, TenantId, SubTenantId, ScopeId, AgentId, Language, Config Overrides, etc.
};

var onMemoryIndexedTask = new TaskCompletionSource<bool>();

var response = await chatService
    .ChatAsync(request, memoryResponse => 
    { 
        try
        {
            // Handle callback.

            onMemoryIndexedTask.SetResult(true);
        }
        catch (Exception ex)
        {
            onMemoryIndexedTask.SetException(ex);
        }

        return Task.CompletedTask;
    });

Console.WriteLine($"Answer: {response.Answer}");
Console.WriteLine($"Reasoning: {response.Reasoning}");

await onMemoryIndexedTask.Task;
```
#### Typed response (question must instruct model to output valid JSON matching the type)
```csharp
public class WeatherForecast
{
    public string Location { get; set; }
    public string Condition { get; set; }
    public int TemperatureC { get; set; }
}

var typedRequest = new ChatRequest
{
    Question = """
        Provide a weather forecast as JSON matching this schema:
        { "Location": string, "Condition": string, "TemperatureC": int }
        """,
    UserId = "user-id",
    CurrentThreadId = "thread-id",
};

var typedResponse = await chatService
    .ChatAsync<WeatherForecast>(typedRequest);

Console.WriteLine($"{typedResponse.Answer.Location}: {typedResponse.Answer.Condition}, {typedResponse.Answer.TemperatureC}");
```
#### Streaming request and response
```csharp
await foreach (var chunk in chatService
    .ChatStreamingAsync(request, memoryResponse => { /* Handle memory indexed callback */ }, chatResponse => { /* Handle chat completed callback */ }))
{
    Console.Write(chunk);
}
```
<br /><br />

## 🧩 Embedding
The **Embedding** configuration contains settings shared by both **Memory** and **Knowledge**, including the embedding **model**, **vector size**, **match score threshold**, and **timeout**.
**Memory** and **Knowledge** also defines configuration thatn are specific to each respectively, and are documented separately below.  

### ⚙️ Configuration
```jsonc
"Ai": {
  "Embedding": {
    "Model": {
      "Name": "<your-embedding-model>",
      "UseHealthCheck": true
    },
    "VectorSize": 1536,
    "Timeout": "00:01:00",
    "Memory": {
      "Indexing":{
        "TextChunking": { }
      },
      "Search": { 
        "Scoring": { },
      },
      "VectorStore": { }
    },
    "Knowledge": {
      "Indexing":{
        "TextChunking": { }
      },
      "Search": { 
        "Scoring": { },
      },
      "VectorStore": { }
    }
  }
}
```

### 📑 Common Embedding Configuration Details

| Setting                | Type     | Default    | Description                                                                                                   |
| ---------------------- | -------- | ---------- | ------------------------------------------------------------------------------------------------------------- |
| `Model`                |          |            | Embedding model configuration.                                                                                |
| `Model.Name`           | string   | `null`     | Name of the embedding model (must be supported by the chosen AI provider). _The configured model may be overridden for individual requests, Use with caution, as different models generate embeddings differently, which may lead to misalignment with existing embeddings._ |
| `Model.UseHealthCheck` | bool     | `true`     | Whether to validate the embedding model on startup.                                                           |
| `VectorSize`           | int      | `1536`     | Embedding dimension size. Depends entirely on the model used.                                                 |
| `Timeout`              | TimeSpan | `00:01:00` | Timeout for embedding operations.                                                                             |
| `Memory`               |          |            | Memory configuration. See [Embedding Memory](#-embedding-memory-service).                                     |
| `Knowledge`            |          |            | Knowledge configuration. See [Embedding Knowledge](#-embedding-knowledge-service).                            |

#### 📊 Indexing
Defines setting related to indexing new content in the vector store.  
```json
"Indexing": {
  "TextChunking": {
    "MinTokens": 20,
    "MaxTokens": 60,
    "NeighborContext": {
      "ContextWindow": 1,
      "RestrictToSameParagraph": true
    }
  }
}
```

| Setting                                                         | Type | Default | Description                                                                                            |
| --------------------------------------------------------------- | ---- | ------- | ------------------------------------------------------------------------------------------------------ |
| `Indexing.TextChunking.MinTokens`                               | int  | `20`    | Minimum number of tokens per chunk. (Approximation)                                                    |
| `Indexing.TextChunking.MaxTokens`                               | int  | `60`    | Maximum number of tokens per chunk. Sentences are merged until this limit is reached. (Approximation)  |
| `Indexing.TextChunking.NeighborContext`                         |      |         | Neighbor context configuration.                                                                        |
| `Indexing.TextChunking.NeighborContext.ContextWindow`           | int  | `1`     | How many chunks before/after are stored as contextual neighbors.                                       |
| `Indexing.TextChunking.NeighborContext.RestrictToSameParagraph` | bool | `true`  | Whether neighbors must belong to the same paragraph.                                                   |

⚠️ **Note:** Read more about [Advanced Text Chunking](#-advanced-text-chunking)  

#### 🧮 Search
Defines settings used when querying the vector store.  
Scoring defines the weight configuration for **approximate nearest neighbor search (ANN)** ranking.
```json
"Search": {
  "UseQueryDeduplication": true,
  "ContextQueryLimit": 3,
  "Scoring": {
    "MatchScoreThreshold": 0.86,
    "DeduplicationMatchScoreThreshold": 0.90,
    "RecencyDecayStrategy": "Linear",
    "RecencyBoostMax": 0.1,
    "RecencyDecayDays": 30,
    "RecencySigmoidSteepness": 1.0
}
```

| Setting                                            | Type   | Default  | Description                                                                          |
| -------------------------------------------------- | ------ | -------- | ------------------------------------------------------------------------------------ |
| `Search.UseQueryDeduplication`                     | bool   | true     | Deduplicate similar memory entries before building context.                          |
| `Search.ContextQueryLimit`                         | int    | 3        | Maximum number of memory entries retrieved per query.                                |
| `Search.Scoring.MatchScoreThreshold`               | float  | `0.86`   | Cosine similarity threshold for semantic matches (see below for recommended ranges). |
| `Search.Scoring.DeduplicationMatchScoreThreshold`  | double | 0.90     | Fuzzy similarity threshold for deduplication.                                        |
| `Search.Scoring.RecencyDecayStrategy`              | enum   | `Linear` | How recency scores decay over time (`Linear`, `Exponential`, `Sigmoid`).             |
| `Search.Scoring.RecencyBoostMax`                   | double | `0.1`    | Max boost applied to the newest entries.                                             |
| `Search.Scoring.RecencyDecayDays`                  | double | `30`     | Days until recency boost becomes negligible.                                         |
| `Search.Scoring.RecencySigmoidSteepness`           | double | `1.0`    | Steepness of the curve (only used for Sigmoid).                                      |

##### Recommended Match Score Thresholds
* 0.00 - 0.70: Often noise, unless domain is very narrow.
* 0.70 - 0.80: Related but not identical (looser recall, brainstorming).
* 0.80 - 0.85: Good semantic match (typical retrieval threshold).
* 0.90+: Very strong / near-duplicate matches.

#### 🗄️ Vector Store
Defines which vector database to use for embedding storage and retrieval.
```json
"VectorStore": {
  "Provider": "None",
  "Host": "localhost",
  "Port": 6334,
  "Username": null,
  "ApiKey": null,
  "Timeout": "00:00:30",
  "UseHealthCheck": true
}
```

| Setting          | Type     | Default     | Description                                                                                                        |
| ---------------- | -------- | ----------- | ------------------------------------------------------------------------------------------------------------------ |
| `Provider`       | enum     | `None`      | Vector DB provider (`Qdrant`, `Pinecone`, etc.). See [Supported Vector Stores](#%EF%B8%8F-supported-vector-stores) |
| `Host`           | string   | `localhost` | Vector DB host.                                                                                                    |
| `Port`           | int      | `6334`      | Vector DB port.                                                                                                    |
| `Username`       | string   | `null`      | Optional username. Used by some providers                                                                          |
| `ApiKey`         | string   | `null`      | Required if authentication is enabled.                                                                             |
| `Timeout`        | TimeSpan | `00:00:30`  | Query timeout.                                                                                                     |
| `UseHealthCheck` | bool     | `true`      | Whether to check connectivity periodically.                                                                        |

<br /><br />

## 🧠 Embedding Memory Service
The `IEmbeddingMemoryService` provides semantic memory storage and retrieval built on embeddings.  
It allows you to persist question-answer pairs, blobs, and metadata as vectorized memories, and later recall them using semantic search, filters, and contextual scoring.

#### Indexing  
  - `IndexAsync<T>` stores question/answer pairs (structured or unstructured), optional blobs, and metadata.  
  - Supports **automatic summarization** (via `ISummarizationService`) to reduce verbosity and improve retrieval quality.  
  - Splits text into chunks, generates embeddings, and links related question/answer contexts for richer semantic connections.  
  - Automatically attaches blob metadata — either provided explicitly or auto-retrieved by `IMetadataService`.

#### Semantic Search  
  - `SearchAsync` retrieves the **most relevant memories** using vector similarity.  
  - Enhances retrieval with **recency scoring** and **same-thread boosting**, so newer or contextually relevant memories are prioritized.  
  - Supports advanced filtering through `MemoryCriteria`.

#### Querying  
  - `QueryAsync` retrieves memories based on structured criteria (user, agent, thread, question/answer flags, date ranges).  
  - Provides pagination support with `Limit` and `Skip`.  
  - Returns raw memory entries with their content, context, and size (in bytes).

#### Deletion  
  - `DeleteAsync` removes memories by ID(s) from the vector store.  
  - Ensures full control over memory lifecycle.

### ⚙️ Embedding Memory Configuration
The **Embedding Memory** configuration contains settings specific to memory handling that are not shared with Knowledge.  
All **TextChunking**, **Scoring**, and **VectorStore** options are already documented in the [Common Embedding Configuration](#common-embedding-configuration-details) section.  

The unique memory-specific setting is:
```json
"Memory": {
  "Indexing": {
    "UseExtendedMemoryContext": true,
    "UseAutomaticSummarization": false,
    "UseAutomaticMetadataRetrieval": true,
    "TextChunking": { },
  }
  "Search": { 
    "RetentionInDays": 180,
    "CounterpartContextQueryLimit": 2,
    "Scoring": { 
      "ThreadMatchBoost": 0.2
    },
  },
  "VectorStore": { }
}
```

### 📑 Embedding Memory Configuration Details

| Setting                                  | Type    | Default   | Description                                                                                                                 |
| ---------------------------------------- | ------- | --------- | --------------------------------------------------------------------------------------------------------------------------- |
| `Indexing`                               |         |           | Memory indexing configuration. [Indexing Configuration](#-indexing)                                                               |
| `Indexing.UseExtendedMemoryContext`      | bool    | `true`    | Enables counterpart lookups so the LLM can reference previous answers to similar questions.                                 |
| `Indexing.UseAutomaticSummarization`     | bool    | `false`   | Enable or disable automatic summarization of memories.                                                                      |
| `Indexing.UseAutomaticMetadataRetrieval` | bool    | `true`    | If enabled, metadata is automatically extracted from documents/blobs (via `IMetadataService`) when not explicitly provided. |
| `Indexing.TextChunking`                  |         |           | Memory indexing text chunking configuration. [Indexing Text Chunking Configuration](#-indexing)                             |
| `Search`                                 |         |           | Memory search configuration. [Search Configuration](#-search)                                                               |
| `Search.RetentionInDays`                 | int     | `180`     | How far back memories will be included in queries.                                                                          |
| `Search.CounterpartContextQueryLimit`    | int     | `2`       | Maximum number of counterpart (Q/A pair) entries retrieved.                                                                 |
| `Search.Scoring`                         |         |           | Memory search scoring configuration. [Search Scoring Configuration](#-search)                                               |
| `Search.Scoring.ThreadMatchBoost`        | double  | `0.2`     | Boosts the score of memories that match the current conversation thread. Only applicable for Memory.                        |
| `VectorStore`                            |         |           | Memory vector store configuration. [Vector Store Configuration](#%EF%B8%8F-vector-store)                                    |

### 🚀 Example Usage
#### Resolve the service from DI
```csharp
var embeddingMemoryService = serviceProvider.GetService<IEmbeddingMemoryService>();
```
#### Index a memory entry
```csharp
var indexRequest = new IndexMemoryRequest<string>
{
    ThreadId = "thread-id",
    UserId = "user-id",
    Question = "What is the customer's preferred communication channel?",
    Answer = "Email",
    Blobs = new BaseBlobMetadata[] { } // optional
    // optional: Language, Config Overrides, etc.
};

var indexResponse = await embeddingMemoryService
    .IndexAsync(indexRequest);

Console.WriteLine($"Indexed embeddings: {indexResponse.TotalEmbeddings}");
Console.WriteLine($"Indexed embeddings size: {indexResponse.TotalEmbeddingsSize}");
```
#### Index typed a memory entry (json embedding)
```csharp
public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PreferredChannel { get; set; }
}

var indexRequest = new IndexMemoryRequest<Customer>
{
    ThreadId = "thread-id",
    UserId = "user-id",
    Question = "Customer details",
    Answer = new Customer { Name = "Alice Johnson", Email = "alice@example.com", PreferredChannel = "Email" },
    Blobs = new BaseBlobMetadata[] { } // optional
    // optional: Language, Config Overrides, etc.
};

var indexResponse = await embeddingMemoryService
    .IndexAsync(indexRequest);

Console.WriteLine($"Indexed embeddings: {indexResponse.TotalEmbeddings}");
Console.WriteLine($"Elapsed time: {indexResponse.ElapsedTime}");
```
#### Search for memories based on a query
```csharp
var searchRequest = new SearchMemoryRequest
{
    Query = "Preferred communication channel",
    Criteria = new MemoryCriteria
    {
        UserId = "user-id"
        ThreadId = "thread-id",
        // additional criteria
    },
    Limit = 5,
    CurrentThreadId = "current-thread" // optional: For boosting results of the current thread.
};

var searchResponse = await embeddingMemoryService
    .SearchAsync(searchRequest);

foreach (var result in searchResponse.Results)
{
    Console.WriteLine($"Score: {result.Score:0.00} | Text: {result.Result.Content}");
}
```
#### Query memories directly with filtering and paging
```csharp
var queryRequest = new QueryMemoryRequest
{
    Criteria = new MemoryCriteria
    {
        UserId = "user-id"
        ThreadId = "thread-id",
        // additional criteria
    },
    Limit = 5,
    Skip = 0
}

var queryResponse = await embeddingMemoryService
    .QueryAsync(queryRequest);

foreach (var memory in queryResponse.Results)
{
    Console.WriteLine($"Text: {memory.Result.Content}");
}
```
#### Delete specific memories by ID
```csharp
var deleteRequest = new DeleteRequest
{
    Ids = ["id"]
};

await embeddingMemoryService
    .DeleteAsync(deleteRequest);
```
<br /><br />

## 📚 Embedding Knowledge Service
The `IEmbeddingKnowledgeService` provides semantic knowledge storage and retrieval built on embeddings.  
It allows you to persist structured and unstructured knowledge (text, documents, images, audio, video, blobs, and metadata) into a vector store and later retrieve them using semantic similarity, filters, and contextual scoring.

#### Indexing  
  - `IndexAsync<T>` supports **text, documents, images, audio, and video**.  
  - Automatically serializes complex objects into JSON before embedding.  
  - Splits text into chunks, generates embeddings, and attaches **neighboring context** for richer semantic connections.  
  - Supports **automatic metadata retrieval** (via `IMetadataService`) when blob metadata is not provided.  
  - Returns detailed indexing results including total embeddings, size, and token usage.

#### Semantic Search  
  - `SearchAsync` retrieves the **most relevant knowledge entries** using vector similarity.  
  - Enhances scoring with **recency decay** so fresher knowledge is prioritized.  
  - Supports advanced filtering through `KnowledgeCriteria` (tenant, sub-tenant, scope, user, language, tags, and content type).

#### Querying  
  - `QueryAsync` retrieves knowledge entries directly from the vector store using **structured filters and ordering**.  
  - Does not apply semantic similarity scoring, useful for exact lookups.  
  - Provides pagination via `Limit` and `Skip`.  
  - Returns raw knowledge entries with their content, context, and size (in bytes).

#### Deletion  
  - `DeleteAsync` removes knowledge entries by ID(s) from the vector store.  
  - Ensures full control over knowledge lifecycle.

### ⚙️ Embedding Knowledge Configuration
The unique knowledge-specific setting is:

```json
"Knowledge": {
  "UseAutomaticMetadataRetrieval": true,
  "Indexing": { 
    "TextChunking": { },
  },
  "Search": { 
    "Scoring": { }
  },
  "VectorStore": { }
}
```

### 📑 Embedding Knowledge Configuration Details

| Setting                                  | Type   | Default | Description                                                                                                                 |
| ---------------------------------------- | ------ | ------- | --------------------------------------------------------------------------------------------------------------------------- |
| `Indexing`                               |        |         | Knowledge indexing configuration. [Indexing Configuration](#-indexing)                                                      |
| `Indexing.UseAutomaticMetadataRetrieval` | bool   | `true`  | If enabled, metadata is automatically extracted from documents/blobs (via `IMetadataService`) when not explicitly provided. |
| `Indexing.TextChunking`                  |        |         | Knowledge indexing text chunking configuration. [Indexing Text Chunking Configuration](#-indexing-text-chunking)            |
| `Search`                                 |        |         | Knowledge search configuration. [Search Configuration](#-search)                                                            |
| `Search.Scoring`                         |        |         | Memory search scoring configuration. [Search Scoring Configuration](#-search-scoring)                                       |
| `VectorStore`                            |        |         | Knowledge vector store configuration. [Vector Store Configuration](#%EF%B8%8F-vector-store)                                 |

### 🚀 Example Usage
#### Resolve the service from DI
```csharp
var embeddingKnowledgeService = serviceProvider.GetService<IEmbeddingKnowledgeService>();
```
#### Index plain text
```csharp
var indexRequest = new IndexTextRequest
{
    Text = "This device supports Bluetooth 5.3 and WiFi 6E."
    // optional: TenantId, SubTenantId, ScopeId, Source, CreatedBy, Tags, Config Overrides, etc
};

var indexResponse = await knowledgeService
    .IndexAsync(indexRequest);

Console.WriteLine($"Total embeddings: {indexResponse.TotalEmbeddings}");
Console.WriteLine($"Total size: {indexResponse.TotalEmbeddingsSize}");
```
#### Index typed a knowledge entry (json embedding)
```csharp
public class Product
{
    public string Name { get; set; }
    public string[] Features { get; set; }
}

var indexRequest = new IndexTextRequest<Product>
{
    Text = new Product { Name = "SmartSensor 3000", Features = new[] { "Bluetooth 5.3", "WiFi 6E", "10-year battery" } };
    // optional: TenantId, SubTenantId, ScopeId, Source, CreatedBy, Tags, Config Overrides, etc.
}

var indexResponse = await knowledgeService
    .IndexAsync(indexRequest);

Console.WriteLine($"Total embeddings (typed): {indexResponse.TotalEmbeddings}");
```
#### Index a blob (document/audio/image/video)
```csharp
var indexRequest = new IndexImageRequest
{
    Blob = new ImageBlob
    {
        Data = new BlobDataBase64 { Base64 = "base64" }, // or File, Uri, Stream, etc. 
        MimeType = ImageMimeType.Png,
        Metadata = new Metadata  // If Metadata is null, it will be automatically retrieved from the blob if Metadata is configured in appsettings.
        {
            Title = "Quarterly Report Graph",
            Description = "Q2 financial summary graph"
        }
    }
    // optional: TenantId, SubTenantId, ScopeId, Source, CreatedBy, Tags, etc.
};

var indexResponse = await knowledgeService
    .IndexAsync(indexRequest);

Console.WriteLine($"Indexed blob embeddings: {indexResponse.TotalEmbeddings}");
Console.WriteLine($"Metadata token usage: {indexResponse.MetadataTokenUsage?.InputTokens ?? 0}");
```
#### Search knowledge (semantic similarity)
```csharp
var searchRequest = new SearchKnowledgeRequest
{
    Query = "Which devices support WiFi 6E?",
    Criteria = new KnowledgeCriteria
    {
        TenantId = "tenant-id",
        // additional criteria
    },
    Limit = 5,
};

var searchResponse = await knowledgeService
    .SearchAsync(searchRequest);

foreach (var result in searchResponse.Results)
{
    Console.WriteLine($"Score: {result.Score:0.00} | Content: {result.Result.Content}");
}
```
#### Query knowledge (filtering / paging — no semantic scoring)
```csharp
var queryRequest = new QueryKnowledgeRequest
{
    Criteria = new KnowledgeCriteria
    {
        TenantId = "tenant-id",
        // additional criteria
    },
    Limit = 10,
    Skip = 0
};

var queryResponse = await knowledgeService
    .QueryAsync(queryRequest);

foreach (var result in queryResponse.Results)
{
    Console.WriteLine($"Id: {result.Result.Id} | Content size: {result.Size} bytes");
}
```
#### Delete specific knowledge by ID
```csharp
var deleteRequest = new DeleteRequest
{
    Ids = ["id"]
};

await embeddingMemoryService
    .DeleteAsync(deleteRequest);
```
<br /><br />

## 🗂️ Metadata Service
The `IMetadataService` provides structured metadata extraction from binary blob content such as **images, audio, video, and documents**. It uses a **chat completion model** with prompt templates to retrieve metadata automatically. The service supports both **basic metadata** (summary and description) and **strongly-typed additional metadata**. Every response also includes **elapsed time, token usage, and internal error information**, making it easy to track usage and performance.  

You don't need to invoke metadata manually. If confiured it will automatically be invoked when embedding memories.  

#### Flexible Metadata API
- `GetAsync(GetMetadataRequest request, CancellationToken cancellationToken)`  
  Returns **basic metadata only** (`Summary` and `Description`) inside `MetadataResponse`. Wraps the generic overload with `dynamic`.

- `GetAsync<T>(GetMetadataRequest request, CancellationToken cancellationToken) where T : class, new()`  
  Generic overload that returns **strongly-typed additional metadata** inside `MetadataResponse<T>`. Always includes:
  - `ElapsedTime` – total processing time  
  - `TokenUsage` – input/output token counts  
  - `ErrorMessage` – internal error message if any  
  - `Metadata` – extracted summary and description  
  - `AdditionalMetadata` – strongly-typed metadata when `T` is provided  

#### Blob Metadata Enrichment
- Attach blobs (PDFs, images, audio, video) to a metadata request.
- The service extracts **summary and description** automatically and, when using the generic overload, additional metadata according to your type `T`.
- Works out-of-the-box if **Metadata service is configured** in `appsettings.json`; otherwise, you must provide blobs in the request.

#### Usage Notes
- All blob processing is asynchronous.
- Ensure that your type `T` has **nullable properties** for optional metadata fields.

### ⚙️ Metadata Configuration
Example `appsettings.json` snippet showing how to configure `IMetadataService` under the `"Ai"` section:
```json
"Ai": {
  "Metadata": {
    "Model": {
      "Name": "<your-metadata-chat-model>",
    }
    "SummaryMaxWords": 30,
    "DescriptionMaxWords": 90,
    "Timeout": "00:01:00"
    }
  }
}
```

### 📑 Metadata Configuration Details

| Setting                        | Type     | Default   | Description                                                                                                                                                                                                           |
| ------------------------------ | -------- | --------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Metadata                       |          |           | Metadata service configuration.                                                                                                                                                                                       |
| Metadata.Model                 |          |           | Chat model configuration for metadata extraction. The model configuration is identical to [Chat Model Configuration](#-chat-configuration-details). _The configured model may be overridden for individual requests._ |
| Metadata.SummaryMaxWords       | int      | 30        | The max words to include for metadata summary.                                                                                                                                                                        |
| Metadata.DescriptionMaxWords   | int      | 90        | The max words to include for metadata description.                                                                                                                                                                    |
| Metadata.Timeout               | TimeSpan | 00:01:00  | Maximum time allowed for a metadata request.                                                                                                                                                                          |

### 🚀 Example Usage
#### Resolve the service from DI
```csharp
var metadataService = serviceProvider.GetService<IMetadataService>();
```
#### Get metadata
```csharp
public class InvoiceMetadata
{
    public string InvoiceNumber { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public decimal? TotalAmount { get; set; }
}

var metadataRequest = new GetMetadataRequest
{
    Blob = new DocumentBlob
    {
        Data = new BlobDataBase64
        {
            Base64 = "base64"
        },
        MimeType = ImageMimeType.Jpg
    }
};

var response = await metadataService
    .GetAsync<InvoiceMetadata>(metadataRequest);

Console.WriteLine($"Summary: {response.Metadata.Summary}");
Console.WriteLine($"Description: {response.Metadata.Description}");

Console.WriteLine($"Invoice Number: {response.AdditionalMetadata.InvoiceNumber}");
Console.WriteLine($"Invoice Date: {response.AdditionalMetadata.InvoiceDate}");
Console.WriteLine($"Total Amount: {response.AdditionalMetadata.TotalAmount}");
```
<br /><br />

## ✂️ Summarization Service
The `ISummarizationService` provides **memory summarization** for questions and answers using an LLM chat completion service. It supports **custom summarization degrees**, leaving inline JSON or XML untouched. Every response includes **elapsed time, token usage, and internal error information**, making it easy to track performance and usage.  

You don't need to invoke summarization manually. If confiured it will automatically be invoked when embedding memories.  

⚠️ **Note:** Currently, summarization is only supported for **memory embeddings**.


#### Flexible Summarization API
- `SummarizeMemoryAsync(SummarizeMemoryRequest request, CancellationToken cancellationToken)`  
  Summarizes a memory consisting of a **question and answer**. Returns a `SummarizationMemoryResponse` containing:
  - `QuestionSummarized` – the summarized question  
  - `AnswerSummarized` – the summarized answer  
  - `ElapsedTime` – total processing time  
  - `TokenUsage` – input/output token counts  
  - `ErrorMessage` – internal error message if any

- `SummarizationDegree`  
  Controls compression level:
  - `0` – No summarization  
  - `25` – Preserve nearly all details  
  - `50` – Keep core meaning, concise  
  - `75` – Summarize concisely, remove fluff  
  - `100` – Compress to most essential ideas only

#### Usage Notes
- All processing is **asynchronous**.
- Inline JSON or XML is **preserved** during summarization.
- Model parameters can be overridden in each request via `ChatModelParameters`.

### ⚙️ Summarization Configuration
Example `appsettings.json` snippet showing how to configure `ISummarizationService` under the `"Ai"` section:

```json
"Ai": {
  "Summarization": {
    "Model": {
      "Name": "<your-summarization-chat-model>",
    }
    "SummarizationDegree": 25,
    "Timeout": "00:01:00"
  }
}
```

### 📑 Summarization Configuration Details

| Setting                             | Type     | Default   | Description                                                                                                                                                                                                     |
| ----------------------------------- | -------- | --------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Summarization                       |          |           | Summarization service configuration.                                                                                                                                                                            |
| Summarization.Model                 |          |           | Chat model configuration for summarization. The model configuration is identical to [Chat Model Configuration](#-chat-configuration-details). _The configured model may be overridden for individual requests._ |
| Summarization.SummarizationDegree   | int      | 25        | Controls how aggressively content is summarized (0 - 100).                                                                                                                                                      |
| Summarization.Timeout               | TimeSpan | 00:01:00  | Maximum time allowed for a summarization request.                                                                                                                                                               |

### 🚀 Example Usage
#### Resolve the service from DI
```csharp
var summarizationService = serviceProvider.GetService<ISummarizationService>();
```
#### Get metadata
```csharp
var summarizationRequest = new SummarizeMemoryRequest
{
    Question = "What were the main points of the meeting?",
    Answer = "We discussed the quarterly financials, the upcoming project deadlines, and team restructuring.",
    SummarizationDegree = 50
};

var response = await summarizationService
    .SummarizeMemoryAsync(summarizationRequest);

Console.WriteLine($"Question Summarized: {response.QuestionSummarized}");
Console.WriteLine($"Answer Summarized: {response.AnswerSummarized}");
```
<br /><br />

## 🕹️ Agents Service
Coming...

### ⚙️ Agents Configuration
Example `appsettings.json` snippet showing how to configure `IAgentsService` under the `"Ai"` section.

### 📑 Agents Configuration Details

| Setting                                 | Type  | Default | Description                                                                                                                                                      |
| --------------------------------------- | ----- | ------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Agents`                                |       |         | Agents configuration.                                                                                                                                            |
| `Agents.Model`                          |       |         | Chat model configuration for metadata extraction. The model configuration is identical to [Chat Model Configuration](#-chat-configuration-details). _The configured model may be overridden for individual requests._ |
| `Agents.Plugins`                        |       |         | Options for configuring built-in agents plugins. See The [Plugins](#-plugins-1).                                                                                 |
| `Agents.Plugins.EnableMemoryPlugin`     | bool  | `true`  | Enables or disables the built-in Memory plugin. The [Embedding Memory](#-embedding-memory-service) must be configured for this setting to take effect.           |
| `Agents.Plugins.EnableKnowledgePlugin`  | bool  | `true`  | Enables or disables the built-in Knowledge plugin. The [Embedding Knowledge](#-embedding-knowledge-service) must be configured for this setting to take effect.  |
| `Agents.Plugins.EnableWebSearchPlugin`  | bool  | `true`  | Enables or disables the built-in Web Search plugin. The [Web Search](#-web-search) must be configured for this setting to take effect.                     |

### 🚀 Example Usage
#### Resolve the service from DI
```csharp
var agentsService = serviceProvider.GetService<IAgentsService>();
```
#### Invoke Agents
```csharp
// Coming
```

<br /><br />

## 🗣️ Transcription Service
Coming...

<br /><br />

## 👁️ Vision Service
Coming...

<br /><br />

## 🔌 Plugins
**Plugins** (also called *tools*) are sets of related functions that can be exposed to a chat model.  
They allow the model to integrate with external services or invoke custom functionality dynamically.

> ⚠️ **Reserved Names:**  
> The built-in plugin names — `memory`, `knowledge`, and `web_search` — are **reserved** and must not be reused for custom plugins.

<br />

### 🧩 Built-In Plugins
Several **built-in plugins** are supported and configured under the `Ai.Plugins` section in your `appsettings.json`.  

When invoking a service method that supports plugins, **required and optional context variables** are passed through the request.
If any required context variable is missing, an exception is thrown — ensuring that a plugin is never invoked without essential parameters.  

Although plugins are configured **globally**, each service (such as `Chat` or `Agents`) includes **enablement toggles** for every built-in plugin.  
This allows you to control precisely when each plugin is active. Configuration overrides on individual requests can also be used to enable or disable specific plugins per request.

#### 🧠 Memory
The **Memory Plugin** allows the model to access and interact with vector-based memory.  
It does not require direct configuration — memory is set up via the [Embedding Memory](#-embedding-memory-service) configuration,  
and enabled through either the `Chat` or `Agents` configuration sections.
```json
"Plugins": {
  "Memory": {
  }
}
```

#### 📚 Knowledge
The **Knowledge Plugin** provides access to stored knowledge using embeddings.  
Like the memory plugin, it is configured through the [Embedding Knowledge](#-embedding-knowledge-service) section,  
and enabled via the `Chat` or `Agents` configuration.  
```json
"Plugins": {
  "Knowledge": {
  }
}
```

#### 🌐 Web Search
The **Web Search Plugin** enables models to perform live web searches.  
It supports multiple search providers and offers three functions that vary in the amount of detail returned for search results.  
This plugin can be enabled via either the `Chat` or `Agents` configuration.
```json
"Plugins": {
  "WebSearch": {
    "Provider": "Google",
    "Id": null,
    "ApiKey": null
  }
}
```

| Setting                     | Type              | Default  | Description                                                         |
| --------------------------- | ----------------  | -------- | ------------------------------------------------------------------- |
| Plugins.WebSearch           |                   | null     | Web search plugin. Dafault null, not enabled.                       |
| Plugins.WebSearch.Provider  | WebSearchProvider | `Google` | The provider for the plugin to use when searching the web.          |
| Plugins.WebSearch.Id        | string            | null     | The identifier used for web search. _Only used by some providers_.  |
| Plugins.WebSearch.ApiKey    | string            | null     | The api-key of the web search provider.                             |

The table below shows the supported providers and their required configuration values (`Id`, `ApiKey`):

| Setting   | Google                   | Bing |  
| --------- | ------------------------ | ---- |  
| `Id`      | ✅ (`Search Engine ID`)  | ❌   |  
| `ApiKey`  | ✅                       | ✅   |  

<br />

### 🧰 Custom Plugins
Custom plugins are user-defined integrations that extend the system’s capabilities beyond the built-in plugins.  
They allow you to expose your own functions, APIs, or services to the model, giving it access to entirely new tools at runtime.

Custom plugins are implemented using [Microsoft.SemanticKernel](https://learn.microsoft.com/en-us/semantic-kernel).  
📖 **Learn more about implementing custom plguins:** [Semantic Kernel Plugins (C#)](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/?pivots=programming-language-csharp)

Custom plugins are **not configured globally** in the app configuration.  
Instead, they are **attached to individual requests** at runtime — giving you full control over which plugins are available for specific operations or users.

When adding a custom plugin to a request, the plugin must have a unique name. Also specify the `Type` of the plugin and amke make sure it has been 
added to the `IServiceCollection`, including any dependencies it might rely on. Last, the context variables must be passed in the plugin of the request as well. 

Each custom plugin consists of:
- A **name** — the unique identifier of the plugin.  
- A **type** — The type (class) of the plugin. 
- One or more **key/value argument pairs** — defining the input parameters required by the plugin.

##### Example: request plugin
```csharp
var customPlugin = new CustomPlugin
{
    Name = "myPlugin",
    Type = typeof(MyPluginType),
    Context =
    {
        { "context", new MyPluginContext }
    }
};

```

#### 🧩 Complex Argument Support
Plugins fully support **complex argument types**, including nested objects.  
You can pass an instance of a complex object as a value in the argument list, and use the **same complex type** in your Semantic Kernel function’s parameter.  
The binding between the provided object and the function parameter is handled automatically.

This approach simplifies implementing complex functions that take many parameters — avoiding manual parsing or serialization.

#### ⚠️ Context Variable Isolation
Even if multiple plugins use some of the same context variables,  they must still be added **individually** to each plugin definition.  
This prevents accidental cross-plugin data sharing and ensures that each plugin’s execution context is explicit and predictable.

#### 🧱 Naming Rules
Plugin names may include only **ASCII letters, digits, and underscores**.

<br /><br />

## ⚡ Core Service Concepts
### 📩 Request/Response Pattern
- All services follow a **request/response pattern**, where requests contain input data and optional configuration, and responses return structured results along with metadata such as **elapsed time** and **token usage**.
- Responses may include additional strongly-typed data depending on the service (e.g., additional metadata or summarized content).
- Asynchronous processing is supported throughout to ensure non-blocking operations.  
<br />

### 🧰 Request Configuration Overrides
- While global defaults are configured in `appsettings.json`, certain configuration values can be overridden directly in a request.
- This allows fine-grained control over individual operations without modifying the global configuration.
- Overrides can affect model parameters, timeouts, or other service-specific behavior depending on the request.
<br />

### ⛔ Error Handling
- Errors encountered during request processing (e.g., AI model failures, validation issues, or deserialization errors) are surfaced consistently across all services.
- When an error occurs in the AI model, an **`AiException`** is set on the response containing the error message.
- Developers can check these exceptions and handle failures programmatically and log error details accordingly.
- When an error happens in validation, and exception is thrown.
<br />

### 💰 Token & Performance Tracking
- Every response includes **elapsed execution time** for the request.
- **Token usage** is tracked for input and output operations across all services, including embeddings, metadata extraction, and summarization. E.g. the `ChatResponse` will return tokens used for both the chat request, as well as any tokens used for memory summarization and embedding, as well as blob metadata retrieval. Full token usage transparency.  
- Token and performance tracking helps with **cost monitoring** and provides **reasoning transparency** for automated operations.
<br />

### 🛠️ Extensible Implementations
- All four services are implemented via **interfaces**, allowing developers to provide **custom implementations** if desired.
- Users can omit the default configuration section entirely and inject their own service logic while maintaining the same request/response patterns.
- This design ensures **flexibility** and **extensibility** for advanced or specialized use cases.
<br />

### 💚 Health Checks
- Health-checks can be enabled for all services (models) in confiugration. When enabling and ASP.NET Core health-check middleware is configured in your application, each service will invoke periodic health requests to your models and ensure they are alive. The request simply invokes a prompt "ping", and expect to get one token back for success.
<br />

### 📈 Observability
- All services integrate with the registered `ILoggerFactory`, ensuring that any logging performed by underlying components is consistent with your application's logging configuration and routed through your preferred providers.  
- This integration allows developers to capture logs, metrics, and diagnostic information provided by the underlying services without modifying the library.  
- By leveraging the application's logging infrastructure, you get **centralized monitoring**, **performance tracking**, and **diagnostic insights** across all services.
<br />
<br />

## 💡 Other Highlighted Features
### 🔀 Advanced Text Chunking  
When storing embeddings in a vector store, the quality of retrieval depends heavily on how the original text is chunked.  
This library includes an **advanced text-chunking engine** that goes far beyond simple paragraph or sentence splitting.  

#### Key Features  
- **Paragraph-aware splitting** – Text is first divided into paragraphs to keep logical boundaries intact.  
- **Mixed content handling** – Embedded **JSON** or **XML** blocks are detected and treated as atomic units, preventing them from being broken into invalid fragments.  
- **Smart sentence detection** – Sentences are split carefully, accounting for edge cases like abbreviations (`e.g.`, `U.S.`), decimals (`3.14`), and initials (`J.R.R.`), so chunks don’t split in the wrong places.  
- **Dynamic token-based merging** – Sentences are merged into chunks based on configurable **min/max token thresholds**. This ensures chunks are neither too small (losing context) nor too large (exceeding embedding model limits). Oversized blocks (like large JSON/XML) are preserved as standalone chunks.  
- **Context-aware retrieval** – Neighboring chunks can be retrieved alongside a target chunk, optionally restricted to the same paragraph, providing more coherent context for embeddings and downstream LLM calls.  

#### Benefits  
- Produces **high-quality, semantically coherent chunks** optimized for embeddings.  
- Works reliably with **mixed structured/unstructured content**.  
- Reduces **duplicate or fragmented embeddings**, improving retrieval accuracy.  
- Easy to configure with `minTokensPerChunk` and `maxTokensPerChunk` settings.  
<br /><br />

### 🧹 Context Deduplication
When working with embeddings and vector search, it’s common to retrieve **highly similar or duplicate results**.  
This library includes a **context deduplication engine** that automatically merges or removes near-duplicate results,  
ensuring cleaner and more meaningful responses.  

#### Key Features  
- **Semantic deduplication** – Results with highly similar text (`similarityThreshold`, default `0.90`) are merged into a single entry.  
- **Blob-aware detection** – If results reference the same underlying blob (file, document, etc.), they are automatically deduplicated by hash.  
- **Recency preference** – When duplicates are found, the most recent result is kept while older context is merged into it.  
- **Memory Question/Answer pair collapsing** – Questions and their corresponding answers are recognized and merged together, reducing redundancy.  
- **Configurable thresholds** – Fine-tune the similarity threshold for different use cases (memory recall vs. knowledge retrieval).  

#### Benefits  
- Prevents **duplicate or repetitive answers** in retrieval.  
- Keeps **question/answer pairs clean and consistent**.  
- Improves **retrieval accuracy** by reducing noise in memory and knowledge results.  
- Ensures the **freshest and most relevant context** is always retained.
<br /><br /><br />

## 📎 Appendix
### 🔑 Licensing
Vivet.AI has a dual license model with a community license for noncommercial use: [Polyform Noncommercial 1.0.0](https://polyformproject.org/licenses/noncommercial/1.0.0/). 
With this license Vivet.AI is free to use for personal/noncommercial use, A Commercial licenses, which includes support, is required for commercial use 
and can be purchased by sending a request to **licensing@vivetonline.com**  

You can read the full [Vivet.AI License](https://raw.githubusercontent.com/vivet-software/Vivet.AI/refs/heads/master/LICENSE) here.  
For guidance on setting up and using a commercial license, see [Licensing](#-licensing).
<br /><br />

### ⚙️ Appsettings
Most settings have sensible defaults that work out of the box.  
For minimal configuration, you only need to provide **Endpoint**, **API Key**, a **vector store**, and the **model names** to use.  

#### Minimal Configuration without default values
```json
{
  "Ai": {
    "Endpoint": "<your-endpoint>",
    "ApiKey": "<your-apikey>",
    "Chat": {
      "Model": {
        "Name": "<your-chat-model>",
      }
    },
    "Embedding": {
      "Model": {
        "Name": "<your-embedding-model>",
      },
      "Memory": {
        "VectorStore": {
          "Provider": "<your-provider>",
          "ApiKey": "<your-secret>"
        }
      },
      "Knowledge": {
        "VectorStore": {
          "Provider": "<your-provider>",
          "ApiKey": "<your-secret>"
        }
      }
    },
    "Metadata": {
      "Model": {
        "Name": "<your-chat-model>",
      }
    },
    "Summarization": {
      "Model": {
        "Name": "<your-chat-model>",
      }
    },
    "Transcription": {
      "Model": {
        "Name": null
      }
    },
    "Vision": {
      "Model": {
        "Name": null
      }
    },
    "Plugins": {
    }
  }
}
```
#### Full Configuration with default values
```json
{
  "Ai": {
    "Endpoint": null,
    "ApiKey": null,
    "ApiKeyId": null,
    "Chat": {
      "Model": {
        "Name": null,
        "UseHealthCheck": true,
        "Parameters": {
          "MaxOuputTokens": 2048,
          "Temperature": null,
          "StopSequences": [
          ],
          "Seed": null,
          "PresencePenalty": null,
          "FrequencyPenalty": null,
          "RepetitionPenalty": null,
          "TopP": null,
          "TopK": null,
          "ReasoningEffort": null
        }
      },
      "Timeout": "00:01:00",
      "Plugins": {
        "EnableMemoryPlugin": true,
        "EnableKnowledgePlugin": true,
        "EnableWebSearchPlugin": true
      }
    },
    "Embedding": {
      "Model": {
        "Name": null,
        "UseHealthCheck": true
      },
      "VectorSize": 1536,
      "Timeout": "00:01:00",
      "Memory": {
        "Indexing": {
          "UseExtendedMemoryContext": true,
          "UseAutomaticSummarization": false,
          "UseAutomaticMetadataRetrieval": true,
          "TextChunking": {
            "MinTokens": 20,
            "MaxTokens": 60,
            "NeighborContext": {
              "ContextWindow": 1,
              "RestrictToSameParagraph": true
            }
          }
        }
        "Search": {
          "UseQueryDeduplication": true,
          "ContextQueryLimit": 3,
          "CounterpartContextQueryLimit": 3,
          "RetentionInDays": 180,
          "Scoring": {
            "MatchScoreThreashold": 0.86,
            "DeduplicationMatchScoreThreshold": 0.9,
            "RecencyDecayStrategy": "Linear",
            "RecencyBoostMax": 0.1,
            "RecencyDecayDays": 30,
            "RecencySigmoidSteepness": 1.0,
            "ThreadMatchBoost": 0.2
          }
        },
        "VectorStore": {
          "Provider": "None",
          "Host": "localhost",
          "Port": 0,
          "Username": null,
          "ApiKey": null,
          "Timeout": "00:00:30",
          "UseHealthCheck": true
        }
      },
      "Knowledge": {
        "Indexing": {
          "UseAutomaticMetadataRetrieval": true,
          "TextChunking": {
            "MinTokens": 20,
            "MaxTokens": 60,
            "NeighborContext": {
              "ContextWindow": 1,
              "RestrictToSameParagraph": true
            }
          }
        },
        "Search": {
          "UseQueryDeduplication": true,
          "ContextQueryLimit": 3,
          "Scoring": {
            "MatchScoreThreashold": 0.86,
            "DeduplicationMatchScoreThreshold": 0.90,
            "RecencyDecayStrategy": "Linear",
            "RecencyBoostMax": 0.1,
            "RecencyDecayDays": 30,
            "RecencySigmoidSteepness": 1.0
          }
        },
        "VectorStore": {
          "Provider": "None",
          "Host": "localhost",
          "Port": 0,
          "Username": null,
          "ApiKey": null,
          "Timeout": "00:00:30",
          "UseHealthCheck": true
        }
      }
    },
    "Metadata": {
      "Model": {
        "Name": null,
        "UseHealthCheck": true,
        "Parameters": {
          "MaxOuputTokens": 2048,
          "Temperature": null,
          "StopSequences": [
          ],
          "Seed": null,
          "PresencePenalty": null,
          "FrequencyPenalty": null,
          "RepetitionPenalty": null,
          "TopP": null,
          "TopK": null,
          "ReasoningEffort": null
        }
      },
      "SummaryMaxWords": 30,
      "DescriptionMaxWords": 90,
      "Timeout": "00:01:00",
      "Plugins": {
        "CustomPlugins": [
        ]
      }
    },
    "Summarization": {
      "Model": {
        "Name": null,
        "UseHealthCheck": true,
        "Parameters": {
          "MaxOuputTokens": 2048,
          "Temperature": null,
          "StopSequences": [
          ],
          "Seed": null,
          "PresencePenalty": null,
          "FrequencyPenalty": null,
          "RepetitionPenalty": null,
          "TopP": null,
          "TopK": null,
          "ReasoningEffort": null
        }
      },
      "SummarizationDegree": 25,
      "Timeout": "00:01:00",
      "Plugins": {
        "CustomPlugins": [
        ]
      }
    },
    "Agents": {
      "Model": {
        "Name": null,
        "UseHealthCheck": true,
        "Parameters": {
          "MaxOuputTokens": 2048,
          "Temperature": null,
          "StopSequences": [
          ],
          "Seed": null,
          "PresencePenalty": null,
          "FrequencyPenalty": null,
          "RepetitionPenalty": null,
          "TopP": null,
          "TopK": null,
          "ReasoningEffort": null
        }
      },
      "Timeout": "00:01:00",
      "Plugins": {
        "EnableMemoryPlugin": true,
        "EnableKnowledgePlugin": true,
        "EnableWebSearchPlugin": true
      }
    },
    "Transcription": {
      "Model": {
        "Name": null,
        "UseHealthCheck": true
      },
      "IncludeWordGranularity": false,
      "Timeout": "00:01:00"
    },
    "Vision": {
      "Model": {
        "Name": null,
        "UseHealthCheck": true
      },
      "Timeout": "00:01:00"
    },
    "Plugins": {
      "Memory": {
      },
      "Knowledge": {
      },
      "WebSearch": {
        "Provider": "Google",
        "Id": null,
        "ApiKey": null
      }
    }
  }
}
```
