using System;
using System.Linq.Expressions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models;

namespace Vivet.AI.Services.Requests.Embedding.Memory.Models;

// BUG: An error occurred. The binary operator Equal is not defined for the types 'System.String' and 'System.Guid'.

/// <summary>
/// Represents criteria for filtering memory entries when searching.
/// </summary>
public class MemorySearchCriteria : BaseMemoryCriteria
{
    /// <summary>
    /// The number of days in the past to search for memories.
    /// </summary>
    public virtual int? RetentionInDays { get; set; }

    internal override Expression<Func<Data.Models.Memory, bool>> BuildFilter()
    {
        var parameterExpression = Expression.Parameter(typeof(Data.Models.Memory), "x");

        Expression body = null;

        var from = DateTimeOffset.UtcNow
            .AddDays(-this.RetentionInDays.GetValueOrDefault());

        var dateRange = new DateRange
        {
            From = from
        };

        var expression = parameterExpression
            .AddExpressionEqual(nameof(Data.Models.Memory.UserId), this.UserId?.ToString(), ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.ScopeId), this.ScopeId?.ToString(), ref body)
            .AddExpressionEqual(nameof(Data.Models.Memory.AgentId), this.AgentId?.ToString(), ref body)
            .AddDateRangeExpression(nameof(Data.Models.Memory.UnixTimestamp), dateRange, ref body);

        if (body == null)
        {
            expression = parameterExpression
                .AddExpressionGreaterThan(nameof(Data.Models.Memory.UnixTimestamp), 0, ref body);
        }

        return expression
            .BuildExpression<Data.Models.Memory>(body);
    }
}