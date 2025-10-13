using System.Globalization;

namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A temperature in Celsius.
/// </summary>
[ValueObject<decimal>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.Explicit,
    emptyValueName: "Zero",
    underlyingTypeEqualityGeneration: UnderlyingTypeEqualityGeneration.GenerateOperatorsAndMethods,
    fluentValidationExtensionsGeneration: FluentValidationExtensionsGeneration.GenerateAll
)]
public readonly partial record struct Celsius
{
    private const decimal AbsoluteZeroValue = -273.15m;
    
    public static readonly Celsius AbsoluteZero = new(AbsoluteZeroValue);

    private static Validation Validate(decimal input)
    {
        return input >= AbsoluteZeroValue
            ? Validation.Ok
            : Validation.Invalid(
                $"Temperature cannot be below absolute zero ({AbsoluteZeroValue.ToString(CultureInfo.InvariantCulture)}°C)."
            );
    }
}