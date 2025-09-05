using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Data.Stores;

/// <summary>
/// Knowledge Vector Store.
/// </summary>
/// <param name="vectorStore">The <see cref="VectorStore"/>.</param>
/// <param name="vectorStoreTextSearch">The <see cref="VectorStoreTextSearch{TRecord}"/>.</param>
/// <param name="vectorStoreCollectionDefinition">The <see cref="VectorStoreCollectionDefinition"/>.</param>
public class KnowledgeVectorStore(VectorStore vectorStore, VectorStoreTextSearch<Knowledge> vectorStoreTextSearch, VectorStoreCollectionDefinition vectorStoreCollectionDefinition) 
    : BaseVectorStore<Knowledge>(vectorStore, vectorStoreTextSearch, vectorStoreCollectionDefinition);