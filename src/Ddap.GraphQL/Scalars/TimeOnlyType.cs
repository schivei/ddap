using HotChocolate.Language;
using HotChocolate.Types;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Scalar type for TimeOnly values in GraphQL.
/// Maps TimeOnly to a string in ISO 8601 format (HH:mm:ss.fffffff).
/// </summary>
public class TimeOnlyType : ScalarType<TimeOnly, StringValueNode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeOnlyType"/> class.
    /// </summary>
    public TimeOnlyType()
        : base("TimeOnly", BindingBehavior.Implicit)
    {
        Description = "Represents a time without date information in ISO 8601 format (HH:mm:ss).";
    }

    /// <summary>
    /// Parse the literal value node.
    /// </summary>
    protected override TimeOnly ParseLiteral(StringValueNode valueSyntax)
    {
        if (TimeOnly.TryParse(valueSyntax.Value, out var timeOnly))
        {
            return timeOnly;
        }

        throw new SerializationException(
            $"Unable to deserialize string to TimeOnly: {valueSyntax.Value}",
            this
        );
    }

    /// <summary>
    /// Serialize the value.
    /// </summary>
    protected override StringValueNode ParseValue(TimeOnly runtimeValue)
    {
        return new StringValueNode(runtimeValue.ToString("HH:mm:ss.fffffff"));
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
            TimeOnly t => new StringValueNode(t.ToString("HH:mm:ss.fffffff")),
            _ => throw new SerializationException(
                $"Unable to serialize {resultValue.GetType().Name} to TimeOnly",
                this
            ),
        };
    }
}
