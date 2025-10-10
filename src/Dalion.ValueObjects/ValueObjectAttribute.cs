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
    ///     <see cref="ComparisonGeneration.UseUnderlying" /> which hoists any IComparable implementations from the underlying type.
    /// </param>
    /// <param name="toUnderlyingTypeCasting">
    ///     Controls how cast operators are generated for casting from the Value Object to the underlying type.
    ///     Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't
    ///     recommended.
    /// </param>
    /// <param name="fromUnderlyingTypeCasting">
    ///     Controls how cast operators are generated for casting from the underlying type to the Value Object.
    ///     Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't
    ///     recommended.
    /// </param>
    /// <param name="stringCaseSensitivity">
    ///     When using a <see cref="string" /> as a backing value, controls whether comparisons are case sensitive or not.
    ///     Defaults to <see cref="StringCaseSensitivity.CaseSensitive" />.
    /// </param>
    /// <param name="underlyingTypeEqualityGeneration">
    ///     Specifies whether to generate underlying type comparison operators, allowing this type to be compared for equality to the
    ///     underlying type.
    ///     Defaults to <see cref="UnderlyingTypeEqualityGeneration.GenerateOperatorsAndMethods" />
    /// </param>
    /// <param name="emptyValueName">
    ///     The name of the static property representing an empty value object, if applicable.
    ///     Defaults to "Empty".
    /// </param>
    public ValueObjectAttribute(
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toUnderlyingTypeCasting = CastOperator.None,
        CastOperator fromUnderlyingTypeCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
        UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration =
            UnderlyingTypeEqualityGeneration.GenerateOperatorsAndMethods,
        string emptyValueName = "Empty"
    )
        : base(
            typeof(T),
            comparison,
            toUnderlyingTypeCasting,
            fromUnderlyingTypeCasting,
            stringCaseSensitivity,
            underlyingTypeEqualityGeneration,
            emptyValueName
        ) { }
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
    /// <param name="underlyingType">The type of the underlying value that is being wrapped.</param>
    /// <param name="comparison">
    ///     Species which comparison code is generated—defaults to
    ///     <see cref="ComparisonGeneration.UseUnderlying" /> which hoists any IComparable implementations from the underlying type.
    /// </param>
    /// <param name="toUnderlyingTypeCasting">
    ///     Specifies the type of casting from wrapper to the underlying type - defaults to
    ///     <see cref="CastOperator.Explicit" />.
    /// </param>
    /// <param name="fromUnderlyingTypeCasting">
    ///     Specifies the type of casting from the underlying type to wrapper - default to
    ///     <see cref="CastOperator.Explicit" />.
    /// </param>
    /// <param name="stringCaseSensitivity">
    ///     When using a <see cref="string" /> as a backing value, controls whether comparisons are case-sensitive or not.
    ///     Defaults to <see cref="StringCaseSensitivity.CaseSensitive" />.
    /// </param>
    /// <param name="underlyingTypeEqualityGeneration">
    ///     Specifies whether to generate underlying value comparison operators, allowing this type to be compared for equality to the
    ///     underlying type.
    ///     Defaults to <see cref="UnderlyingTypeEqualityGeneration.GenerateOperatorsAndMethods" />
    /// </param>
    /// <param name="emptyValueName">
    ///     The name of the static property representing an empty value object, if applicable.
    ///     Defaults to "Empty".
    /// </param>
    public ValueObjectAttribute(
        Type? underlyingType = null!,
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toUnderlyingTypeCasting = CastOperator.None,
        CastOperator fromUnderlyingTypeCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
        UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration =
            UnderlyingTypeEqualityGeneration.GenerateOperatorsAndMethods,
        string emptyValueName = "Empty"
    ) { }
}
