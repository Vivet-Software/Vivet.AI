using System;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Plugins.TextSearch.Mappers;

// BUG: Memory / Knowledge

/// <summary>
/// String mapper which converts a <see cref="BaseEmbedding"/> to a string.
/// </summary>
public sealed class EmbeddingTextSearchStringMapper : ITextSearchStringMapper
{
    /// <inheritdoc />
    public string MapFromResultToString(object result)
    {
        if (result is BaseEmbedding embedding)
        {
            return embedding.Content;
        }

        throw new ArgumentException("Invalid result type.");
    }
}