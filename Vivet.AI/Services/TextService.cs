using Vivet.AI.Services.Interfaces;

namespace Vivet.AI.Services;

// TODO: Image To Text: IImageToTextService, Only pre-implemented for HuggingFace. Needs registration in ServiceCollection / Kernel. 
// TODO: Audio To Text: IAudioToTextService, No pre-built integrations
// TODO: Video To Text: AudioToText (Whisper, Azure Speech) + Frame extraction (ImageToText ) + Temporal metadata (combine with timestamps) 

/// <inheritdoc cref="ITextService"/>
public class TextService : ITextService;