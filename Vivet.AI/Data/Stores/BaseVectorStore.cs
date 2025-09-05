using System;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Data.Stores;

/// <summary>
/// Provides a base abstraction for working with a vector store, including access to collections and text search.
/// </summary>
/// <typeparam name="TCollection">The embedding type used in the collection.</typeparam>
public abstract class BaseVectorStore<TCollection>
    where TCollection : BaseEmbedding
{
    /// <summary>
    /// Gets the underlying vector store instance.
    /// </summary>
    public VectorStore Store { get; }

    /// <summary>
    /// Gets the vector store collection for the specified <typeparamref name="TCollection"/>.
    /// </summary>
    public VectorStoreCollection<Guid, TCollection> Collection { get; }

    /// <summary>
    /// Gets the text search interface for the vector store.
    /// </summary>
    public VectorStoreTextSearch<TCollection> TextSearch { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseVectorStore{TCollection}"/> class.
    /// </summary>
    /// <param name="vectorStore">The vector store instance to use.</param>
    /// <param name="vectorStoreTextSearch">The text search component associated with the vector store.</param>
    /// <param name="vectorStoreCollectionDefinition">The collection definition used to configure the vector store collection.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="vectorStore"/> or <paramref name="vectorStoreTextSearch"/> is <c>null</c>.
    /// </exception>
    protected BaseVectorStore(VectorStore vectorStore, VectorStoreTextSearch<TCollection> vectorStoreTextSearch, VectorStoreCollectionDefinition vectorStoreCollectionDefinition)
    {
        this.Store = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        this.TextSearch = vectorStoreTextSearch ?? throw new ArgumentNullException(nameof(vectorStoreTextSearch));

        this.Collection = this.Store
            .GetCollection<Guid, TCollection>(typeof(TCollection).Name, vectorStoreCollectionDefinition);
    }
}