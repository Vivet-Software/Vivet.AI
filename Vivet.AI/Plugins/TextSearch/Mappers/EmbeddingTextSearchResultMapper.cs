using System;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Plugins.TextSearch.Mappers;

// BUG: Memory / Knowledge

/// <summary>
/// Result mapper which converts a <see cref="BaseEmbedding"/> to a <see cref="TextSearchResult"/>.
/// </summary>
public sealed class EmbeddingTextSearchResultMapper : ITextSearchResultMapper
{
    /// <inheritdoc />
    public TextSearchResult MapFromResultToTextSearchResult(object result)
    {
        if (result is BaseEmbedding embedding)
        {
            return new TextSearchResult(embedding.Content)
            {
                Name = embedding.Id.ToString()
            };
        }

        throw new ArgumentException("Invalid result type.");
    }
}