namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A temperature in Celsius.
/// </summary>
[ValueObject<decimal>(
    fromPrimitiveCasting: CastOperator.Explicit,
    toPrimitiveCasting: CastOperator.Explicit
)]
public readonly partial record struct Celsius
{
    private const decimal AbsoluteZero = -273.15m;

    private static Validation Validate(decimal input)
    {
        return input >= AbsoluteZero
            ? Validation.Ok
            : Validation.Invalid($"Temperature cannot be below absolute zero ({AbsoluteZero}°C).");
    }
}
