using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Vivet.AI.Data.Models;
using Vivet.AI.Services.Models;
using Vivet.AI.Services.Requests.Embedding.Knowledge.Enums;

namespace Vivet.AI.Services.Extensions;

internal static class ParameterExpressionExtensions
{
    internal static ParameterExpression AddExpressionEqual(this ParameterExpression parameterExpression, string name, object value, ref Expression body)
    {
        if (parameterExpression == null)
            throw new ArgumentNullException(nameof(parameterExpression));

        if (value == null)
        {
            return parameterExpression;
        }

        var propertyExpression = Expression.Property(parameterExpression, name);
        var valueExpression = Expression.Constant(value);
        var equalsExpression = Expression.Equal(propertyExpression, valueExpression);

        body = body == null
            ? equalsExpression
            : Expression.AndAlso(body, equalsExpression);

        return parameterExpression;
    }

    internal static ParameterExpression AddExpressionGreaterThan(this ParameterExpression parameterExpression, string name, long value, ref Expression body)
    {
        if (parameterExpression == null)
            throw new ArgumentNullException(nameof(parameterExpression));

        var propertyExpression = Expression.Property(parameterExpression, name);
        var convertedValue = Convert.ChangeType(value, propertyExpression.Type);
        var valueExpression = Expression.Constant(convertedValue, propertyExpression.Type);
        var greaterThanExpression = Expression.GreaterThan(propertyExpression, valueExpression);

        body = body == null
            ? greaterThanExpression
            : Expression.AndAlso(body, greaterThanExpression);

        return parameterExpression;
    }

    internal static ParameterExpression AddExpressionContains(this ParameterExpression parameterExpression, string name, object value, ref Expression body)
    {
        if (parameterExpression == null)
            throw new ArgumentNullException(nameof(parameterExpression));

        if (value == null)
        {
            return parameterExpression;
        }

        var propertyExpression = Expression.Property(parameterExpression, name);

        var elementType = propertyExpression.Type
            .GetInterfaces()
            .Where(x => 
                x.IsGenericType && 
                x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(x => x.GetGenericArguments()[0])
            .FirstOrDefault() ?? propertyExpression.Type.GetElementType();

        if (elementType == null)
        {
            throw new InvalidOperationException($"Property '{name}' is not a collection or array");
        }

        var typedValue = Expression.Constant(Convert.ChangeType(value, elementType), elementType);

        var containsMethod = typeof(Enumerable)
            .GetMethods()
            .First(x => 
                x.Name == "Contains" && 
                x.GetParameters().Length == 2)
            .MakeGenericMethod(elementType);

        var containsExpression = Expression.Call(containsMethod, propertyExpression, typedValue);

        body = body == null
            ? containsExpression
            : Expression.AndAlso(body, containsExpression);

        return parameterExpression;
    }

    internal static ParameterExpression AddDateRangeExpression(this ParameterExpression parameterExpression, DateRange dateRange, ref Expression body)
    {
        if (parameterExpression == null)
            throw new ArgumentNullException(nameof(parameterExpression));

        if (dateRange == null)
        {
            return parameterExpression;
        }

        if (dateRange.From.HasValue)
        {
            var unixTimestampStart = dateRange.From.Value
                .ToUnixTimeSeconds();

            var propertyExpression = Expression.Property(parameterExpression, nameof(BaseEmbedding.UnixTimestamp));
            var valueExpression = Expression.Constant(unixTimestampStart);
            var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertyExpression, valueExpression);

            body = body == null
                ? greaterThanOrEqualExpression
                : Expression.AndAlso(body, greaterThanOrEqualExpression);
        }

        if (dateRange.To != null)
        {
            var unixTimestampEnd = dateRange.To.Value
                .ToUnixTimeSeconds();

            var propertyExpression = Expression.Property(parameterExpression, nameof(BaseEmbedding.UnixTimestamp));
            var valueExpression = Expression.Constant(unixTimestampEnd);
            var lessThanOrEqualExpression = Expression.LessThanOrEqual(propertyExpression, valueExpression);

            body = body == null
                ? lessThanOrEqualExpression
                : Expression.AndAlso(body, lessThanOrEqualExpression);
        }

        return parameterExpression;
    }
 
    internal static ParameterExpression AddExpressionSearchFor(this ParameterExpression parameterExpression, SearchFor? searchFor, ref Expression body)
    {
        if (parameterExpression == null)
            throw new ArgumentNullException(nameof(parameterExpression));

        if (searchFor == null)
        {
            return parameterExpression;
        }

        switch (searchFor)
        {
            case SearchFor.Text:
                parameterExpression
                    .AddExpressionEqual(nameof(Knowledge.IsImage), false, ref body)
                    .AddExpressionEqual(nameof(Knowledge.IsAudio), false, ref body)
                    .AddExpressionEqual(nameof(Knowledge.IsVideo), false, ref body)
                    .AddExpressionEqual(nameof(Knowledge.IsDocument), false, ref body);
                break;

            case SearchFor.Audio:
                parameterExpression
                    .AddExpressionEqual(nameof(Knowledge.IsAudio), true, ref body);
                break;

            case SearchFor.Image:
                parameterExpression
                    .AddExpressionEqual(nameof(Knowledge.IsImage), true, ref body);
                break;

            case SearchFor.Video:
                parameterExpression
                    .AddExpressionEqual(nameof(Knowledge.IsVideo), true, ref body);
                break;

            case SearchFor.Document:
                parameterExpression
                    .AddExpressionEqual(nameof(Knowledge.IsDocument), true, ref body);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(searchFor));
        }

        return parameterExpression;
    }

    internal static Expression<Func<T, bool>> BuildExpression<T>(this ParameterExpression parameterExpression, Expression body)
        where T : BaseEmbedding
    {
        if (parameterExpression == null)
            throw new ArgumentNullException(nameof(parameterExpression));

        return body == null
            ? null
            : Expression.Lambda<Func<T, bool>>(body, parameterExpression);
    }
}