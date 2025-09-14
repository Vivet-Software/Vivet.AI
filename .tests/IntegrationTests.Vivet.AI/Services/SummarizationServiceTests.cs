using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Summarization;
using Vivet.AI.Services.Responses;

namespace IntegrationTests.Vivet.AI.Services;

[TestClass]
public class SummarizationServiceTests : BaseTests
{
    private ISummarizationService SummarizationService => this.ServiceProvider.GetRequiredService<ISummarizationService>();

    [TestMethod]
    public async Task SummarizeMemoryWhenDefaultTest()
    {
        const string QUESTION = "A girl, mistreated by her stepmother and stepsisters, dreams of a better life." +
                                "With magical help, she attends the royal ball, captivating the prince but fleeing at midnight, leaving behind a glass slipper." +
                                "The prince searches the kingdom, finds her, and they live happily ever after. what story is that";

        const string ANSWER = "That is the classic fairy tale of Cinderella, a beloved story where a kind-hearted young woman overcomes mistreatment with the aid of magic, " +
                              "attends a royal ball, and ultimately finds true love.";

        var response = await this.SummarizationService
            .SummarizeMemoryAsync(new SummarizeMemoryRequest
            {
                Question = QUESTION,
                Answer = ANSWER
            });

        Assert.IsNotNull(response);
        Assert.IsNull(response.ErrorMessage);
        Assert.IsNotNull(response.TokenUsage);
        Assert.IsTrue(response.TokenUsage.InputTokens > 250);
        Assert.IsTrue(response.TokenUsage.OutputTokens > 50);

        Assert.IsTrue(response.QuestionSummarized.Contains("magic"));
        Assert.IsTrue(response.QuestionSummarized.Contains("ball"));
        Assert.IsTrue(response.QuestionSummarized.Contains("prince"));

        var lengthDiffQuestion = (double)QUESTION.Length / response.QuestionSummarized.Length;
        Assert.IsTrue(lengthDiffQuestion >= 1.2);

        Assert.IsTrue(response.AnswerSummarized.Contains("Cinderella"));
        Assert.IsTrue(response.AnswerSummarized.Contains("love"));

        var lengthDiffAnswer = (double)ANSWER.Length / response.AnswerSummarized.Length;
        Assert.IsTrue(lengthDiffAnswer >= 1.2);
    }

    [TestMethod]
    public async Task SummarizeMemoryWhenSummarizationDegreeIsHighTest()
    {
        const string QUESTION = "A girl, mistreated by her stepmother and stepsisters, dreams of a better life." +
                                "With magical help, she attends the royal ball, captivating the prince but fleeing at midnight, leaving behind a glass slipper." +
                                "The prince searches the kingdom, finds her, and they live happily ever after. what story is that";

        const string ANSWER = "That is the classic fairy tale of Cinderella, a beloved story where a kind-hearted young woman overcomes mistreatment with the aid of magic, " +
                              "attends a royal ball, and ultimately finds true love.";

        var response = await this.SummarizationService
            .SummarizeMemoryAsync(new SummarizeMemoryRequest
            {
                Question = QUESTION,
                Answer = ANSWER,
                ConfigOverrides =
                {
                    SummarizationDegree = 75
                }
            });

        Assert.IsNotNull(response);
        Assert.IsNotNull(response.TokenUsage);

        Assert.IsTrue(response.QuestionSummarized.Contains("magic"));
        Assert.IsTrue(response.QuestionSummarized.Contains("ball"));
        Assert.IsTrue(response.QuestionSummarized.Contains("prince"));
        Assert.IsTrue(response.AnswerSummarized.Contains("Cinderella"));

        var lengthDiffQuestion = (double)QUESTION.Length / response.QuestionSummarized.Length;
        Assert.IsTrue(lengthDiffQuestion >= 1.5);

        Assert.IsTrue(response.AnswerSummarized.Contains("Cinderella"));

        var lengthDiffAnswer = (double)ANSWER.Length / response.AnswerSummarized.Length;
        Assert.IsTrue(lengthDiffAnswer >= 1.5);
    }

    [TestMethod]
    public async Task SummarizeMemoryWhenSummarizationDegreeIsZeroTest()
    {
        const string QUESTION = "A girl, mistreated by her stepmother and stepsisters, dreams of a better life." +
                                "With magical help, she attends the royal ball, captivating the prince but fleeing at midnight, leaving behind a glass slipper." +
                                "The prince searches the kingdom, finds her, and they live happily ever after. what story is that";

        const string ANSWER = "That is the classic fairy tale of Cinderella, a beloved story where a kind-hearted young woman overcomes mistreatment with the aid of magic, " +
                              "attends a royal ball, and ultimately finds true love.";

        var response = await this.SummarizationService
            .SummarizeMemoryAsync(new SummarizeMemoryRequest
            {
                Question = QUESTION,
                Answer = ANSWER,
                ConfigOverrides =
                {
                    SummarizationDegree = 0
                }
            });

        Assert.IsNotNull(response);
        Assert.AreEqual(QUESTION, response.QuestionSummarized);
        Assert.AreEqual(ANSWER, response.AnswerSummarized);
        Assert.IsNull(response.TokenUsage);
    }

