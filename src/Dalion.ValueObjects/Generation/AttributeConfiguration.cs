using System;

namespace Dalion.ValueObjects.Generation;

internal class AttributeConfiguration
{
    public AttributeConfiguration(
        Type underlyingType,
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toUnderlyingTypeCasting = CastOperator.None,
        CastOperator fromUnderlyingTypeCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
        UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration = UnderlyingTypeEqualityGeneration.GenerateOperatorsAndMethods,
        string emptyValueName = "Empty"
    )
    {
        UnderlyingType = underlyingType;
        Comparison = comparison;
        ToUnderlyingTypeCasting = toUnderlyingTypeCasting;
        FromUnderlyingTypeCasting = fromUnderlyingTypeCasting;
        CaseSensitivity = stringCaseSensitivity;
        UnderlyingTypeEqualityGeneration = underlyingTypeEqualityGeneration;
        EmptyValueName = emptyValueName;
    }

    public Type UnderlyingType { get; }
    public ComparisonGeneration Comparison { get; }
    public CastOperator ToUnderlyingTypeCasting { get; }
    public CastOperator FromUnderlyingTypeCasting { get; }
    public StringCaseSensitivity CaseSensitivity { get; }
    public UnderlyingTypeEqualityGeneration UnderlyingTypeEqualityGeneration { get; }
    public string EmptyValueName { get; }
}