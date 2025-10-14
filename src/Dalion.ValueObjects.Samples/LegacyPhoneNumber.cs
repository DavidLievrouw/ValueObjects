namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A legacy phone number.
/// </summary>
[ValueObject<string>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.Implicit,
    comparison: ComparisonGeneration.Omit,
    fluentValidationExtensionsGeneration: FluentValidationExtensionsGeneration.GenerateMustBeInitialized,
    parsableGeneration: ParsableGeneration.Omit,
    underlyingTypeCreationMethodGeneration: UnderlyingTypeCreationMethodGeneration.Generate
)]
public readonly partial record struct LegacyPhoneNumber;
