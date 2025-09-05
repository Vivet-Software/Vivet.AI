using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;

namespace Tests.Vivet.AI.Services.Extensions;

[TestClass]
public class StreamExtensionsTests
{
    // Helper class for non-seekable stream
    private sealed class NonSeekableMemoryStream(byte[] buffer) : MemoryStream(buffer)
    {
        public override bool CanSeek => false;
    }

    [TestMethod]
    public void ToBase64Test()
    {
        const string TEXT = "Hello, World!";
        var bytes = Encoding.UTF8.GetBytes(TEXT);

        using var stream = new MemoryStream(bytes);
        var result = stream.ToBase64();

        var decoded = Convert.FromBase64String(result);
        var decodedText = Encoding.UTF8.GetString(decoded);

        Assert.AreEqual(TEXT, decodedText);
    }

    [TestMethod]
    public void ToBase64ThrowsArgumentNullExceptionTest()
    {
        Stream stream = null;
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(stream.ToBase64);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void ToBase64WhenEmptyStreamTest()
    {
        using var stream = new MemoryStream();
        var result = stream.ToBase64();

        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, Convert.FromBase64String(result).Length == 0 ? string.Empty : result); // empty decoded array
    }

    [TestMethod]
    public void ToBase64WhenStreamPositionResetTest()
    {
        const string TEXT = "12345";
        var bytes = Encoding.UTF8.GetBytes(TEXT);

        using var stream = new MemoryStream(bytes);
        stream.Position = 2; // move position forward

        var result = stream.ToBase64(); // should reset to 0 automatically

        var decodedText = Encoding.UTF8.GetString(Convert.FromBase64String(result));
        Assert.AreEqual(TEXT, decodedText);
    }

    [TestMethod]
    public void ToBase64WhenNonSeekableStreamTest()
    {
        const string TEXT = "NonSeekable";
        var bytes = Encoding.UTF8.GetBytes(TEXT);

        using var stream = new NonSeekableMemoryStream(bytes);

        var result = stream.ToBase64();
        var decodedText = Encoding.UTF8.GetString(Convert.FromBase64String(result));

        Assert.AreEqual(TEXT, decodedText);
    }
}