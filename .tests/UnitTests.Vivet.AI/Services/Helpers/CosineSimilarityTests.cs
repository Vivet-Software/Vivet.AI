using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Helpers;

namespace UnitTests.Vivet.AI.Services.Helpers;

[TestClass]
public class CosineSimilarityTests
{
    [TestMethod]
    public void GetMatchesWhenIdenticalVectorsTest()
    {
        var vectorA = new float[] { 1, 2, 3 };
        var vectorB = new float[] { 1, 2, 3 };

        var result = CosineSimilarity.GetMatches(vectorA, vectorB);
        Assert.AreEqual(1.0, result, 1e-9, "Cosine similarity of identical vectors should be 1.");
    }

    [TestMethod]
    public void GetMatchesWhenOrthogonalVectorsTest()
    {
        var vectorA = new float[] { 1, 0 };
        var vectorB = new float[] { 0, 1 };

        var result = CosineSimilarity.GetMatches(vectorA, vectorB);
        Assert.AreEqual(0.0, result, 1e-9, "Cosine similarity of orthogonal vectors should be 0.");
    }

    [TestMethod]
    public void GetMatchesWhenOppositeVectorsTest()
    {
        var vectorA = new float[] { 1, 0 };
        var vectorB = new float[] { -1, 0 };

        var result = CosineSimilarity.GetMatches(vectorA, vectorB);
        Assert.AreEqual(-1.0, result, 1e-9, "Cosine similarity of opposite vectors should be -1.");
    }

    [TestMethod]
    public void GetMatchesWhenDifferentLengthVectorsTest()
    {
        var vectorA = new float[] { 1, 2 };
        var vectorB = new float[] { 1, 2, 3 };

        Assert.ThrowsException<ArgumentException>(() => CosineSimilarity.GetMatches(vectorA, vectorB));
    }

    [TestMethod]
    public void GetMatchesWhenVectorANullTest()
    {
        float[] vectorA = null;
        var vectorB = new float[] { 1, 2, 3 };

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => CosineSimilarity.GetMatches(vectorA, vectorB));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetMatchesWhenVectorBNullTest()
    {
        var vectorA = new float[] { 1, 2, 3 };
        float[] vectorB = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => CosineSimilarity.GetMatches(vectorA, vectorB));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetMatchesWhenSimilarVectorsTest()
    {
        var vectorA = new float[] { 1, 2 };
        var vectorB = new float[] { 2, 4 };

        var result = CosineSimilarity.GetMatches(vectorA, vectorB);
        Assert.AreEqual(1.0, result, 1e-9, "Vectors that are scalar multiples should have cosine similarity 1.");
    }
}