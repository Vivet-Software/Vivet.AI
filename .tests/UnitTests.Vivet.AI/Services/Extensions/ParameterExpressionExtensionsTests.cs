using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Data.Models;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Enums;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class ParameterExpressionExtensionsTests
{
    private sealed class TestEmbedding : BaseEmbedding
    {
        public int IntProp { get; set; }
        public List<string> Tags { get; set; }
    }

    [TestMethod]
    public void AddExpressionEqualTest()
    {
        Expression body = null;
        var param = Expression.Parameter(typeof(TestEmbedding), "x");

        param.AddExpressionEqual(nameof(TestEmbedding.IntProp), 5, ref body);

        Assert.IsNotNull(body);
        Assert.AreEqual(ExpressionType.Equal, body.NodeType);
    }

    [TestMethod]
    public void AddExpressionGreaterThanTest()
    {
        Expression body = null;
        var param = Expression.Parameter(typeof(TestEmbedding), "x");

        param.AddExpressionGreaterThan(nameof(TestEmbedding.IntProp), 5, ref body);

        Assert.IsNotNull(body);
        Assert.AreEqual(ExpressionType.GreaterThan, body.NodeType);
    }

    [TestMethod]
    public void AddExpressionContainsTest()
    {
        Expression body = null;
        var param = Expression.Parameter(typeof(TestEmbedding), "x");

        param.AddExpressionContains(nameof(TestEmbedding.Tags), "tag1", ref body);

        Assert.IsNotNull(body);
        Assert.AreEqual(ExpressionType.Call, body.NodeType);
    }

    [TestMethod]
    public void AddDateRangeExpressionWhenFromAtTest()
    {
        Expression body = null;
        var param = Expression.Parameter(typeof(TestEmbedding), "x");

        var range = new DateRange
        {
            From = DateTimeOffset.UtcNow.AddDays(-1)
        };

        param.AddDateRangeExpression(nameof(BaseEmbedding.UnixTimestamp), range, ref body);

        Assert.IsNotNull(body);
        Assert.AreEqual(ExpressionType.GreaterThanOrEqual, body.NodeType);
    }

    [TestMethod]
    public void AddDateRangeExpressionWhenToAtTest()
    {
        Expression body = null;
        var param = Expression.Parameter(typeof(TestEmbedding), "x");

        var range = new DateRange
        {
            To = DateTimeOffset.UtcNow
        };

        param.AddDateRangeExpression(nameof(BaseEmbedding.UnixTimestamp), range, ref body);

        Assert.IsNotNull(body);
        Assert.AreEqual(ExpressionType.LessThanOrEqual, body.NodeType);
    }

    [TestMethod]
    public void AddExpressionSearchForWhenTextTest()
    {
        Expression body = null;
        var param = Expression.Parameter(typeof(Knowledge), "k");

        param.AddExpressionSearchFor(SearchFor.Text, ref body);

        Assert.IsNotNull(body);
        Assert.AreEqual(ExpressionType.AndAlso, body.NodeType);
    }

    [TestMethod]
    public void BuildExpressionTest()
    {
        var param = Expression.Parameter(typeof(TestEmbedding), "x");
        Expression body = Expression.Constant(true);

        var expr = param.BuildExpression<TestEmbedding>(body);

        Assert.IsNotNull(expr);
        Assert.AreEqual(typeof(Func<TestEmbedding, bool>), expr.Type);
    }

    [TestMethod]
    public void AddExpressionSearchForWhenEnumIsOutOfRangeTest()
    {
        Expression body = null;
        var param = Expression.Parameter(typeof(Knowledge), "k");

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => param.AddExpressionSearchFor((SearchFor)999, ref body));
    }

    [TestMethod]
    public void AddExpressionEqualWhenExpressionIsNullTest()
    {
        Expression body = null;

        Assert.ThrowsException<ArgumentNullException>(() => ((ParameterExpression)null).AddExpressionEqual("IntProp", 5, ref body));
    }
}