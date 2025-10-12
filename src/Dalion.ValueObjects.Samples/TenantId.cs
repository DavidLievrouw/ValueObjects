namespace Dalion.ValueObjects.Samples;

/// <summary>
///     An identifier of a tenant.
/// </summary>
[ValueObject<Guid>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.None,
    comparison: ComparisonGeneration.Omit,
    underlyingTypeEqualityGeneration: UnderlyingTypeEqualityGeneration.Omit,
    fluentValidationExtensionsGeneration: FluentValidationExtensionsGeneration.GenerateMustBeInitializedAndValid
)]
public readonly partial record struct TenantId;