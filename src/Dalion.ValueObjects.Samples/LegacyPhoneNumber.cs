namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A legacy phone number.
/// </summary>
[ValueObject<string>(
    fromPrimitiveCasting: CastOperator.Implicit,
    toPrimitiveCasting: CastOperator.Implicit,
    comparison: ComparisonGeneration.Omit
)]
public readonly partial record struct LegacyPhoneNumber;