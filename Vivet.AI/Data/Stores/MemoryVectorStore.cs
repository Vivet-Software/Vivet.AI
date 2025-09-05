using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Data.Stores;

/// <summary>
/// Memory Vector Store.
/// </summary>
/// <param name="vectorStore">The <see cref="VectorStore"/>.</param>
/// <param name="vectorStoreTextSearch">The <see cref="VectorStoreTextSearch{TRecord}"/>.</param>
/// <param name="vectorStoreCollectionDefinition">The <see cref="VectorStoreCollectionDefinition"/>.</param>
public class MemoryVectorStore(VectorStore vectorStore, VectorStoreTextSearch<Memory> vectorStoreTextSearch, VectorStoreCollectionDefinition vectorStoreCollectionDefinition) 
    : BaseVectorStore<Memory>(vectorStore, vectorStoreTextSearch, vectorStoreCollectionDefinition);