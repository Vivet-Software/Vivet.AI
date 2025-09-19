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

    /// <summary>
    /// This ID may be exposed by the underlying language model through its metadata. 
    /// Its presence is model-dependent and may not always be available.
    /// </summary>
    public virtual string ExternalId { get; set; }
}