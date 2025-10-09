using System;

namespace Dalion.ValueObjects;

internal class AttributeConfiguration
{
    public AttributeConfiguration(
        Type underlyingType,
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toPrimitiveCasting = CastOperator.None,
        CastOperator fromPrimitiveCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive
    )
    {
        UnderlyingType = underlyingType;
        Comparison = comparison;
        ToPrimitiveCasting = toPrimitiveCasting;
        FromPrimitiveCasting = fromPrimitiveCasting;
        CaseSensitivity = stringCaseSensitivity;
    }

    public Type UnderlyingType { get; }
    public ComparisonGeneration Comparison { get; }
    public CastOperator ToPrimitiveCasting { get; }
    public CastOperator FromPrimitiveCasting { get; }
    public StringCaseSensitivity CaseSensitivity { get; }
}