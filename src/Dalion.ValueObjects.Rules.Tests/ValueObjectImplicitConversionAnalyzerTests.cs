using Xunit;

namespace Dalion.ValueObjects.Rules;

public class ValueObjectImplicitConversionAnalyzerTests : RuleTests
{
    [Fact]
    public async Task AllowsToImplicitOnly()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>(
    toUnderlyingTypeCasting: CastOperator.Implicit
)]
public readonly partial record struct IdempotencyId;
"
        );
    }
    
    [Fact]
    public async Task AllowsToImplicitAndFromExplicit()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>(
    fromUnderlyingTypeCasting: CastOperator.Explicit,
    toUnderlyingTypeCasting: CastOperator.Implicit
)]
public readonly partial record struct IdempotencyId;
"
        );
    }
    
    [Fact]
    public async Task AllowsToImplicitAndFromNone()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>(
    fromUnderlyingTypeCasting: CastOperator.None,
    toUnderlyingTypeCasting: CastOperator.Implicit
)]
public readonly partial record struct IdempotencyId;
"
        );
    }

    [Fact]
    public async Task AllowsFromImplicitOnly()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>(
    fromUnderlyingTypeCasting: CastOperator.Implicit
)]
public readonly partial record struct IdempotencyId;
"
        );
    }

    [Fact]
    public async Task AllowsFromImplicitAndToExplicit()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>(
    fromUnderlyingTypeCasting: CastOperator.Implicit,
    toUnderlyingTypeCasting: CastOperator.Explicit
)]
public readonly partial record struct IdempotencyId;
"
        );
    }

    [Fact]
    public async Task AllowsFromImplicitAndToNone()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>(
    fromUnderlyingTypeCasting: CastOperator.Implicit,
    toUnderlyingTypeCasting: CastOperator.None
)]
public readonly partial record struct IdempotencyId;
"
        );
    }

    [Fact]
    public async Task DoesNotAllowTwoWayImplicit()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>(
    fromUnderlyingTypeCasting: CastOperator.Implicit,
    toUnderlyingTypeCasting: CastOperator.Implicit
)]
public readonly partial record struct [|IdempotencyId|];
"
        );
    }

    [Fact]
    public async Task AllowsOmitting()
    {
        await Declare<ValueObjectImplicitConversionAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<System.Guid>]
public readonly partial record struct IdempotencyId;
"
        );
    }
}
