using System;
using System.Linq.Expressions;
using Vivet.AI.Services.Extensions;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models;

/// <summary>
/// Represents criteria for filtering knowledge entries when searching.
/// </summary>
public class KnowledgeSearchCriteria : BaseKnowledgeCriteria
{
    internal override Expression<Func<Data.Models.Knowledge, bool>> BuildFilter()
    {
        var parameterExpression = Expression.Parameter(typeof(Data.Models.Knowledge), "x");

        Expression body = null;

        var expression = parameterExpression
            .AddExpressionEqual(nameof(Data.Models.Knowledge.TenantId), this.TenantId?.ToString(), ref body)
            .AddExpressionEqual(nameof(Data.Models.Knowledge.SubTenantId), this.SubTenantId?.ToString(), ref body)
            .AddExpressionEqual(nameof(Data.Models.Knowledge.ScopeId), this.ScopeId?.ToString(), ref body)
            .AddExpressionEqual(nameof(Data.Models.Knowledge.UserId), this.UserId?.ToString(), ref body);

        if (body == null)
        {
            expression = parameterExpression
                .AddExpressionGreaterThan(nameof(Data.Models.Knowledge.UnixTimestamp), 0, ref body);
        }

        return expression
            .BuildExpression<Data.Models.Knowledge>(body);
    }
}