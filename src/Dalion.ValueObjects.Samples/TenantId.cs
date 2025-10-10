namespace Dalion.ValueObjects.Samples;

/// <summary>
///     An identifier of a tenant.
/// </summary>
[ValueObject<Guid>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.None,
    comparison: ComparisonGeneration.Omit,
    underlyingTypeEqualityGeneration: UnderlyingTypeEqualityGeneration.GenerateMethods
)]
public readonly partial record struct TenantId;