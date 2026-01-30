using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Represents a GraphQL scalar type for signed 8-bit integers (sbyte/SByte).
/// This type properly handles signed byte values without unsafe conversions.
/// </summary>
public class SByteType : IntegerTypeBase<sbyte>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SByteType"/> class with min/max bounds.
    /// </summary>
    public SByteType(sbyte min, sbyte max)
        : this("SByte", "Signed 8-bit integer (SByte)", min, max, BindingBehavior.Implicit) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SByteType"/> class with full configuration.
    /// </summary>
    public SByteType(
        string name,
        string? description = null,
        sbyte min = sbyte.MinValue,
        sbyte max = sbyte.MaxValue,
        BindingBehavior bind = BindingBehavior.Explicit
    )
        : base(name, min, max, bind)
    {
        Description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SByteType"/> class with default bounds.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public SByteType()
        : this(sbyte.MinValue, sbyte.MaxValue) { }

    /// <summary>
    /// Parses a GraphQL integer literal to an sbyte value.
    /// </summary>
    protected override sbyte ParseLiteral(IntValueNode valueSyntax)
    {
        return valueSyntax.ToSByte();
    }

    /// <summary>
    /// Converts an sbyte runtime value to a GraphQL integer literal.
    /// </summary>
    protected override IntValueNode ParseValue(sbyte runtimeValue)
    {
        return new IntValueNode(runtimeValue);
    }
}
