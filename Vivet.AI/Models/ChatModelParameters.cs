using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Amazon;
using Microsoft.SemanticKernel.Connectors.AzureAIInference;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.HuggingFace;
using Microsoft.SemanticKernel.Connectors.Ollama;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Vivet.AI.Models.Enums;

namespace Vivet.AI.Models;

// BUG: what about ToolCallBehavior vs FunctionCallBehavior (https://chatgpt.com/c/68c4059d-4520-8333-b659-d7834d88db5d)

/// <summary>
/// The parameters for the chat model.
/// Not all parameters are supported by all orchestrations, and even if an orchestration supports a parameters it's not garanteed that all available models supports specific parameters.
/// Consult the documentation of the orchestration and the deployed models for information about chat model parameter support.
/// </summary>
public class ChatModelParameters
{
    /// <summary>
    /// The maximum number of output tokens to generate.
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// <item>Ollama</item>
    /// <item>HuggingFace</item>
    /// <item>AzureAIInference</item>
    /// <item>GoogleGemini</item>
    /// <item>Amazon Bedrock
    ///     <list type="bullet">
    ///         <item>Claude</item>
    ///         <item>Cohere Command</item>
    ///         <item>Cohere Command-R</item>
    ///         <item>AI21 Labs Jamba</item>
    ///         <item>AI21 Labs Jurassic</item>
    ///         <item>Mistral</item>
    ///         <item>Titan</item>
    ///         <item>Llama</item>
    ///     </list>
    /// </item>
    /// </list>
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public virtual int MaxOuputTokens { get; set; } = 2048;

    /// <summary>
    /// The sampling temperature to use that controls the apparent creativity of generated completions.
    /// Higher values will make output more random while lower values will make results more focused and deterministic.
    /// It is not recommended to modify temperature and top_p for the same completions request as the interaction of these two settings is difficult to predict.
    /// Supported range is [0, 1].
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// <item>Ollama</item>
    /// <item>HuggingFace</item>
    /// <item>AzureAIInference</item>
    /// <item>GoogleGemini</item>
    /// <item>Amazon Bedrock
    ///     <list type="bullet">
    ///         <item>Claude</item>
    ///         <item>Cohere Command</item>
    ///         <item>Cohere Command-R</item>
    ///         <item>AI21 Labs Jamba</item>
    ///         <item>AI21 Labs Jurassic</item>
    ///         <item>Mistral</item>
    ///         <item>Titan</item>
    ///         <item>Llama</item>
    ///     </list>
    /// </item>
    /// </list>
    /// </summary>
    [Range(0.0F, 100.0F)]
    public virtual float? Temperature { get; set; }

    /// <summary>
    /// A collection of textual sequences that will end completion generation.
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// <item>Ollama</item>
    /// <item>HuggingFace</item>
    /// <item>AzureAIInference</item>
    /// <item>GoogleGemini</item>
    /// <item>Amazon Bedrock
    ///     <list type="bullet">
    ///         <item>Claude</item>
    ///         <item>Cohere Command</item>
    ///         <item>Cohere Command-R</item>
    ///         <item>AI21 Labs Jamba</item>
    ///         <item>AI21 Labs Jurassic</item>
    ///         <item>Mistral</item>
    ///         <item>Titan</item>
    ///     </list>
    /// </item>
    /// </list>
    /// </summary>
    [Required]
    public virtual List<string> StopSequences { get; set; } = [];

    /// <summary>
    /// If specified, the system will make a best effort to sample deterministically such that repeated requests with the same seed and parameters should return the same result.
    /// Determinism is not guaranteed.
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// <item>HuggingFace</item>
    /// <item>AzureAIInference</item>
    /// </list>
    /// </summary>
    public virtual int? Seed { get; set; }

    /// <summary>
    /// A value that influences the probability of generated tokens appearing based on their existing presence in generated text.
    /// Positive values will make tokens less likely to appear when they already exist and increase the model's likelihood to output new topics.
    /// Supported range is [-2, 2].
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// <item>HuggingFace</item>
    /// <item>AzureAIInference</item>
    /// <item>Amazon Bedrock
    ///     <list type="bullet">
    ///         <item>Cohere Command-R</item>
    ///         <item>AI21 Labs Jamba</item>
    ///     </list>
    /// </item>
    /// </list>
    /// </summary>
    [Range(-2F, 2F)]
    public virtual float? PresencePenalty { get; set; }

