namespace Dalion.ValueObjects.Samples;

/// <summary>
///     An identifier of a tenant.
/// </summary>
[ValueObject<Guid>(
    fromPrimitiveCasting: CastOperator.Explicit,
    toPrimitiveCasting: CastOperator.None,
    comparison: ComparisonGeneration.Omit,
    primitiveEqualityGeneration: PrimitiveEqualityGeneration.GenerateMethods
)]
public readonly partial record struct TenantId;