namespace Vivet.AI.Services.Responses.Summarization;

/// <summary>
/// Represents the response of a memory summarization operation.
/// </summary>
public class SummarizationMemoryResponse : BaseResponse
{
    /// <summary>
    /// The summarized version of the original question.
    /// </summary>
    public virtual string QuestionSummarized { get; set; }

    /// <summary>
    /// The summarized version of the original answer.
    /// </summary>
    public virtual string AnswerSummarized { get; set; }
}