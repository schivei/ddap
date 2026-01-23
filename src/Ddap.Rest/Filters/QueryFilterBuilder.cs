using System.Linq.Expressions;
using System.Reflection;

namespace Ddap.Rest.Filters;

/// <summary>
/// Builds dynamic LINQ queries from OData-like filter expressions.
/// Supports basic comparison operators and logical operators.
/// </summary>
/// <example>
/// <code>
/// var query = dbContext.Users.AsQueryable();
///
/// // Apply filter: "age gt 18 and status eq 'active'"
/// query = QueryFilterBuilder.ApplyFilter(query, "age gt 18 and status eq 'active'");
///
/// // Apply sorting: "name desc"
/// query = QueryFilterBuilder.ApplyOrdering(query, "name desc");
///
/// var results = await query.ToListAsync();
/// </code>
/// </example>
public static class QueryFilterBuilder
{
    /// <summary>
    /// Applies a filter expression to the queryable.
    /// Supports operators: eq, ne, gt, ge, lt, le, contains, and, or
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to filter.</param>
    /// <param name="filterExpression">The filter expression.</param>
    /// <returns>The filtered queryable.</returns>
    /// <example>
    /// <code>
    /// var filtered = QueryFilterBuilder.ApplyFilter(users, "name eq 'John' and age gt 25");
    /// </code>
    /// </example>
    public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, string filterExpression)
    {
        if (string.IsNullOrWhiteSpace(filterExpression))
            return query;

        try
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var expression = ParseFilterExpression(filterExpression, parameter, typeof(T));
            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            return query.Where(lambda);
        }
        catch (Exception)
        {
            // Return unfiltered query if filter expression is invalid
            return query;
        }
    }

    /// <summary>
    /// Applies ordering to the queryable.
    /// Supports single or multiple sort fields with asc/desc direction.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to sort.</param>
    /// <param name="orderByExpression">The order by expression.</param>
    /// <returns>The sorted queryable.</returns>
    /// <example>
    /// <code>
    /// var sorted = QueryFilterBuilder.ApplyOrdering(users, "name asc, age desc");
    /// </code>
    /// </example>
    public static IQueryable<T> ApplyOrdering<T>(IQueryable<T> query, string orderByExpression)
    {
        if (string.IsNullOrWhiteSpace(orderByExpression))
            return query;

        var orderClauses = orderByExpression.Split(',', StringSplitOptions.RemoveEmptyEntries);
        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var clause in orderClauses)
        {
            var parts = clause.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var propertyName = parts[0];
            var direction = parts.Length > 1 ? parts[1].ToLower() : "asc";

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (property == null)
                continue;

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByLambda = Expression.Lambda(propertyAccess, parameter);

            var methodName =
                orderedQuery == null
                    ? (direction == "desc" ? "OrderByDescending" : "OrderBy")
                    : (direction == "desc" ? "ThenByDescending" : "ThenBy");

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.PropertyType },
                orderedQuery?.Expression ?? query.Expression,
                Expression.Quote(orderByLambda)
            );

            orderedQuery =
                orderedQuery == null
                    ? (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExpression)
                    : (IOrderedQueryable<T>)orderedQuery.Provider.CreateQuery<T>(resultExpression);
        }

        return orderedQuery ?? query;
    }

    private static Expression ParseFilterExpression(
        string filter,
        ParameterExpression parameter,
        Type entityType
    )
    {
        filter = filter.Trim();

        // Handle logical operators
        var orIndex = FindLogicalOperator(filter, " or ");
        if (orIndex != -1)
        {
            var left = ParseFilterExpression(filter.Substring(0, orIndex), parameter, entityType);
            var right = ParseFilterExpression(filter.Substring(orIndex + 4), parameter, entityType);
            return Expression.OrElse(left, right);
        }

        var andIndex = FindLogicalOperator(filter, " and ");
        if (andIndex != -1)
        {
            var left = ParseFilterExpression(filter.Substring(0, andIndex), parameter, entityType);
            var right = ParseFilterExpression(
                filter.Substring(andIndex + 5),
                parameter,
                entityType
            );
            return Expression.AndAlso(left, right);
        }

        // Parse comparison expression
        return ParseComparison(filter, parameter, entityType);
    }

    private static Expression ParseComparison(
        string expression,
        ParameterExpression parameter,
        Type entityType
    )
    {
        var operators = new[] { " eq ", " ne ", " gt ", " ge ", " lt ", " le ", " contains " };

        foreach (var op in operators)
        {
            var index = expression.IndexOf(op, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                continue;

            var propertyName = expression.Substring(0, index).Trim();
            var valueStr = expression.Substring(index + op.Length).Trim().Trim('\'', '"');

            var property = entityType.GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );
            if (property == null)
                continue;

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var value = ConvertValue(valueStr, property.PropertyType);
            var constant = Expression.Constant(value, property.PropertyType);

            return op.Trim() switch
            {
                "eq" => Expression.Equal(propertyAccess, constant),
                "ne" => Expression.NotEqual(propertyAccess, constant),
                "gt" => Expression.GreaterThan(propertyAccess, constant),
                "ge" => Expression.GreaterThanOrEqual(propertyAccess, constant),
                "lt" => Expression.LessThan(propertyAccess, constant),
                "le" => Expression.LessThanOrEqual(propertyAccess, constant),
                "contains" => Expression.Call(
                    propertyAccess,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    constant
                ),
                _ => Expression.Constant(true),
            };
        }

        return Expression.Constant(true);
    }

    private static int FindLogicalOperator(string expression, string op)
    {
        var level = 0;
        for (int i = 0; i < expression.Length; i++)
        {
            if (expression[i] == '(')
                level++;
            if (expression[i] == ')')
                level--;
            if (
                level == 0
                && i + op.Length <= expression.Length
                && expression.Substring(i, op.Length).Equals(op, StringComparison.OrdinalIgnoreCase)
            )
            {
                return i;
            }
        }
        return -1;
    }

    private static object? ConvertValue(string value, Type targetType)
    {
        if (targetType == typeof(string))
            return value;

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(int))
            return int.Parse(value);
        if (underlyingType == typeof(long))
            return long.Parse(value);
        if (underlyingType == typeof(decimal))
            return decimal.Parse(value);
        if (underlyingType == typeof(double))
            return double.Parse(value);
        if (underlyingType == typeof(bool))
            return bool.Parse(value);
        if (underlyingType == typeof(DateTime))
            return DateTime.Parse(value);
        if (underlyingType == typeof(Guid))
            return Guid.Parse(value);

        return value;
    }
}
