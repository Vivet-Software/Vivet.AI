using Vivet.AI.Services.Interfaces;

namespace Vivet.AI.Services;

// TODO: Image To Text: IImageToTextService, Only pre-implemented for HuggingFace. Needs registration in ServiceCollection / Kernel. 
// TODO: Audio To Text: IAudioToTextService, No pre-built integrations
// TODO: Video To Text: AudioToText (Whisper, Azure Speech) + Frame extraction (ImageToText ) + Temporal metadata (combine with timestamps) 

// TODO: Text Analysis (Analyze Sentiment, Extract Key Phrases, Recognize Named Entities, Recognize / Redact PII Entities, Recognize Linked Entities, Detect Language, 
// - https://learn.microsoft.com/en-us/azure/ai-services/language-service/overview
// - https://github.com/Azure/azure-sdk-for-net/blob/Azure.AI.TextAnalytics_5.3.0/sdk/textanalytics/Azure.AI.TextAnalytics/README.md ("Run multiple actions Asynchronously". That's important we mirror that - at least look into it)

// TODO: Translation (Text, Document?)
// - https://learn.microsoft.com/en-us/azure/ai-services/translator/overview
// - https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/translation

/// <inheritdoc cref="ITextService"/>
public class TextService : ITextService;