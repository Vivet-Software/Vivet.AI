using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.MimeTypes;

namespace UnitTests.Vivet.AI.Services.Extensions;

[TestClass]
public class TypeExtensionsTests
{
    private sealed class DummyMetadata;
    private class TestBlobAdditionalMetadata : BaseBlobAdditionalMetadata<MimeType, DummyMetadata>;
    private sealed class DerivedTestBlobAdditionalMetadata : TestBlobAdditionalMetadata;
    private sealed class NonGenericClass;
    private sealed class SampleObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public NestedObject Nested { get; set; }
        public List<string> Tags { get; set; }
    }
    // ReSharper disable ClassNeverInstantiated.Local
    private sealed class NestedObject;
    // ReSharper restore ClassNeverInstantiated.Local

    [TestMethod]
    public void IsSimpleWhenTypeIsNullTest()
    {
        Type type = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.Throws<ArgumentNullException>(() => type.IsSimple());
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void IsSimpleWhenPrimitiveTypesTest()
    {
        Assert.IsTrue(typeof(int).IsSimple());
        Assert.IsTrue(typeof(bool).IsSimple());
        Assert.IsTrue(typeof(double).IsSimple());
    }

    [TestMethod]
    public void IsSimpleWhenCommonSimpleTypesTest()
    {
        Assert.IsTrue(typeof(string).IsSimple());
        Assert.IsTrue(typeof(Guid).IsSimple());
        Assert.IsTrue(typeof(Guid?).IsSimple());
        Assert.IsTrue(typeof(TimeSpan).IsSimple());
        Assert.IsTrue(typeof(TimeSpan?).IsSimple());
        Assert.IsTrue(typeof(TimeOnly).IsSimple());
        Assert.IsTrue(typeof(TimeOnly?).IsSimple());
        Assert.IsTrue(typeof(DateOnly).IsSimple());
        Assert.IsTrue(typeof(DateOnly?).IsSimple());
        Assert.IsTrue(typeof(DateTime).IsSimple());
        Assert.IsTrue(typeof(DateTime?).IsSimple());
        Assert.IsTrue(typeof(DateTimeOffset).IsSimple());
        Assert.IsTrue(typeof(DateTimeOffset?).IsSimple());
    }

    [TestMethod]
    public void IsSimpleWhenComplexTypeTest()
    {
        Assert.IsFalse(typeof(TypeExtensionsTests).IsSimple());
        Assert.IsFalse(typeof(List<string>).IsSimple());
    }


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


    [TestMethod]
    public void GenerateJsonMapWhenTypeIsNullTest()
    {
        Type type = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.Throws<ArgumentNullException>(type.GenerateJsonMap);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GenerateJsonMapWhenContainsExpectedPropertiesTest()
    {
        var map = typeof(SampleObject).GenerateJsonMap();

        Assert.IsTrue(map.ContainsKey(nameof(SampleObject.Id)));
        Assert.IsTrue(map.ContainsKey(nameof(SampleObject.Name)));
        Assert.IsTrue(map.ContainsKey(nameof(SampleObject.Nested)));
        Assert.IsTrue(map.ContainsKey(nameof(SampleObject.Tags)));
    }

    [TestMethod]
    public void GenerateJsonMapWhenHasCorrectJsonTypesTest()
    {
        var map = typeof(SampleObject).GenerateJsonMap();

        Assert.AreEqual("number", map["Id"]);
        Assert.AreEqual("string", map["Name"]);
        Assert.IsInstanceOfType(map["Nested"], typeof(Dictionary<string, object>));
        Assert.IsInstanceOfType(map["Tags"], typeof(List<object>));
    }
}