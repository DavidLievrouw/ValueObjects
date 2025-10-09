using System;

namespace Dalion.ValueObjects;

/// <inheritdoc />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ValueObjectAttribute<T> : ValueObjectAttribute
{
    /// <summary>
    ///     Configures aspects of this individual value object.
    /// </summary>
    /// <param name="comparison">
    ///     Species which comparison code is generated—defaults to
    ///     <see cref="ComparisonGeneration.UseUnderlying" /> which hoists any IComparable implementations from the primitive.
    /// </param>
    /// <param name="toPrimitiveCasting">
    ///     Controls how cast operators are generated for casting from the Value Object to the primitive.
    ///     Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't
    ///     recommended.
    /// </param>
    /// <param name="fromPrimitiveCasting">
    ///     Controls how cast operators are generated for casting from the primitive to the Value Object.
    ///     Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't
    ///     recommended.
    /// </param>
    /// <param name="stringCaseSensitivity">
    ///     When using a <see cref="string" /> as a backing value, controls whether comparisons are case sensitive or not.
    ///     Defaults to <see cref="StringCaseSensitivity.CaseSensitive" />.
    /// </param>
    /// <param name="primitiveEqualityGeneration">
    ///     Specifies whether to generate primitive comparison operators, allowing this type to be compared for equality to the
    ///     primitive.
    ///     Defaults to <see cref="PrimitiveEqualityGeneration.GenerateOperatorsAndMethods" />
    /// </param>
    public ValueObjectAttribute(
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toPrimitiveCasting = CastOperator.None,
        CastOperator fromPrimitiveCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
        PrimitiveEqualityGeneration primitiveEqualityGeneration =
            PrimitiveEqualityGeneration.GenerateOperatorsAndMethods
    )
        : base(
            typeof(T),
            comparison,
            toPrimitiveCasting,
            fromPrimitiveCasting,
            stringCaseSensitivity,
            primitiveEqualityGeneration
        )
    {
    }
}

/// <summary>
///     Marks a type as a Value Object. The type that this is applied to should be partial so that the
///     source generator can augment it with equality, creation barriers, and any conversions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ValueObjectAttribute : Attribute
{
    /// <summary>
    ///     Configures aspects of this individual value object.
    /// </summary>
    /// <param name="underlyingType">The type of the primitive that is being wrapped—defaults to int.</param>
    /// <param name="comparison">
    ///     Species which comparison code is generated—defaults to
    ///     <see cref="ComparisonGeneration.UseUnderlying" /> which hoists any IComparable implementations from the primitive.
    /// </param>
    /// <param name="toPrimitiveCasting">
    ///     Specifies the type of casting from wrapper to primitive - defaults to
    ///     <see cref="CastOperator.Explicit" />.
    /// </param>
    /// <param name="fromPrimitiveCasting">
    ///     Specifies the type of casting from primitive to wrapper - default to
    ///     <see cref="CastOperator.Explicit" />.
    /// </param>
    /// <param name="stringCaseSensitivity">
    ///     When using a <see cref="string" /> as a backing value, controls whether comparisons are case-sensitive or not.
    ///     Defaults to <see cref="StringCaseSensitivity.CaseSensitive" />.
    /// </param>
    /// <param name="primitiveEqualityGeneration">
    ///     Specifies whether to generate primitive comparison operators, allowing this type to be compared for equality to the
    ///     primitive.
    ///     Defaults to <see cref="PrimitiveEqualityGeneration.GenerateOperatorsAndMethods" />
    /// </param>
    public ValueObjectAttribute(
        Type? underlyingType = null!,
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toPrimitiveCasting = CastOperator.None,
        CastOperator fromPrimitiveCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
        PrimitiveEqualityGeneration primitiveEqualityGeneration =
            PrimitiveEqualityGeneration.GenerateOperatorsAndMethods
    )
    {
    }
}