using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Config;
using Vivet.AI.Config.Enums;
using Vivet.AI.Data.Models;
using Vivet.AI.Services.Extensions;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class BaseEmbeddingExtensionsTests
{
    private sealed class TestEmbedding : BaseEmbedding
    {
        public TestEmbedding(long unixTimestamp)
        {
            this.UnixTimestamp = unixTimestamp;
        }
    }

    private sealed class TestScoringOptions : BaseScoringOptions;
   
    private static TestScoringOptions DefaultOptions(RecencyDecayStrategy strategy) =>
        new()
        {
            RecencyBoostMax = 10.0,
            RecencyDecayDays = 5.0,
            RecencySigmoidSteepness = 1.0,
            RecencyDecayStrategy = strategy
        };

    [TestMethod]
    public void GetRecencyScoreWhenLinearStrategyTest()
    {
        var record = new TestEmbedding(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var options = DefaultOptions(RecencyDecayStrategy.Linear);

        var score = record.GetRecencyScore(options);

        Assert.AreEqual(options.RecencyBoostMax, score, 1e-3);
    }

    [TestMethod]
    public void GetRecencyScoreWhenLinearStrategyAndAgeBeyondDecayTest()
    {
        var recordTime = DateTimeOffset.UtcNow.AddDays(-100).ToUnixTimeSeconds();
        var record = new TestEmbedding(recordTime);
        var options = DefaultOptions(RecencyDecayStrategy.Linear);

        var score = record.GetRecencyScore(options);

        Assert.AreEqual(0.0, score, 1e-6);
    }

    [TestMethod]
    public void GetRecencyScoreWhenExponentialStrategyTest()
    {
        // Arrange: record timestamp = now
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var record = new TestEmbedding(now);
        var options = DefaultOptions(RecencyDecayStrategy.Exponential);

        // Act
        var score = record.GetRecencyScore(options);

        // Assert: ageInDays ~ 0 → exp(0) = 1 → score = RecencyBoostMax
        Assert.AreEqual(options.RecencyBoostMax, score, 1e-3);
    }

    [TestMethod]
    public void GetRecencyScoreWhenSigmoidStrategyTest()
    {
        var recordTime = DateTimeOffset.UtcNow.AddDays(-5).ToUnixTimeSeconds();
        var record = new TestEmbedding(recordTime);
        var options = DefaultOptions(RecencyDecayStrategy.Sigmoid);

        var score = record.GetRecencyScore(options);
        var expected = options.RecencyBoostMax / 2.0;

        Assert.AreEqual(expected, score, 1e-3);
    }

    [TestMethod]
    public void GetRecencyScoreWhenUnsupportedStrategyTest()
    {
        var record = new TestEmbedding(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var options = DefaultOptions((RecencyDecayStrategy)999); // unsupported value

        var score = record.GetRecencyScore(options);

        Assert.AreEqual(0.0, score, 1e-6);
    }

    [TestMethod]
    public void GetRecencyScoreWhenRecordIsNullThrowsArgumentNullExceptionTest()
    {
        TestEmbedding record = null;
        var options = DefaultOptions(RecencyDecayStrategy.Linear);

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => record.GetRecencyScore(options));
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetRecencyScoreWhenOptionsIsNullThrowsArgumentNullExceptionTest()
    {
        var record = new TestEmbedding(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        Assert.ThrowsException<ArgumentNullException>(() => record.GetRecencyScore(null));
    }
}