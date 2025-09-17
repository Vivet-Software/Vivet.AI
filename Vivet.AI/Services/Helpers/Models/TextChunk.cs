namespace Vivet.AI.Services.Helpers.Models;

internal class TextChunk
{
    internal virtual string Text { get; set; }

    internal virtual int TokenCount { get; set; }

    internal virtual int ParagraphId { get; set; }
}