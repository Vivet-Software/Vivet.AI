namespace Vivet.AI.Config;

/// <summary>
/// AI Options.
/// </summary>
public class AiOptions
{
    /// <summary>
    /// Section Name.
    /// </summary>
    internal static string SectionName => "Ai";

    /// <summary>
    /// The Endpoint of the AI provider.
    /// <list type="bullet">
    /// <item><description>OpenAI: Required</description></item>
    /// <item><description>Azure OpenAI: Required</description></item>
    /// <item><description>Azure InferenceAI: Required</description></item>
    /// <item><description>HuggingFace: Required</description></item>
    /// <item><description>Ollama: Required</description></item>
    /// <item><description>Google Gemini: Not Required</description></item>
    /// <item><description>Amazon Bedrock: Required (AWS Region Identifier, e.g. "USWest1")</description></item>
    /// </list>
    /// </summary>
    public virtual string Endpoint { get; set; }

    /// <summary>
    /// The API key of the AI provider.  
    /// Can be <c>null</c> if none is required.  
    /// Not all orchestration providers require an API key.
    /// <list type="bullet">
    /// <item><description>OpenAI: Required</description></item>
    /// <item><description>Azure OpenAI: Required</description></item>
    /// <item><description>Azure InferenceAI: Required</description></item>
    /// <item><description>HuggingFace: Required</description></item>
    /// <item><description>Ollama: Not Required</description></item>
    /// <item><description>Google Gemini: Required</description></item>
    /// <item><description>Amazon Bedrock: Required</description></item>
    /// </list>
    /// </summary>
    public virtual string ApiKey { get; set; }

    /// <summary>
    /// The secondary API credential or key identifier, depending on the provider.
    /// <list type="bullet">
    /// <item><description>OpenAI: Not Required</description></item>
    /// <item><description>Azure OpenAI: Not Required</description></item>
    /// <item><description>Azure InferenceAI: Not Required</description></item>
    /// <item><description>HuggingFace: Not Required</description></item>
    /// <item><description>Ollama: Not Required</description></item>
    /// <item><description>Google Gemini: Not Required</description></item>
    /// <item><description>Amazon Bedrock: Required</description></item>
    /// </list>
    /// </summary>
    public virtual string ApiKeyId { get; set; }

    /// <summary>
    /// Chat.
    /// </summary>
    public virtual ChatOptions Chat { get; set; }

    /// <summary>
    /// Embedding.
    /// </summary>
    public virtual EmbeddingOptions Embedding { get; set; }

    /// <summary>
    /// Metadata.
    /// </summary>
    public virtual MetadataOptions Metadata { get; set; }

    /// <summary>
    /// Summarization.
    /// </summary>
    public virtual SummarizationOptions Summarization { get; set; }

    /// <summary>
    /// Agents.
    /// </summary>
    public virtual AgentsOptions Agents { get; set; }
}