using HotChocolate.Language;
using HotChocolate.Types;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Scalar type for TimeOnly values in GraphQL.
/// Maps TimeOnly to a string in ISO 8601 format (HH:mm:ss.fff).
/// Uses DateTimeValueNode for proper serialization without type inference.
/// </summary>
public class TimeOnlyType : ScalarType<TimeOnly, DateTimeValueNode>
{
    private const string SpecifiedByUrl = "https://www.graphql-scalars.com/time-only";

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeOnlyType"/> class with full configuration.
    /// </summary>
    public TimeOnlyType(
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
    /// Initializes a new instance of the <see cref="TimeOnlyType"/> class with default configuration.
    /// </summary>
    public TimeOnlyType()
        : this(
            "TimeOnly",
            "Represents a time without date information in ISO 8601 format (HH:mm:ss).",
            BindingBehavior.Implicit
        ) { }

    /// <summary>
    /// Parse the literal value node from GraphQL query.
    /// </summary>
    protected override TimeOnly ParseLiteral(DateTimeValueNode valueSyntax)
    {
        return valueSyntax.TryToTimeOnly(out var value)
            ? value
            : throw new SerializationException(
                "The string value is not a valid TimeOnly representation.",
                this
            );
    }

    /// <summary>
    /// Serialize the runtime value to a value node.
    /// </summary>
    protected override DateTimeValueNode ParseValue(TimeOnly runtimeValue)
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
            string s => new StringValueNode(s),
            TimeOnly t => ParseValue(t),
            DateTime dt => ParseValue(TimeOnly.FromDateTime(dt)),
            _ => throw new SerializationException(
                "The result value is not a valid TimeOnly representation.",
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

        if (runtimeValue is TimeOnly t)
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

        if (resultValue is TimeOnly)
        {
            runtimeValue = resultValue;
            return true;
        }

        if (resultValue is DateTimeValueNode dt && dt.TryToTimeOnly(out var t))
        {
            runtimeValue = t;
            return true;
        }

        return false;
    }
}
