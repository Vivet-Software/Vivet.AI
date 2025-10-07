using System;
using System.Linq.Expressions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Requests.Embedding.Memory.Models;

/// <summary>
/// Represents criteria for filtering memory entries when querying.
/// </summary>
public class MemoryQueryCriteria : BaseMemoryCriteria
{
    /// <summary>
    /// The ID of the thread or conversation.
    /// </summary>
    public virtual Guid? ThreadId { get; set; }

    /// <summary>
    /// The ID of the specific question-answer pair.
    /// </summary>
    public virtual Guid? QuestionAnswerId { get; set; }

    /// <summary>
    /// Indicates whether the memory entry is a question.
    /// </summary>
    public virtual bool? IsQuestion { get; set; }

    /// <summary>
    /// Indicates whether the memory entry is an answer.
    /// </summary>
    public virtual bool? IsAnswer { get; set; }

    /// <summary>
    /// Gets or sets the date range for filtering.
    /// </summary>
    public virtual DateRange DateRange { get; set; }

    internal override Expression<Func<Data.Models.Memory, bool>> BuildFilter()
    {
        var parameterExpression = Expression.Parameter(typeof(Data.Models.Memory), "x");

        Expression body = null;

        var expression = parameterExpression
            .AddExpressionEqual(nameof(Data.Models.Memory.UserId), this.UserId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.ScopeId), this.ScopeId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.AgentId), this.AgentId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.ThreadId), this.ThreadId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.QuestionAnswerId), this.QuestionAnswerId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.IsQuestion), this.IsQuestion, ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.IsAnswer), this.IsAnswer, ref body)
            .AddDateRangeExpression(nameof(Data.Models.Memory.UnixTimestamp), this.DateRange, ref body);

        if (body == null)
        {
            expression = parameterExpression
                .AddExpressionGreaterThan(nameof(Data.Models.Memory.UnixTimestamp), 0, ref body);
        }

        return expression
            .BuildExpression<Data.Models.Memory>(body);
    }
}