using System;
using System.Linq.Expressions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Enums;

namespace Vivet.AI.Services.Requests.Embedding.Knowledge.Models;

/// <summary>
/// Represents criteria for filtering knowledge entries when querying.
/// </summary>
public class KnowledgeQueryCriteria : BaseKnowledgeCriteria
{
    /// <summary>
    /// The language of the knowledge entries to filter.
    /// </summary>
    public virtual string Language { get; set; }

    /// <summary>
    /// A tag to filter knowledge entries by.
    /// </summary>
    public virtual string Tag { get; set; }

    /// <summary>
    /// Specifies which type of content to search for.
    /// </summary>
    public virtual SearchFor? SearchFor { get; set; }

    /// <summary>
    /// Gets or sets the date range for filtering.
    /// </summary>
    public virtual DateRange DateRange { get; set; }

    internal override Expression<Func<Data.Models.Knowledge, bool>> BuildFilter()
    {
        var parameterExpression = Expression.Parameter(typeof(Data.Models.Knowledge), "x");

        Expression body = null;

        var expression = parameterExpression
            .AddExpressionEqual(nameof(Data.Models.Knowledge.TenantId), this.TenantId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Knowledge.SubTenantId), this.SubTenantId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Knowledge.ScopeId), this.ScopeId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Knowledge.UserId), this.UserId, ref body)
            .AddExpressionEqual(nameof(Data.Models.Knowledge.Language), this.Language, ref body)
            .AddExpressionContains(nameof(Data.Models.Knowledge.Tags), this.Tag, ref body)
            .AddExpressionSearchFor(this.SearchFor, ref body)
            .AddDateRangeExpression(nameof(Data.Models.Knowledge.UnixTimestamp), this.DateRange, ref body);

        if (body == null)
        {
            expression = parameterExpression
                .AddExpressionGreaterThan(nameof(Data.Models.Knowledge.UnixTimestamp), 0, ref body);
        }

        return expression
            .BuildExpression<Data.Models.Knowledge>(body);
    }
}