using System;

namespace Dalion.ValueObjects.Generation;

internal class AttributeConfiguration
{
    public AttributeConfiguration(
        Type underlyingType,
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toPrimitiveCasting = CastOperator.None,
        CastOperator fromPrimitiveCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
        PrimitiveEqualityGeneration primitiveEqualityGeneration = PrimitiveEqualityGeneration.GenerateOperatorsAndMethods,
        string emptyValueName = "Empty"
    )
    {
        UnderlyingType = underlyingType;
        Comparison = comparison;
        ToPrimitiveCasting = toPrimitiveCasting;
        FromPrimitiveCasting = fromPrimitiveCasting;
        CaseSensitivity = stringCaseSensitivity;
        PrimitiveEqualityGeneration = primitiveEqualityGeneration;
        EmptyValueName = emptyValueName;
    }

    public Type UnderlyingType { get; }
    public ComparisonGeneration Comparison { get; }
    public CastOperator ToPrimitiveCasting { get; }
    public CastOperator FromPrimitiveCasting { get; }
    public StringCaseSensitivity CaseSensitivity { get; }
    public PrimitiveEqualityGeneration PrimitiveEqualityGeneration { get; }
    public string EmptyValueName { get; }
}