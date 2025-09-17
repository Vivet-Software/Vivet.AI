using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Models;

namespace UnitTests.Vivet.AI.Services.Models;

[TestClass]
public class TokenUsageTests
{
    [TestMethod]
    public void TotalTokensTest()
    {
        var usage = new TokenUsage { InputTokens = 100, OutputTokens = 50 };
        Assert.AreEqual(150, usage.TotalTokens);
    }

    [TestMethod]
    public void TotalTokensWhenNullTest()
    {
        var usage = new TokenUsage { InputTokens = null, OutputTokens = 50 };
        Assert.AreEqual(50, usage.TotalTokens);

        usage = new TokenUsage { InputTokens = 25, OutputTokens = null };
        Assert.AreEqual(25, usage.TotalTokens);

        usage = new TokenUsage { InputTokens = null, OutputTokens = null };
        Assert.AreEqual(0, usage.TotalTokens);
    }

    [TestMethod]
    public void InputTokensWhenNegativeValueTest()
    {
        var usage = new TokenUsage { InputTokens = -10, OutputTokens = 5 };
        Assert.AreEqual(0, usage.InputTokens);
        Assert.AreEqual(5, usage.OutputTokens);
    }

    [TestMethod]
    public void OutputTokensWhenNegativevalueTest()
    {
        var usage = new TokenUsage { InputTokens = 5, OutputTokens = -15 };
        Assert.AreEqual(5, usage.InputTokens);
        Assert.AreEqual(0, usage.OutputTokens);
    }

    [TestMethod]
    public void PlusOperator()
    {
        var a = new TokenUsage { InputTokens = 10, OutputTokens = 20 };
        var b = new TokenUsage { InputTokens = 5, OutputTokens = 15 };

        var result = a + b;

        Assert.AreEqual(15, result.InputTokens);
        Assert.AreEqual(35, result.OutputTokens);
    }

    [TestMethod]
    public void PlusOperatorWhenNegativeValuesTest()
    {
        var a = new TokenUsage { InputTokens = -10, OutputTokens = 20 };
        var b = new TokenUsage { InputTokens = 5, OutputTokens = -15 };

        var result = a + b;

        Assert.AreEqual(5, result.InputTokens);
        Assert.AreEqual(20, result.OutputTokens); 
    }

    [TestMethod]
    public void PlusOperatorWhenNull()
    {
        var b = new TokenUsage { InputTokens = 5, OutputTokens = 15 };

        var result = null + b;

        Assert.AreEqual(5, result.InputTokens);
        Assert.AreEqual(15, result.OutputTokens);
    }

    [TestMethod]
    public void MinusOperatorTest()
    {
        var a = new TokenUsage { InputTokens = 10, OutputTokens = 20 };
        var b = new TokenUsage { InputTokens = 5, OutputTokens = 15 };

        var result = a - b;

        Assert.AreEqual(5, result.InputTokens);
        Assert.AreEqual(5, result.OutputTokens);
    }

    [TestMethod]
    public void MinusOperatorWhenNegativeValuesTest()
    {
        var a = new TokenUsage { InputTokens = 5, OutputTokens = 10 };
        var b = new TokenUsage { InputTokens = 10, OutputTokens = 15 };

        var result = a - b;

        Assert.AreEqual(0, result.InputTokens);
        Assert.AreEqual(0, result.OutputTokens);
    }

    [TestMethod]
    public void MinusOperatorWhenNull()
    {
        var b = new TokenUsage { InputTokens = 5, OutputTokens = 15 };

        var result = null - b;

        Assert.AreEqual(5, result.InputTokens);
        Assert.AreEqual(15, result.OutputTokens);
    }
}