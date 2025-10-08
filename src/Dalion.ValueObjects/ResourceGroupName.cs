namespace Dalion.ValueObjects;

/// <summary>
///     The name of an Azure Resource Group.
/// </summary>
[ValueObject<string>(
    fromPrimitiveCasting: CastOperator.Explicit,
    toPrimitiveCasting: CastOperator.Implicit,
    comparison: ComparisonGeneration.UseUnderlying,
    stringCaseSensitivity: StringCaseSensitivity.CaseInsensitive
)]
public readonly partial record struct ResourceGroupName;