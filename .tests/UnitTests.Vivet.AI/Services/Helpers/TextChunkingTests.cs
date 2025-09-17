using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Data.Models;
using Vivet.AI.Services.Helpers;
using Vivet.AI.Services.Helpers.Models;

namespace UnitTests.Vivet.AI.Services.Helpers;

[TestClass]
public class TextChunkingTests
{
    [TestMethod]
    public void GetTextChunksWhenTextLengthLessThanMaxTest()
    {
        const string TEXT = "This is sentence one. This is sentence two.";
        var chunks = TextChunking.GetTextChunks(TEXT, 1, 10);

        // Because tokens (approx 4 + 4) <= 10, they will be merged into a single chunk
        Assert.AreEqual(1, chunks.Length);
        Assert.IsTrue(chunks[0].Text.Contains("This is sentence one"));
        Assert.IsTrue(chunks[0].Text.Contains("This is sentence two"));
    }

    [TestMethod]
    public void GetTextChunksWhenTextLengthGreaterThanMaxTest()
    {
        const string TEXT = "This is an example sentence one. This is an example sentence two.";
        var chunks = TextChunking.GetTextChunks(TEXT, 1, 7);

        Assert.AreEqual(2, chunks.Length, "Expected two chunks when maxTokens prevents merging.");
        Assert.IsTrue(chunks[0].Text.StartsWith("This is an example sentence one"));
        Assert.IsTrue(chunks[1].Text.StartsWith("This is an example sentence two"));
    }

    [TestMethod]
    public void GetTextChunksWhenMinTokenSmallerThanSentenceTest()
    {
        const string TEXT = "This is a short sentence.";
        var chunks = TextChunking.GetTextChunks(TEXT, 10, 50);

        Assert.AreEqual(1, chunks.Length);
        Assert.IsTrue(chunks[0].Text.Contains("This is a short sentence."));
    }

    [TestMethod]
    public void GetTextChunksWhenMaxTokenSmallerThanSentenceTest()
    {
        var sentence = string.Join(" ", Enumerable.Range(0, 20).Select(_ => "word")) + ".";
        var chunks = TextChunking.GetTextChunks(sentence, 1, 10);

        Assert.AreEqual(1, chunks.Length);
        Assert.AreEqual(sentence, chunks[0].Text);
        Assert.IsTrue(chunks[0].TokenCount > 10);
    }

    [TestMethod]
    public void GetTextChunksWhenVeryLongSentenceTest()
    {
        var longSentence = string.Join(" ", Enumerable.Range(0, 50).Select(_ => " word")) + ".";
        var text = longSentence + " Relatively short and concise sentence okay";
        var chunks = TextChunking.GetTextChunks(text, 3, 10);

        Assert.IsTrue(chunks.Length >= 2, "Expected at least two chunks: the long sentence and the short one.");
        Assert.IsTrue(chunks[0].TokenCount > 10, "The long sentence should have token count > maxTokens.");
        Assert.IsTrue(chunks.Any(c => c.Text.Contains("Relatively short and concise sentence okay")));
    }

    [TestMethod]
    public void GetTextChunksWhenExactFitTest()
    {
        const string TEXT = "One two three four five. Six seven eight nine ten.";
        var chunks = TextChunking.GetTextChunks(TEXT, 5, 10);

        Assert.AreEqual(1, chunks.Length);
        Assert.AreEqual(10, chunks[0].TokenCount);
    }

    [TestMethod]
    public void GetTextChunksWhenWhenLeftoverChunkTest()
    {
        const string TEXT = "One two three. Four five six.";
        var chunks = TextChunking.GetTextChunks(TEXT, 2, 4);

        Assert.IsTrue(chunks.Length >= 1);
        Assert.IsTrue(chunks.Last().TokenCount >= 2);
        Assert.IsTrue(chunks.Last().Text.Contains("Four five six"));
    }

    [TestMethod]
    public void GetTextChunksWhenJsonInlineTest()
    {
        const string TEXT = "Here is a response: {\"key\": \"value\"} Next sentence follows.";
        var chunks = TextChunking.GetTextChunks(TEXT, 1, 50);

        Assert.IsTrue(chunks.Any(c => c.Text.Contains("{\"key\": \"value\"}")));
        Assert.IsTrue(chunks.Any(c => c.Text.Contains("Here is a response")));
        Assert.IsTrue(chunks.Any(c => c.Text.Contains("Next sentence follows")));
    }

