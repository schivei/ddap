using HotChocolate.Language;
using HotChocolate.Types;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Scalar type for DateTimeOffset values in GraphQL.
/// Maps DateTimeOffset to a string in ISO 8601 format with timezone offset (yyyy-MM-ddTHH:mm:ss.fffzzz).
/// Uses DateTimeValueNode for proper serialization without type inference.
/// </summary>
public class DateTimeOffsetType : ScalarType<DateTimeOffset, DateTimeValueNode>
{
    private const string SpecifiedByUrl = "https://www.graphql-scalars.com/date-time";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeOffsetType"/> class with full configuration.
    /// </summary>
    public DateTimeOffsetType(
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
    /// Initializes a new instance of the <see cref="DateTimeOffsetType"/> class with default configuration.
    /// </summary>
    public DateTimeOffsetType()
        : this(
            "DateTimeOffset",
            "Represents a point in time with timezone offset in ISO 8601 format (yyyy-MM-ddTHH:mm:ss.fffzzz).",
            BindingBehavior.Implicit
        ) { }

    /// <summary>
    /// Parse the literal value node from GraphQL query.
    /// </summary>
    protected override DateTimeOffset ParseLiteral(DateTimeValueNode valueSyntax)
    {
        return valueSyntax.TryToDateTimeOffset(out var value)
            ? value
            : throw new SerializationException(
                "The string value is not a valid DateTimeOffset representation.",
                this
            );
    }

    /// <summary>
    /// Serialize the runtime value to a value node.
    /// </summary>
    protected override DateTimeValueNode ParseValue(DateTimeOffset runtimeValue)
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
            DateTimeOffset dto => ParseValue(dto),
            DateTime dt => ParseValue(new DateTimeOffset(dt)),
            _ => throw new SerializationException(
                "The result value is not a valid DateTimeOffset representation.",
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

        if (runtimeValue is DateTimeOffset dto)
        {
            resultValue = new DateTimeValueNode(dto).Value;
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

        if (resultValue is DateTimeOffset)
        {
            runtimeValue = resultValue;
            return true;
        }

        if (resultValue is DateTimeValueNode dt && dt.TryToDateTimeOffset(out var dto))
        {
            runtimeValue = dto;
            return true;
        }

        return false;
    }
}