    /// <summary>
    /// Frequency penalty.
    /// A value that influences the probability of generated tokens appearing based on their cumulative frequency in generated text.
    /// Positive values will make tokens less likely to appear as their frequency increases and decrease the likelihood of the model repeating the same statements verbatim.
    /// Supported range is [-2, 2].
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// <item>AzureAIInference</item>
    /// <item>Amazon Bedrock
    ///     <list type="bullet">
    ///         <item>Cohere Command-R</item>
    ///         <item>AI21 Labs Jamba</item>
    ///     </list>
    /// </item>
    /// </list>
    /// </summary>
    [Range(-2F, 2F)]
    public virtual float? FrequencyPenalty { get; set; }

    /// <summary>
    /// Repetition penalty.
    /// The more a token is used within generation the more it is penalized to not be picked in successive generation passes.
    /// Positive values penalize new tokens based on their existing frequency in the text so far,
    /// decreasing the model's likelihood to repeat the same line verbatim.
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>HuggingFace</item>
    /// </list>
    /// </summary>
    [Range(0.0F, 100.0F)]
    public virtual float? RepetitionPenalty { get; set; }

    /// <summary>
    /// An alternative to sampling with temperature called nucleus sampling (Top-P).
    /// This value causes the model to consider the results of tokens with the provided probability mass.
    /// As an example, a value of 0.15 will cause only the tokens comprising the top 15% of probability mass to be considered.
    /// It is not recommended to modify temperature and top_p for the same completions request as the interaction of these two settings is difficult to predict.
    /// Supported range is [0, 1].
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// <item>Ollama</item>
    /// <item>HuggingFace</item>
    /// <item>AzureAIInference</item>
    /// <item>GoogleGemini</item>
    /// <item>Amazon Bedrock
    ///     <list type="bullet">
    ///         <item>Claude</item>
    ///         <item>Cohere Command</item>
    ///         <item>Cohere Command-R</item>
    ///         <item>AI21 Labs Jamba</item>
    ///         <item>AI21 Labs Jurassic</item>
    ///         <item>Mistral</item>
    ///         <item>Titan</item>
    ///         <item>Llama</item>
    ///     </list>
    /// </item>
    /// </list>
    /// </summary>
    [Range(0.0F, 1.0F)]
    public virtual float? TopP { get; set; }

    /// <summary>
    /// Reduces the probability of generating nonsense. A higher value (e.g. 100) will give more diverse answers,
    /// while a lower value (e.g. 10) will be more conservative. Be aware that this setting is not supported by all orchestrations and/or models.
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>Ollama</item>
    /// <item>HuggingFace</item>
    /// <item>GoogleGemini</item>
    /// <item>Amazon Bedrock
    ///     <list type="bullet">
    ///         <item>Claude</item>
    ///         <item>Cohere Command</item>
    ///         <item>Cohere Command-R</item>
    ///         <item>Mistral</item>
    ///     </list>
    /// </item>
    /// </list>
    /// </summary>
    public virtual int? TopK { get; set; }

    /// <summary>
    /// Gets or sets an object specifying the effort level for the model to use when generating the completion.
    /// Reducing reasoning effort can result in faster responses and fewer tokens used on reasoning in a response.
    /// Be aware, that this setting is not supported by all orchestrations and/or models.
    /// <para>Supported by:</para>
    /// <list type="bullet">
    /// <item>AzureOpenAI</item>
    /// </list>
    /// </summary>
    public ReasoningEffort? ReasoningEffort { get; set; }

    internal PromptExecutionSettings GetPromptExecutionSettings<T>()
        where T : PromptExecutionSettings, new()
    {
        return this.GetPromptExecutionSettings(typeof(T));
    }

