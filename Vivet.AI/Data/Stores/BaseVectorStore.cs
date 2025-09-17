using System;
using Microsoft.Extensions.VectorData;
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
    /// Initializes a new instance of the <see cref="BaseVectorStore{TCollection}"/> class.
    /// </summary>
    /// <param name="vectorStore">The vector store instance to use.</param>
    /// <param name="vectorStoreCollectionDefinition">The collection definition used to configure the vector store collection.</param>
    protected BaseVectorStore(VectorStore vectorStore, VectorStoreCollectionDefinition vectorStoreCollectionDefinition)
    {
        this.Store = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));

        this.Collection = this.Store
            .GetCollection<Guid, TCollection>(typeof(TCollection).Name, vectorStoreCollectionDefinition);
    }
}