namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A legacy phone number.
/// </summary>
[ValueObject<string>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.Implicit,
    comparison: ComparisonGeneration.Omit
)]
public readonly partial record struct LegacyPhoneNumber;