    internal PromptExecutionSettings GetPromptExecutionSettings(Type type)
    {
        if (type == null) 
            throw new ArgumentNullException(nameof(type));

        return type switch
        {
            not null when type == typeof(OpenAIPromptExecutionSettings) => this.GetOpenAiPromptExecutionSettings(),
            not null when type == typeof(AzureOpenAIPromptExecutionSettings) => this.GetAzureOpenAiPromptExecutionSettings(),
            not null when type == typeof(AzureAIInferencePromptExecutionSettings) => this.GetAzureAiInferencePromptExecutionSettings(),
            not null when type == typeof(OllamaPromptExecutionSettings) => this.GetOllamaPromptExecutionSettings(),
            not null when type == typeof(HuggingFacePromptExecutionSettings) => this.GetHuggingFacePromptExecutionSettings(),
            not null when type == typeof(GeminiPromptExecutionSettings) => this.GetGoogleGeminiPromptExecutionSettings(),
            not null when type == typeof(AmazonClaudeExecutionSettings) => this.GetAmazonClaudeExecutionSettings(),
            not null when type == typeof(AmazonCommandExecutionSettings) => this.GetAmazonCommandExecutionSettings(),
            not null when type == typeof(AmazonCommandRExecutionSettings) => this.GetAmazonCommandRExecutionSettings(),
            not null when type == typeof(AmazonJambaExecutionSettings) => this.GetAmazonJambaExecutionSettings(),
            not null when type == typeof(AmazonMistralExecutionSettings) => this.GetAmazonMistralExecutionSettings(),
            not null when type == typeof(AmazonTitanExecutionSettings) => this.GetAmazonTitanExecutionSettings(),
            not null when type == typeof(AmazonJurassicExecutionSettings) => this.GetAmazonJurassicExecutionSettings(),
            not null when type == typeof(AmazonLlama3ExecutionSettings) => this.GetAmazonLlama3ExecutionSettings(),
            _ => throw new ArgumentOutOfRangeException(type.Name, $"Unsupported execution settings type: {type.Name}")
        };
    }

    internal static PromptExecutionSettings GetHealthPromptExecutionSettings<T>()
        where T : PromptExecutionSettings, new()
    {
        var promptExecutionSettings = new T();

        switch (promptExecutionSettings)
        {
            case AzureOpenAIPromptExecutionSettings azureOpenAiPromptExecutionSettings:
                azureOpenAiPromptExecutionSettings.MaxTokens = 1;
                azureOpenAiPromptExecutionSettings.Temperature = 0;

                return azureOpenAiPromptExecutionSettings;

            case OpenAIPromptExecutionSettings openAiPromptExecutionSettings:
                openAiPromptExecutionSettings.MaxTokens = 1;
                openAiPromptExecutionSettings.Temperature = 0;

                return openAiPromptExecutionSettings;

            case AzureAIInferencePromptExecutionSettings azureAiInferencePromptExecutionSettings:
                azureAiInferencePromptExecutionSettings.MaxTokens = 1;
                azureAiInferencePromptExecutionSettings.Temperature = 0;

                return azureAiInferencePromptExecutionSettings;

            case OllamaPromptExecutionSettings ollamaPromptExecutionSettings:
                ollamaPromptExecutionSettings.NumPredict = 1;
                ollamaPromptExecutionSettings.Temperature = 0;

                return ollamaPromptExecutionSettings;

            case HuggingFacePromptExecutionSettings huggingFacePromptExecutionSettings:
                huggingFacePromptExecutionSettings.MaxTokens = 1;
                huggingFacePromptExecutionSettings.Temperature = 0;

                return huggingFacePromptExecutionSettings;

            case GeminiPromptExecutionSettings geminiFacePromptExecutionSettings:
                geminiFacePromptExecutionSettings.MaxTokens = 1;
                geminiFacePromptExecutionSettings.Temperature = 0;

                return geminiFacePromptExecutionSettings;

            case AmazonClaudeExecutionSettings amazonClaudeExecutionSettings:
                amazonClaudeExecutionSettings.MaxTokensToSample = 1;
                amazonClaudeExecutionSettings.Temperature = 0;

                return amazonClaudeExecutionSettings;

            case AmazonCommandExecutionSettings amazonCommandExecutionSettings:
                amazonCommandExecutionSettings.MaxTokens = 1;
                amazonCommandExecutionSettings.Temperature = 0;

                return amazonCommandExecutionSettings;

            case AmazonCommandRExecutionSettings amazonCommandRExecutionSettings:
                amazonCommandRExecutionSettings.MaxTokens = 1;
                amazonCommandRExecutionSettings.Temperature = 0;

                return amazonCommandRExecutionSettings;

            case AmazonJambaExecutionSettings amazonJambaExecutionSettings:
                amazonJambaExecutionSettings.MaxTokens = 1;
                amazonJambaExecutionSettings.Temperature = 0;

                return amazonJambaExecutionSettings;

            case AmazonMistralExecutionSettings amazonMistralExecutionSettings:
                amazonMistralExecutionSettings.MaxTokens = 1;
                amazonMistralExecutionSettings.Temperature = 0;

                return amazonMistralExecutionSettings;

            case AmazonTitanExecutionSettings amazonTitanExecutionSettings:
                amazonTitanExecutionSettings.MaxTokenCount = 1;
                amazonTitanExecutionSettings.Temperature = 0;

                return amazonTitanExecutionSettings;

            case AmazonJurassicExecutionSettings amazonJurassicExecutionSettings:
                amazonJurassicExecutionSettings.MaxTokens = 1;
                amazonJurassicExecutionSettings.Temperature = 0;

                return amazonJurassicExecutionSettings;
            
            case AmazonLlama3ExecutionSettings amazonLlama3ExecutionSettings:
                amazonLlama3ExecutionSettings.MaxGenLen = 1;
                amazonLlama3ExecutionSettings.Temperature = 0;

                return amazonLlama3ExecutionSettings;

            default:
                throw new ArgumentOutOfRangeException(nameof(promptExecutionSettings));
        }
    }


