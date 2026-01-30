using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Represents a GraphQL scalar type for unsigned 16-bit integers (ushort/UInt16).
/// This type properly handles unsigned short integer values without unsafe conversions.
/// </summary>
public class UShortType : IntegerTypeBase<ushort>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UShortType"/> class with min/max bounds.
    /// </summary>
    public UShortType(ushort min, ushort max)
        : this("UShort", "Unsigned 16-bit integer (UInt16)", min, max, BindingBehavior.Implicit) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UShortType"/> class with full configuration.
    /// </summary>
    public UShortType(
        string name,
        string? description = null,
        ushort min = ushort.MinValue,
        ushort max = ushort.MaxValue,
        BindingBehavior bind = BindingBehavior.Explicit
    )
        : base(name, min, max, bind)
    {
        Description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UShortType"/> class with default bounds.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public UShortType()
        : this(ushort.MinValue, ushort.MaxValue) { }

    /// <summary>
    /// Parses a GraphQL integer literal to a ushort value.
    /// </summary>
    protected override ushort ParseLiteral(IntValueNode valueSyntax)
    {
        return valueSyntax.ToUInt16();
    }

    /// <summary>
    /// Converts a ushort runtime value to a GraphQL integer literal.
    /// </summary>
    protected override IntValueNode ParseValue(ushort runtimeValue)
    {
        return new IntValueNode(runtimeValue);
    }
}
