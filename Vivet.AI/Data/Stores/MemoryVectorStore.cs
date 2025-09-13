using Microsoft.Extensions.VectorData;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Data.Stores;

/// <summary>
/// Memory Vector Store.
/// </summary>
/// <param name="vectorStore">The <see cref="VectorStore"/>.</param>
/// <param name="vectorStoreCollectionDefinition">The <see cref="VectorStoreCollectionDefinition"/>.</param>
public class MemoryVectorStore(VectorStore vectorStore, VectorStoreCollectionDefinition vectorStoreCollectionDefinition) 
    : BaseVectorStore<Memory>(vectorStore, vectorStoreCollectionDefinition);