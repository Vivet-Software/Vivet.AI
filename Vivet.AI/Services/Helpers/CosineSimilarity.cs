using System;

namespace Vivet.AI.Services.Helpers;

internal static class CosineSimilarity
{
    internal static double GetMatches(float[] vectorA, float[] vectorB)
    {
        if (vectorA == null) 
            throw new ArgumentNullException(nameof(vectorA));
        
        if (vectorB == null) 
            throw new ArgumentNullException(nameof(vectorB));
        
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vectors must be the same length.");
        }

        var dot = 0.0D;
        var magA = 0.0D;
        var magB = 0.0D;

        for (var i = 0; i < vectorA.Length; i++)
        {
            dot += vectorA[i] * vectorB[i];

            magA += Math.Pow(vectorA[i], 2);
            magB += Math.Pow(vectorB[i], 2);
        }

        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }
}