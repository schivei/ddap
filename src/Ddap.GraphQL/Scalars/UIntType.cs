using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Represents a GraphQL scalar type for unsigned 32-bit integers (uint/UInt32).
/// This type properly handles unsigned integer values without unsafe conversions.
/// </summary>
public class UIntType : IntegerTypeBase<uint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIntType"/> class with min/max bounds.
    /// </summary>
    public UIntType(uint min, uint max)
        : this("UInt", "Unsigned 32-bit integer (UInt32)", min, max, BindingBehavior.Implicit) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIntType"/> class with full configuration.
    /// </summary>
    public UIntType(
        string name,
        string? description = null,
        uint min = uint.MinValue,
        uint max = uint.MaxValue,
        BindingBehavior bind = BindingBehavior.Explicit
    )
        : base(name, min, max, bind)
    {
        Description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIntType"/> class with default bounds.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public UIntType()
        : this(uint.MinValue, uint.MaxValue) { }

    /// <summary>
    /// Parses a GraphQL integer literal to a uint value.
    /// </summary>
    protected override uint ParseLiteral(IntValueNode valueSyntax)
    {
        return valueSyntax.ToUInt32();
    }

    /// <summary>
    /// Converts a uint runtime value to a GraphQL integer literal.
    /// </summary>
    protected override IntValueNode ParseValue(uint runtimeValue)
    {
        return new IntValueNode(runtimeValue);
    }
}
