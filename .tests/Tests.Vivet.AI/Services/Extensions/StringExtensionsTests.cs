using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Extensions;

namespace Tests.Vivet.AI.Services.Extensions;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void GetChatResponseAnswerTest()
    {
        const string CONTENT = "<think>ignore this</think> Hello world";

        var result = CONTENT.GetChatResponseAnswer();
        Assert.AreEqual("Hello world", result);
    }

    [TestMethod]
    public void GetChatResponseAnswerWhenNestedTest()
    {
        const string CONTENT = "Hello <think>ignore this</think> world";

        var result = CONTENT.GetChatResponseAnswer();
        Assert.AreEqual("world", result);
    }

    [TestMethod]
    public void GetChatResponseAnswerWhenNullThrowsArgumentNullExceptionTest()
    {
        string content = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(content.GetChatResponseAnswer);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetChatResponseAnswerWhenOrphanedClosingTagTest()
    {
        const string CONTENT = "</think>Hello world";

        var result = CONTENT.GetChatResponseAnswer();
        Assert.AreEqual("Hello world", result);
    }

    [TestMethod]
    public void GetChatResponseAnswerWhenNoThinkTagsTest()
    {
        const string CONTENT = "Just some text";

        var result = CONTENT.GetChatResponseAnswer();
        Assert.AreEqual("Just some text", result);
    }


    [TestMethod]
    public void GetChatResponseThinkingTest()
    {
        const string CONTENT = "<think>test</think>";

        var result = CONTENT.GetChatResponseThinking();
        Assert.AreEqual("test", result);
    }

    [TestMethod]
    public void GetChatResponseThinkingWhenNullThrowsArgumentNullExceptionTest()
    {
        string content = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(content.GetChatResponseThinking);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [TestMethod]
    public void GetChatResponseThinkingWhenEmptyThinkTest()
    {
        const string CONTENT = "<think></think>";

        var result = CONTENT.GetChatResponseThinking();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetChatResponseThinkingWhenNoThinkTagsTest()
    {
        const string CONTENT = "No thinking here";

        var result = CONTENT.GetChatResponseThinking();
        Assert.IsNull(result);
    }


    [TestMethod]
    public void GetContentHashTest()
    {
        const string TEXT = "Hello";

        var hash = TEXT.GetContentHash();
        Assert.IsFalse(string.IsNullOrEmpty(hash));
        Assert.AreEqual(hash.Length, hash.Length); // SHA256 hex length
    }

    [TestMethod]
    public void GetContentHashWhenNullThrowsArgumentNullExceptionTest()
    {
        string text = null;

        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(text.GetContentHash);
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void GetUtf8ByteCountTest()
    {
        const string TEXT = "Hello";
        Assert.AreEqual(TEXT.Length, TEXT.GetUtf8ByteCount());

        const string UNICODE = "こんにちは"; // 15 bytes in UTF8
        Assert.AreEqual(System.Text.Encoding.UTF8.GetByteCount(UNICODE), UNICODE.GetUtf8ByteCount());
    }

    [TestMethod]
    public void GetUtf8ByteCountWhenNullThrowsArgumentNullException()
    {
        string text = null;
 
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.ThrowsException<ArgumentNullException>(() => text.GetUtf8ByteCount());
        // ReSharper restore ExpressionIsAlwaysNull
    }


    [TestMethod]
    public void AreSimilarWhenValueIsNullThrowsArgumentNullExceptionTest()
    {
        string value = null;

        // ReSharper disable ExpressionIsAlwaysNull
        var ex = Assert.ThrowsException<ArgumentNullException>(() => value.AreSimilar("test"));
        // ReSharper restore ExpressionIsAlwaysNull
        Assert.AreEqual("value", ex.ParamName);
    }

    [TestMethod]
    public void AreSimilarWhenOtherIsNullTest()
    {
        const string VALUE = "test";
        var result = VALUE.AreSimilar();
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AreSimilarWhenValueUsEmptyTest()
    {
        const string VALUE = "";
        var result = VALUE.AreSimilar("something");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AreSimilarWhenOtherIsEmptyTest()
    {
        const string VALUE = "something";
        var result = VALUE.AreSimilar("");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AreSimilarWhenExactMatchTest()
    {
        const string VALUE = "Hello";
        const string OTHER = "Hello";
        var result = VALUE.AreSimilar(OTHER);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AreSimilarWhenCaseAndWhitespaceDifferenceTest()
    {
        const string VALUE = "  Hello ";
        const string OTHER = "hello";
        var result = VALUE.AreSimilar(OTHER);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AreSimilarWhenFuzzyMatchAboveThresholdTest()
    {
        const string VALUE = "hello";
        const string OTHER = "helo"; // missing one 'l'
        var result = VALUE.AreSimilar(OTHER, 0.8); // lower threshold
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AreSimilarWhenFuzzyMatchBelowThresholdTest()
    {
        const string VALUE = "hello";
        const string OTHER = "hxlxo"; // very different
        var result = VALUE.AreSimilar(OTHER, 0.9);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AreSimilarWhenDefaultThresholdTest()
    {
        const string VALUE = "kitten";
        const string OTHER = "sitting";
        // Fuzz.Ratio("kitten","sitting") ~ 62 → 0.62 < 0.95
        var result = VALUE.AreSimilar(OTHER);
        Assert.IsFalse(result);
    }
}