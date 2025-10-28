namespace ValueObjectsConsumer;

/// <summary>
///     An identifier of a subscription.
/// </summary>
[ValueObject<Guid>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.None,
    comparison: ComparisonGeneration.Omit,
    underlyingTypeEqualityGeneration: UnderlyingTypeEqualityGeneration.Omit,
    fluentValidationExtensionsGeneration: FluentValidationExtensionsGeneration.GenerateMustBeInitializedAndValid,
    parsableGeneration: ParsableGeneration.Omit
)]
public readonly partial record struct SubscriptionId;

/*
public class ShouldNotCompile
{
    public void Example()
    {
        var s = new SubscriptionId();
    }
}
*/