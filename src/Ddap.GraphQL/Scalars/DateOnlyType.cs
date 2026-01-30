using HotChocolate.Language;
using HotChocolate.Types;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Scalar type for DateOnly values in GraphQL.
/// Maps DateOnly to a string in ISO 8601 format (yyyy-MM-dd).
/// Uses DateTimeValueNode for proper serialization without type inference.
/// </summary>
public class DateOnlyType : ScalarType<DateOnly, DateTimeValueNode>
{
    private const string SpecifiedByUrl = "https://www.graphql-scalars.com/date-time";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateOnlyType"/> class with full configuration.
    /// </summary>
    public DateOnlyType(
        string name,
        string? description = null,
        BindingBehavior bind = BindingBehavior.Explicit
    )
        : base(name, bind)
    {
        Description = description;
        SpecifiedBy = new Uri(SpecifiedByUrl);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateOnlyType"/> class with default configuration.
    /// </summary>
    public DateOnlyType()
        : this(
            "DateOnly",
            "Represents a date without time information in ISO 8601 format (yyyy-MM-dd).",
            BindingBehavior.Implicit
        ) { }

    /// <summary>
    /// Parse the literal value node from GraphQL query.
    /// </summary>
    protected override DateOnly ParseLiteral(DateTimeValueNode valueSyntax)
    {
        return valueSyntax.TryToDateOnly(out var value)
            ? value
            : throw new SerializationException(
                "The string value is not a valid DateOnly representation.",
                this
            );
    }

    /// <summary>
    /// Serialize the runtime value to a value node.
    /// </summary>
    protected override DateTimeValueNode ParseValue(DateOnly runtimeValue)
    {
        return new DateTimeValueNode(runtimeValue);
    }

    /// <summary>
    /// Parse result from execution engine.
    /// </summary>
    public override IValueNode ParseResult(object? resultValue)
    {
        return resultValue switch
        {
            null => NullValueNode.Default,
            string s => new DateTimeValueNode(s),
            DateOnly d => ParseValue(d),
            DateTime dt => ParseValue(DateOnly.FromDateTime(dt)),
            _ => throw new SerializationException(
                "The result value is not a valid DateOnly representation.",
                this
            ),
        };
    }

    /// <summary>
    /// Try to serialize runtime value to result value.
    /// </summary>
    public override bool TrySerialize(object? runtimeValue, out object? resultValue)
    {
        resultValue = runtimeValue;
        if (runtimeValue is null)
        {
            return true;
        }

        if (runtimeValue is DateOnly t)
        {
            resultValue = new DateTimeValueNode(t).Value;
            return true;
        }

        if (runtimeValue is DateTimeValueNode d)
        {
            resultValue = d.Value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to deserialize result value to runtime value.
    /// </summary>
    public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
    {
        runtimeValue = resultValue;
        if (resultValue is null)
        {
            return true;
        }

        if (resultValue is string s)
        {
            resultValue = new DateTimeValueNode(s);
        }

        if (resultValue is DateOnly)
        {
            runtimeValue = resultValue;
            return true;
        }

        if (resultValue is DateTimeValueNode dt && dt.TryToDateOnly(out var d))
        {
            runtimeValue = d;
            return true;
        }

        return false;
    }
}
