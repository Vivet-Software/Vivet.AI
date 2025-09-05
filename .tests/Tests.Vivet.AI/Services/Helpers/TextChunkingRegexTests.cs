using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Helpers;

namespace Tests.Vivet.AI.Services.Helpers;

[TestClass]
public class TextChunkingRegexTests
{
    [TestMethod]
    public void GetParagraphsTest()
    {
        const string TEXT = "First paragraph.\n\nSecond paragraph.\r\n\r\nThird paragraph.";

        var paragraphs = TextChunkingRegex.GetParagraphs(TEXT).ToArray();

        Assert.AreEqual(3, paragraphs.Length);
        Assert.AreEqual("First paragraph.", paragraphs[0]);
        Assert.AreEqual("Second paragraph.", paragraphs[1]);
        Assert.AreEqual("Third paragraph.", paragraphs[2]);
    }

    [TestMethod]
    public void GetParagraphsWhenTextIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => TextChunkingRegex.GetParagraphs(null));
    }


    [TestMethod]
    public void GetSentencesTest()
    {
        RunGetSentencesTest("Hello world. This is a test.", 2, "Hello world.", "This is a test.");
    }

    [TestMethod]
    public void GetSentencesWhenSingleLetterInitialTest()
    {
        RunGetSentencesTest("My name is J. Smith. Nice to meet you.", 2, "My name is J. Smith.", "Nice to meet you.");
    }

    [TestMethod]
    public void GetSentencesWhenShortAbbreviationsTest()
    {
        RunGetSentencesTest("I met Dr. Adams. He was kind.", 2, "I met Dr. Adams.", "He was kind.");
    }

    [TestMethod]
    public void GetSentencesWhenDoubleInitialsNoSpaceTest()
    {
        RunGetSentencesTest("He lives in the U.S. It is sunny and everybody is happy.", 2, "He lives in the U.S.", "It is sunny and everybody is happy.");
    }

    [TestMethod]
    public void GetSentencesWhenDoubleInitialsNoSpaceAndLessThanXWordsAfterTest()
    {
        RunGetSentencesTest("He lives in the U.S. It is sunny.", 1, "He lives in the U.S. It is sunny.");
    }

    [TestMethod]
    public void GetSentencesWhenDoubleInitialsWithSpacesTest()
    {
        RunGetSentencesTest("He lives in the U. S. It is sunny and everybody is happy.", 2, "He lives in the U. S.", "It is sunny and everybody is happy.");
    }

    [TestMethod]
    public void GetSentencesWhenDoubleInitialsWithSpacesAndLessThanXWordsAfterTest()
    {
        RunGetSentencesTest("He lives in the U. S. It is sunny.", 1, "He lives in the U. S. It is sunny.");
    }

    [TestMethod]
    public void GetSentencesWhenTripleInitialsNoSpaceTest()
    {
        RunGetSentencesTest("She met A.B.C. Johnson. He was late.", 2, "She met A.B.C. Johnson.", "He was late.");
    }

    [TestMethod]
    public void GetSentencesWhenTripleInitialsWithSpacesTest()
    {
        RunGetSentencesTest("She met A. B. C. Johnson. He was late.", 2, "She met A. B. C. Johnson.", "He was late.");
    }

    [TestMethod]
    public void GetSentencesWhenMultipleInitialsTest()
    {
        RunGetSentencesTest("Letters A. B. C. D. E. F. are here and will stay forever. Now a new sentence.", 2, "Letters A. B. C. D. E. F. are here and will stay forever.", "Now a new sentence.");
    }

    [TestMethod]
    public void GetSentencesWhenDecimalNumbersTest()
    {
        RunGetSentencesTest("The value is 3.14. Pi is interesting.", 2, "The value is 3.14.", "Pi is interesting.");
    }

    [TestMethod]
    public void GetSentencesWhenLetterNumberTest()
    {
        RunGetSentencesTest("The plane B.52 landed safely. All is well.", 2, "The plane B.52 landed safely.", "All is well.");
    }

    [TestMethod]
    public void GetSentencesWhenAbbreviationMidSentenceTest()
    {
        RunGetSentencesTest("Call Mr. Smith now. He has been waiting for a long time.", 2, "Call Mr. Smith now.", "He has been waiting for a long time.");
    }

    [TestMethod]
    public void GetSentencesWhenInitialsAndShortSentenceAfterTest()
    {
        RunGetSentencesTest("She met A.B.C. Peter Manfred Johnson. The President.", 2, "She met A.B.C. Peter Manfred Johnson.", "The President.");
    }

    [TestMethod]
    public void GetSentencesWhenNoUppercaseAfterPeriodTest()
    {
        RunGetSentencesTest("He lives in the U.S. and likes it.", 1, "He lives in the U.S. and likes it.");
    }

    [TestMethod]
    public void GetSentencesWhenNestedJsonObjectTest()
    {
        const string JSON = "{\"a\":1,\"b\":{\"c\":2}}";

        RunGetSentencesTest(JSON, 1, JSON);
    }

    [TestMethod]
    public void GetSentencesWhenNestedJsonArrayTest()
    {
        const string JSON_ARRAY = "[1,[2,3],4]";

        RunGetSentencesTest(JSON_ARRAY, 1, JSON_ARRAY);
    }

    [TestMethod]
    public void GetSentencesJsonWithWhitespaceTest()
    {
        const string JSON = "{\n  \"a\": 1,\n  \"b\": [1,2,3]\n}";

        RunGetSentencesTest(JSON, 1, JSON);
    }

    [TestMethod]
    public void GetSentencesNestedXmlTest()
    {
        const string XML = "<root><child><subchild>value</subchild></child></root>";

        RunGetSentencesTest(XML, 1, XML);
    }

    [TestMethod]
    public void GetSentencesWhenXmlWithSelfClosingTest()
    {
        const string XML = "<root><child /><child2>value</child2></root>";

        RunGetSentencesTest(XML, 1, XML);
    }

    [TestMethod]
    public void GetSentencesWhenJsonAtStartMiddleEndTest()
    {
        const string TEXT = "{\"start\":1} middle text {\"end\":2}";

        var sentences = TextChunkingRegex.GetSentences(TEXT).ToArray();

        Assert.AreEqual(3, sentences.Length);
        Assert.AreEqual("{\"start\":1}", sentences[0]);
        Assert.AreEqual("middle text", sentences[1]);
        Assert.AreEqual("{\"end\":2}", sentences[2]);
    }

    [TestMethod]
    public void GetSentencesWHenXmlEmbeddedInsideTextTest()
    {
        const string TEXT = "Intro <tag>value</tag> Outro.";

        var sentences = TextChunkingRegex.GetSentences(TEXT).ToArray();

        Assert.AreEqual(3, sentences.Length);
        Assert.AreEqual("Intro", sentences[0]);
        Assert.AreEqual("<tag>value</tag>", sentences[1]);
        Assert.AreEqual("Outro.", sentences[2]);
    }

    [TestMethod]
    public void GetSentencesWhenMixedJsonXmlTest()
    {
        const string TEXT = "Start {\"a\":1} Middle <tag>text</tag> End.";

        var sentences = TextChunkingRegex.GetSentences(TEXT).ToArray();

        Assert.AreEqual(5, sentences.Length);
        Assert.AreEqual("Start", sentences[0]);
        Assert.AreEqual("{\"a\":1}", sentences[1]);
        Assert.AreEqual("Middle", sentences[2]);
        Assert.AreEqual("<tag>text</tag>", sentences[3]);
        Assert.AreEqual("End.", sentences[4]);
    }

    [TestMethod]
    public void GetSentencesWhenJsonAndXmlWithPunctuationTest()
    {
        const string TEXT = "Hello {\"key\":123}. Next <tag/> sentence.";

        var sentences = TextChunkingRegex.GetSentences(TEXT).ToArray();

        Assert.AreEqual(5, sentences.Length);
        Assert.AreEqual("Hello", sentences[0]);
        Assert.AreEqual("{\"key\":123}.", sentences[1]);
        Assert.AreEqual("Next", sentences[2]);
        Assert.AreEqual("<tag/>", sentences[3]);
        Assert.AreEqual("sentence.", sentences[4]);
    }

    [TestMethod]
    public void GetSentencesWhenAdjacentJsonXmlTest()
    {
        const string TEXT = "{\"a\":1}<tag>value</tag>End.";

        var sentences = TextChunkingRegex.GetSentences(TEXT).ToArray();

        Assert.AreEqual(3, sentences.Length);
        Assert.AreEqual("{\"a\":1}", sentences[0]);
        Assert.AreEqual("<tag>value</tag>", sentences[1]);
        Assert.AreEqual("End.", sentences[2]);
    }

    [TestMethod]
    public void GetSentencesWhen_JsonInsideTextTest()
    {
        const string TEXT = "Intro {\"a\":1} End.";

        var sentences = TextChunkingRegex.GetSentences(TEXT).ToArray();

        Assert.AreEqual(3, sentences.Length);
        Assert.AreEqual("Intro", sentences[0]);
        Assert.AreEqual("{\"a\":1}", sentences[1]);
        Assert.AreEqual("End.", sentences[2]);
    }

    [TestMethod]
    public void GetSentencesWhenComplexTest()
    {
        RunGetSentencesTest("A. B. C. D. met with Dr. X. at 3.14 p.m. It was very good and good.", 2, "A. B. C. D. met with Dr. X. at 3.14 p.m.", "It was very good and good.");
    }

    [TestMethod]
    public void GetSentencesWhenParagraphIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => TextChunkingRegex.GetSentences(null));
    }


    [TestMethod]
    public void GetJsonMatchesWHenMultipleNestedObjectsAndArraysTest()
    {
        const string TEXT = "Start {\"a\":1,\"b\":[2,3]} middle [4,5,{\"c\":6}] end";

        var matches = TextChunkingRegex.GetJsonMatches(TEXT).ToArray();

        Assert.AreEqual(2, matches.Length);
        Assert.AreEqual("{\"a\":1,\"b\":[2,3]}", matches[0].Value);
        Assert.AreEqual("[4,5,{\"c\":6}]", matches[1].Value);
    }


    [TestMethod]
    public void GetXmlMatchesWhenNestedAndSelfClosingTagsTest()
    {
        const string TEXT = "<root><a><b>1</b></a><c/></root>";

        var matches = TextChunkingRegex.GetXmlMatches(TEXT).ToArray();

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("<root><a><b>1</b></a><c/></root>", matches[0].Value);
    }

    [TestMethod]
    public void GetXmlMatchesWhenMultipleTopLevelTagsTest()
    {
        const string TEXT = "<a>1</a><b>2</b><c/>Text";

        var matches = TextChunkingRegex.GetXmlMatches(TEXT).ToArray();

        Assert.AreEqual(3, matches.Length);
        Assert.AreEqual("<a>1</a>", matches[0].Value);
        Assert.AreEqual("<b>2</b>", matches[1].Value);
        Assert.AreEqual("<c/>", matches[2].Value);
    }

    [TestMethod]
    public void GetXmlMatchesWhenTextIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => TextChunkingRegex.GetXmlMatches(null));
    }


    [TestMethod]
    public void GetTokenCountWhenSentenceIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => TextChunkingRegex.GetTokenCount(null));
    }

    [TestMethod]
    public void GetTokenCountTest()
    {
        const string SENTENCE = "This is a test sentence with seven tokens.";

        var count = TextChunkingRegex.GetTokenCount(SENTENCE);

        Assert.AreEqual(8, count);
    }

    [TestMethod]
    public void GetTokenCountWhenSentenceIsEmptyStringTest()
    {
        var count = TextChunkingRegex.GetTokenCount(string.Empty);

        Assert.AreEqual(0, count);
    }


    private static void RunGetSentencesTest(string input, int expectedCount, params string[] expectedSentences)
    {
        var sentences = TextChunkingRegex.GetSentences(input).ToArray();

        Assert.AreEqual(expectedCount, sentences.Length, $"Expected {expectedCount} sentences but got {sentences.Length}");
        CollectionAssert.AreEqual(expectedSentences, sentences);
    }
}