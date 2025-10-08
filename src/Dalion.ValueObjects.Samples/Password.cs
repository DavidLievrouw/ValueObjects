namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A secure password.
/// </summary>
[ValueObject<string>(
    fromPrimitiveCasting: CastOperator.None,
    toPrimitiveCasting: CastOperator.Explicit,
    comparison: ComparisonGeneration.Omit,
    stringCaseSensitivity: StringCaseSensitivity.CaseSensitive
)]
public readonly partial record struct Password;