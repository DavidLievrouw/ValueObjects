namespace Dalion.ValueObjects.Samples;

/// <summary>
///     The current player level.
/// </summary>
[ValueObject<int>(
    comparison: ComparisonGeneration.Omit,
    emptyValueName: "Unspecified",
    fluentValidationExtensionsGeneration: FluentValidationExtensionsGeneration.GenerateAll,
    parsableGeneration: ParsableGeneration.Omit
)]
public readonly partial record struct PlayerLevel
{
    public static readonly PlayerLevel Invalid = new(0);
    
    private static Validation Validate(int input)
    {
        return input switch
        {
            0 => Validation.Invalid("Player level must be specified."),
            < 0 => Validation.Invalid("Player level cannot be negative."),
            _ => Validation.Ok,
        };
    }
}
