namespace Dalion.ValueObjects.Samples;

/// <summary>
///     The current player level.
/// </summary>
[ValueObject<int>(
    comparison: ComparisonGeneration.Omit,
    emptyValueName: "Unspecified"
)]
public readonly partial record struct PlayerLevel
{
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
