using Microsoft.Extensions.VectorData;
using System;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Data.Definitions;

internal class EmbeddingVectorStoreCollectionDefinition
{
    internal static VectorStoreCollectionDefinition GetVectorStoreCollectionDefinition(int vectorSize)
    {
        return new VectorStoreCollectionDefinition
        {
            Properties = 
            [
                new VectorStoreVectorProperty(nameof(BaseEmbedding.Vector), typeof(ReadOnlyMemory<float>), vectorSize)
                {
                    DistanceFunction = DistanceFunction.CosineSimilarity,
                    IndexKind = IndexKind.Hnsw
                }
            ]
        };
    }
}