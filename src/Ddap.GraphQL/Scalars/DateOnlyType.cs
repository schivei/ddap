using HotChocolate.Language;
using HotChocolate.Types;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Scalar type for DateOnly values in GraphQL.
/// Maps DateOnly to a string in ISO 8601 format (yyyy-MM-dd).
/// </summary>
public class DateOnlyType : ScalarType<DateOnly, StringValueNode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateOnlyType"/> class.
    /// </summary>
    public DateOnlyType()
        : base("DateOnly", BindingBehavior.Implicit)
    {
        Description = "Represents a date without time information in ISO 8601 format (yyyy-MM-dd).";
    }

    /// <summary>
    /// Parse the literal value node.
    /// </summary>
    protected override DateOnly ParseLiteral(StringValueNode valueSyntax)
    {
        if (DateOnly.TryParse(valueSyntax.Value, out var dateOnly))
        {
            return dateOnly;
        }

        throw new SerializationException(
            $"Unable to deserialize string to DateOnly: {valueSyntax.Value}",
            this
        );
    }

    /// <summary>
    /// Serialize the value.
    /// </summary>
    protected override StringValueNode ParseValue(DateOnly runtimeValue)
    {
        return new StringValueNode(runtimeValue.ToString("yyyy-MM-dd"));
    }

    /// <summary>
    /// Check if the value syntax is valid.
    /// </summary>
    public override IValueNode ParseResult(object? resultValue)
    {
        return resultValue switch
        {
            null => NullValueNode.Default,
            string s => new StringValueNode(s),
            DateOnly d => new StringValueNode(d.ToString("yyyy-MM-dd")),
            _ => throw new SerializationException(
                $"Unable to serialize {resultValue.GetType().Name} to DateOnly",
                this
            ),
        };
    }
}
