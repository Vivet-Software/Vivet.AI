using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.MimeTypes;

namespace Tests.Vivet.AI.Services.Extensions;

[TestClass]
public class TypeExtensionsTests
{
    private sealed class DummyMetadata;
    private class TestBlobAdditionalMetadata : BaseBlobAdditionalMetadata<MimeType, DummyMetadata>;
    private sealed class DerivedTestBlobAdditionalMetadata : TestBlobAdditionalMetadata;
    private sealed class NonGenericClass;

    [TestMethod]
    public void GetMetadataTypeWhenGenericTypeTest()
    {
        var type = typeof(TestBlobAdditionalMetadata);
        var metadataType = type.GetMetadataType();

        Assert.IsNotNull(metadataType);
        Assert.AreEqual(typeof(DummyMetadata), metadataType);
    }

    [TestMethod]
    public void GetMetadataTypeWhenInheritedGenericTypeTest()
    {
        var type = typeof(DerivedTestBlobAdditionalMetadata);
        var metadataType = type.GetMetadataType();

        Assert.IsNotNull(metadataType);
        Assert.AreEqual(typeof(DummyMetadata), metadataType);
    }

    [TestMethod]
    public void GetMetadataTypeWhenNonGenericTypeTest()
    {
        var type = typeof(NonGenericClass);
        var metadataType = type.GetMetadataType();

        Assert.IsNull(metadataType);
    }

    [TestMethod]
    public void GetMetadataTypeWhenObjectTypeTest()
    {
        var type = typeof(object);
        var metadataType = type.GetMetadataType();

        Assert.IsNull(metadataType);
    }

    [TestMethod]
    public void GetMetadataTypeWhenNullThrowsArgumentNullExceptionTest()
    {
        Type type = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(type.GetMetadataType);
        // ReSharper restore ExpressionIsAlwaysNull
    }
}