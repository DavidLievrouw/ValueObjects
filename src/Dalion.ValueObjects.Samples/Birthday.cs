namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A birthday.
/// </summary>
[ValueObject<DateOnly>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.Explicit,
    emptyValueName: "None",
    underlyingTypeEqualityGeneration: UnderlyingTypeEqualityGeneration.GenerateOperatorsAndMethods,
    fluentValidationExtensionsGeneration: FluentValidationExtensionsGeneration.GenerateAll
)]
public readonly partial record struct Birthday
{
    public static readonly Birthday Patrick = new(new DateOnly(1976, 9, 13));
    public static readonly Birthday Sandra = new(new DateOnly(1980, 5, 25));
    public static readonly Birthday InvalidFuture = new(new DateOnly(3000, 1, 1));

    private static Validation Validate(DateOnly input)
    {
        if (input <= DateOnly.MinValue)
        {
            return Validation.Invalid(
                $"Birthday must be initialized (greater than {DateOnly.MinValue.ToString("yyyy-MM-dd")})."
            );
        }

        if (input >= DateOnly.FromDateTime(DateTime.Today))
        {
            return Validation.Invalid("Birthday must be in the past.");
        }

        return Validation.Ok;
    }
}