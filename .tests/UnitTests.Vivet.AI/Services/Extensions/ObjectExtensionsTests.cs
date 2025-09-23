using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class ObjectExtensionsTests
{
    [TestMethod]
    public void ValidateTest()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public void ValidateWhenObjectIsNullThrowsArgumentNullExceptionTest()
    {
        object @object = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(@object.Validate);
        // ReSharper restore ExpressionIsAlwaysNull
    }
}