    private AzureOpenAIPromptExecutionSettings GetAzureOpenAiPromptExecutionSettings()
    {
        return new AzureOpenAIPromptExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature,
            TopP = this.TopP,
            PresencePenalty = this.PresencePenalty,
            FrequencyPenalty = this.FrequencyPenalty,
            MaxTokens = this.MaxOuputTokens,
            StopSequences = this.StopSequences,
            Seed = this.Seed,
            ResponseFormat = "text",
            TokenSelectionBiases = null,
            // BUG: ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            User = null,
            ChatSystemPrompt = null,
            ChatDeveloperPrompt = null,
            Logprobs = null,
            TopLogprobs = null,
            Store = false,
            Metadata = null,
            ReasoningEffort = this.ReasoningEffort?.ToString().ToLower(),
            WebSearchOptions = null,
            Modalities = "text",
            Audio = null
        };
    }
    private OpenAIPromptExecutionSettings GetOpenAiPromptExecutionSettings()
    {
        return new OpenAIPromptExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature,
            TopP = this.TopP,
            PresencePenalty = this.PresencePenalty,
            FrequencyPenalty = this.FrequencyPenalty,
            MaxTokens = this.MaxOuputTokens,
            StopSequences = this.StopSequences,
            Seed = this.Seed,
            ResponseFormat = "text",
            TokenSelectionBiases = null,
            // BUG: ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            User = null,
            ChatSystemPrompt = null,
            ChatDeveloperPrompt = null,
            Logprobs = null,
            TopLogprobs = null,
            Store = false,
            Metadata = null,
            ReasoningEffort = this.ReasoningEffort?.ToString().ToLower(),
            WebSearchOptions = null,
            Modalities = "text",
            Audio = null
        };
    }
    private OllamaPromptExecutionSettings GetOllamaPromptExecutionSettings()
    {
        return new OllamaPromptExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Stop = this.StopSequences.ToList(),
            Temperature = this.Temperature,
            TopP = this.TopP,
            TopK = this.TopK,
            NumPredict = this.MaxOuputTokens
        };
    }
    private HuggingFacePromptExecutionSettings GetHuggingFacePromptExecutionSettings()
    {
        return new HuggingFacePromptExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature ?? 1.0F,
            TopP = this.TopP,
            TopK = this.TopK,
            MaxTokens = this.MaxOuputTokens,
            MaxNewTokens = null,
            MaxTime = null,
            RepetitionPenalty = this.RepetitionPenalty,
            UseCache = true,
            WaitForModel = false,
            ResultsPerPrompt = 1,
            PresencePenalty = this.PresencePenalty,
            LogProbs = null,
            Seed = this.Seed,
            Stop = this.StopSequences.ToList(),
            TopLogProbs = null,
            ReturnFullText = true,
            DoSample = null,
            Details = true
        };
    }
    private AzureAIInferencePromptExecutionSettings GetAzureAiInferencePromptExecutionSettings()
    {
        return new AzureAIInferencePromptExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            ExtraParameters = null,
            FrequencyPenalty = this.FrequencyPenalty,
            PresencePenalty = this.PresencePenalty,
            Temperature = this.Temperature,
            NucleusSamplingFactor = this.TopP,
            MaxTokens = this.MaxOuputTokens,
            ResponseFormat = "text",
            StopSequences = this.StopSequences,
            Tools = null,
            Seed = this.Seed
        };
    }
    private GeminiPromptExecutionSettings GetGoogleGeminiPromptExecutionSettings()
    {
        return new GeminiPromptExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature,
            TopP = this.TopP,
            TopK = this.TopK,
            MaxTokens = this.MaxOuputTokens,
            CandidateCount = 1,
            StopSequences = this.StopSequences,
            SafetySettings = null,
            // BUG: ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
            AudioTimestamp = null,
            ResponseMimeType = "text/plain",
            ResponseSchema = null,
            ThinkingConfig = null
        };
    }
    private AmazonClaudeExecutionSettings GetAmazonClaudeExecutionSettings()
    {
        return new AmazonClaudeExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            MaxTokensToSample = this.MaxOuputTokens,
            StopSequences = this.StopSequences,
            Temperature = this.Temperature,
            TopK = this.TopK,
            TopP = this.TopP
        };
    }
    private AmazonCommandExecutionSettings GetAmazonCommandExecutionSettings()
    {
        return new AmazonCommandExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature,
            TopK = this.TopK,
            TopP = this.TopP,
            MaxTokens = this.MaxOuputTokens,
            StopSequences = this.StopSequences,
            ReturnLikelihoods = null,
            Stream = null,
            NumGenerations = null,
            LogitBias = null,
            Truncate = null
        };
    }
    private AmazonCommandRExecutionSettings GetAmazonCommandRExecutionSettings()
    {
        return new AmazonCommandRExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            ChatHistory = null,
            Documents = null,
            SearchQueriesOnly = null,
            Preamble = null,
            MaxTokens = this.MaxOuputTokens,
            Temperature = this.Temperature,
            TopP = this.TopP,
            TopK = this.TopK,
            PromptTruncation = null,
            FrequencyPenalty = this.FrequencyPenalty,
            PresencePenalty = this.PresencePenalty,
            Seed = this.Seed,
            ReturnPrompt = null,
            Tools = null,
            ToolResults = null,
            StopSequences = this.StopSequences,
            RawPrompting = true
        };
    }
    private AmazonJambaExecutionSettings GetAmazonJambaExecutionSettings()
    {
        return new AmazonJambaExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature,
            TopP = this.TopP,
            MaxTokens = this.MaxOuputTokens,
            Stop = this.StopSequences,
            NumberOfResponses = 1,
            FrequencyPenalty = this.FrequencyPenalty,
            PresencePenalty = this.PresencePenalty
        };
    }
    private AmazonJurassicExecutionSettings GetAmazonJurassicExecutionSettings()
    {
        return new AmazonJurassicExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature,
            TopP = this.TopP,
            MaxTokens = this.MaxOuputTokens,
            StopSequences = this.StopSequences,
            CountPenalty = null,
            PresencePenalty = null,
            FrequencyPenalty = null
        };
    }
    private AmazonMistralExecutionSettings GetAmazonMistralExecutionSettings()
    {
        return new AmazonMistralExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            MaxTokens = this.MaxOuputTokens,
            StopSequences = this.StopSequences,
            Temperature = this.Temperature,
            TopP = this.TopP,
            TopK = this.TopK
        };
    }
    private AmazonTitanExecutionSettings GetAmazonTitanExecutionSettings()
    {
        return new AmazonTitanExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            TopP = this.TopP,
            Temperature = this.Temperature,
            MaxTokenCount = this.MaxOuputTokens,
            StopSequences = this.StopSequences
        };
    }
    private AmazonLlama3ExecutionSettings GetAmazonLlama3ExecutionSettings()
    {
        return new AmazonLlama3ExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = this.Temperature,
            TopP = this.TopP,
            MaxGenLen = this.MaxOuputTokens
        };
    }
}