    [TestMethod]
    public void GetTextChunksWhenXmlInlineTest()
    {
        const string TEXT = "Start text <note><to>User</to><from>AI</from></note> End text.";
        var chunks = TextChunking.GetTextChunks(TEXT, 1, 50);

        Assert.IsTrue(chunks.Any(c => c.Text.Contains("<note><to>User</to><from>AI</from></note>")));
        Assert.IsTrue(chunks.Any(c => c.Text.Contains("Start text")));
        Assert.IsTrue(chunks.Any(c => c.Text.Contains("End text")));
    }

    [TestMethod]
    public void GetTextChunksWhenMultipleParagraphsTest()
    {
        const string TEXT = "Paragraph one.\n\nParagraph two.\n\nParagraph three.";
        var chunks = TextChunking.GetTextChunks(TEXT, 1, 50);

        var paragraphIds = chunks.Select(c => c.ParagraphId).Distinct().ToList();
        Assert.AreEqual(3, paragraphIds.Count);
    }

    [TestMethod]
    public void GetTextChunksWhenEmptyParagraphsTest()
    {
        const string TEXT = "Paragraph one.\n\n\n\nParagraph two.";
        var chunks = TextChunking.GetTextChunks(TEXT, 1, 50);

        var paragraphIds = chunks.Select(c => c.ParagraphId).Distinct().ToList();
        Assert.AreEqual(2, paragraphIds.Count);
    }

    [TestMethod]
    public void GetTextChunksWhenSingleVeryLongTokenTest()
    {
        var longToken = new string('x', 100);
        var text = $"Start {longToken} End";
        var chunks = TextChunking.GetTextChunks(text, 1, 10);

        Assert.IsTrue(chunks.Any(c => c.Text.Contains(longToken)));
    }

    [TestMethod]
    public void GetTextChunksWhenTextIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => TextChunking.GetTextChunks(null, 5, 10));
    }


    [TestMethod]
    public void GetNeighborContextWhenRestrictToSameParagraphIsTrueTest()
    {
        var chunks = new[]
        {
            new TextChunk { Text = "A", ParagraphId = 1 },
            new TextChunk { Text = "B", ParagraphId = 1 },
            new TextChunk { Text = "C", ParagraphId = 2 }
        };

        var context = TextChunking.GetTextChunkNeighboringContext(chunks, 1, 1, true);

        Assert.AreEqual("A B", context);
    }
  
    [TestMethod]
    public void GetNeighborContextWhenRestrictToSameParagraphIsFalseTest()
    {
        var chunks = new[]
        {
            new TextChunk { Text = "A", ParagraphId = 1 },
            new TextChunk { Text = "B", ParagraphId = 2 },
            new TextChunk { Text = "C", ParagraphId = 3 }
        };

        var context = TextChunking.GetTextChunkNeighboringContext(chunks, 1, 1, false);

        Assert.AreEqual("A B C", context);
    }

    [TestMethod]
    public void GetNeighborContextWhenBoundaryHandlingSkipsInvalidIndexesTest()
    {
        var chunks = new[]
        {
            new TextChunk { Text = "A", ParagraphId = 1 },
            new TextChunk { Text = "B", ParagraphId = 1 }
        };

        var context = TextChunking.GetTextChunkNeighboringContext(chunks, 0, 1, false);
        Assert.AreEqual("A B", context);
    }

    [TestMethod]
    public void GetTextChunkNeighboringContextWhenMixedParagraphsAndJsonTest()
    {
        var chunks = new[]
        {
            new TextChunk { Text = "Intro text", ParagraphId = 1 },
            new TextChunk { Text = "{\"json\":1}", ParagraphId = 1 },
            new TextChunk { Text = "Following text", ParagraphId = 2 }
        };

        var context = TextChunking.GetTextChunkNeighboringContext(chunks, 1, 1, true);
        Assert.AreEqual("Intro text {\"json\":1}", context);

        var contextAll = TextChunking.GetTextChunkNeighboringContext(chunks, 1, 1, false);
        Assert.AreEqual("Intro text {\"json\":1} Following text", contextAll);
    }

    [TestMethod]
    public void GetTextChunkNeighboringContextWhenChunksIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => TextChunking.GetTextChunkNeighboringContext(null, 0, 1, false));
    }

    [TestMethod]
    public void GetNeighborContextWhenIndexOutOfRangeTest()
    {
        var chunks = new[]
        {
            new TextChunk
            {
                Text = "text",
                ParagraphId = 1
            }
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => TextChunking.GetTextChunkNeighboringContext(chunks, 5, 1, false));
    }
}

internal class TestEmbedding : BaseEmbedding
{
    public TestEmbedding(long unixTimestamp)
    {
        this.UnixTimestamp = unixTimestamp;
    }
}