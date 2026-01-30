using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Represents a GraphQL scalar type for unsigned 64-bit integers (ulong/UInt64).
/// This type properly handles unsigned long integer values without unsafe conversions.
/// </summary>
public class ULongType : IntegerTypeBase<ulong>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ULongType"/> class with min/max bounds.
    /// </summary>
    public ULongType(ulong min, ulong max)
        : this("ULong", "Unsigned 64-bit integer (UInt64)", min, max, BindingBehavior.Implicit) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ULongType"/> class with full configuration.
    /// </summary>
    public ULongType(
        string name,
        string? description = null,
        ulong min = ulong.MinValue,
        ulong max = ulong.MaxValue,
        BindingBehavior bind = BindingBehavior.Explicit
    )
        : base(name, min, max, bind)
    {
        Description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ULongType"/> class with default bounds.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public ULongType()
        : this(ulong.MinValue, ulong.MaxValue) { }

    /// <summary>
    /// Parses a GraphQL integer literal to a ulong value.
    /// </summary>
    protected override ulong ParseLiteral(IntValueNode valueSyntax)
    {
        return valueSyntax.ToUInt64();
    }

    /// <summary>
    /// Converts a ulong runtime value to a GraphQL integer literal.
    /// </summary>
    protected override IntValueNode ParseValue(ulong runtimeValue)
    {
        return new IntValueNode(runtimeValue);
    }
}
