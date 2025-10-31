using Xunit;

namespace Dalion.ValueObjects.Rules;

public class UnsupportedUnderlyingTypeAnalyzerTests : RuleTests
{
    [Theory]
    [InlineData("Guid")]
    [InlineData("System.Guid")]
    [InlineData("string")]
    [InlineData("System.String")]
    [InlineData("int")]
    [InlineData("System.Int32")]
    [InlineData("long")]
    [InlineData("System.Int64")]
    [InlineData("double")]
    [InlineData("System.Double")]
    [InlineData("float")]
    [InlineData("System.Single")]
    [InlineData("decimal")]
    [InlineData("System.Decimal")]
    [InlineData("byte")]
    [InlineData("System.Byte")]
    [InlineData("short")]
    [InlineData("System.Int16")]
    [InlineData("char")]
    [InlineData("System.Char")]
    [InlineData("DateTime")]
    [InlineData("System.DateTime")]
    [InlineData("DateTimeOffset")]
    [InlineData("System.DateTimeOffset")]
    [InlineData("TimeSpan")]
    [InlineData("System.TimeSpan")]
    [InlineData("TimeOnly")]
    [InlineData("System.TimeOnly")]
    public async Task AllowsSupportedTypes(string underlyingType)
    {
        await Declare<UnsupportedUnderlyingTypeAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<{underlyingType}>]
public readonly partial record struct IdempotencyId;
"
        );
    }

    [Fact]
    public async Task DoesNotAllowUnsupportedTypes()
    {
        await Declare<UnsupportedUnderlyingTypeAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<[|System.Uri|]>]
public readonly partial record struct IdempotencyId;
"
        );
    }

    [Fact]
    public async Task DoesNotAllowNullableValueTypes()
    {
        await Declare<UnsupportedUnderlyingTypeAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<[|int?|]>]
public readonly partial record struct IdempotencyId;
"
        );
    }

    [Theory]
    [InlineData("bool")]
    [InlineData("System.Boolean")]
    public async Task DoesNotAllowBoolTypes(string underlyingType)
    {
        await Declare<UnsupportedUnderlyingTypeAnalyzer>(
            $@"
{GetAttributesForTest()}

[ValueObject<[|{underlyingType}|]>]
public readonly partial record struct IdempotencyId;
"
        );
    }
}