    [TestMethod]
    public async Task SummarizeMemoryWhenQuestionAndAnswerContainsJsonTest()
    {
        // {'title':'Cinderella','mainCharacter':'Cinderella','antagonists':['Stepmother','Stepsisters'], 'helper':'Fairy Godmother','event':'Royal Ball','conflict':'She must flee before midnight','symbol':'Glass Slipper','ending':'Happily Ever After'}What story is this?

        const string QUESTION_JSON = @"{""title"":""Cinderella"",""mainCharacter"":""Cinderella"",""antagonists"":[""Stepmother"",""Stepsisters""],""helper"":""Fairy Godmother"",""event"":""Royal Ball"",""conflict"":""She must flee before midnight"",""symbol"":""Glass Slipper"",""ending"":""Happily Ever After""}";
        const string QUESTION = "A girl, mistreated by her stepmother and stepsisters, dreams of a better life." +
                                "With magical help, she attends the royal ball, captivating the prince but fleeing at midnight, leaving behind a glass slipper." +
                                $"{QUESTION_JSON}" +
                                "The prince searches the kingdom, finds her, and they live happily ever after. what story is that";

        const string ANSWER_JSON = @"{""story"":""Cinderella"",""genre"":""Fairy Tale"",""theme"":""Kindness and resilience"",""setting"":""Royal Kingdom"",""resolution"":""True Love""}";
        const string ANSWER = "That is the classic fairy tale of Cinderella, a beloved story where a kind-hearted young woman overcomes mistreatment with the aid of magic, " +
                              $"{ANSWER_JSON}" +
                              "attends a royal ball, and ultimately finds true love.";

        var response = await this.SummarizationService
            .SummarizeMemoryAsync(new SummarizeMemoryRequest
            {
                Question = QUESTION,
                Answer = ANSWER
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.QuestionSummarized.Replace('\'', '"').Contains(QUESTION_JSON), response.QuestionSummarized);
        Assert.IsTrue(response.AnswerSummarized.Replace('\'', '"').Contains(ANSWER_JSON));
    }

    [TestMethod]
    public async Task SummarizeMemoryWhenQuestionAndAnswerContainsXmlTest()
    {
        const string QUESTION_XML = "<story><title>Cinderella</title><mainCharacter>Cinderella</mainCharacter><antagonists><antagonist>Stepmother</antagonist><antagonist>Stepsisters</antagonist></antagonists><helper>Fairy Godmother</helper><event>Royal Ball</event><conflict>She must flee before midnight</conflict><symbol>Glass Slipper</symbol><ending>Happily Ever After</ending></story>";
        const string QUESTION = "A girl, mistreated by her stepmother and stepsisters, dreams of a better life." +
                                "With magical help, she attends the royal ball, captivating the prince but fleeing at midnight, leaving behind a glass slipper." +
                                $"{QUESTION_XML}" +
                                "The prince searches the kingdom, finds her, and they live happily ever after. what story is that";

        const string ANSWER_XML = "<answer><story>Cinderella</story><genre>Fairy Tale</genre><theme>Kindness and resilience</theme><setting>Royal Kingdom</setting><resolution>True Love</resolution></answer>";
        const string ANSWER = "That is the classic fairy tale of Cinderella, a beloved story where a kind-hearted young woman overcomes mistreatment with the aid of magic, " +
                              $"{ANSWER_XML}" +
                              "attends a royal ball, and ultimately finds true love.";

        var response = await this.SummarizationService
            .SummarizeMemoryAsync(new SummarizeMemoryRequest
            {
                Question = QUESTION,
                Answer = ANSWER
            });

        Assert.IsNotNull(response);
        Assert.IsTrue(response.QuestionSummarized.Contains(QUESTION_XML), response.QuestionSummarized);
        Assert.IsTrue(response.AnswerSummarized.Contains(ANSWER_XML), response.AnswerSummarized);
    }

    [TestMethod]
    public async Task SummarizeMemoryWhenOverrideModelTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task SummarizeMemoryWhenCustomPluginTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task SummarizeMemoryWhenErrorMessageTest()
    {
        const string QUESTION = $"This is a test request, where I want you to respond with an {nameof(BaseResponse.ErrorMessage)}.";
        const string ANSWER = "N/A";

        await Assert.ThrowsAsync<AiException>(async () => await this.SummarizationService
            .SummarizeMemoryAsync(new SummarizeMemoryRequest
            {
                Question = QUESTION,
                Answer = ANSWER
            }));
    }
}