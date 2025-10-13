using Xunit;

namespace Dalion.ValueObjects.Rules;

public class DoNotUseNewAnalyzerTests : RuleTests
{
    [Fact]
    public async Task ProhibitsNewingUp()
    {
        await Run<DoNotUseNewAnalyzer>(
            "var actual = [|new Dalion.ValueObjects.Samples.Celsius()|];"
        );
    }

    [Fact]
    public async Task AllowsNewingUpInValueObject()
    {
        await Declare<DoNotUseNewAnalyzer>($@"
{GetAttributesForTest()}

[ValueObject<System.Decimal>]
public readonly partial record struct Fahrenheit {{
    private const decimal AbsoluteZeroValue = -459.67m;
    public static readonly Fahrenheit AbsoluteZero = new(AbsoluteZeroValue, validation: false);

    // For testing purposes only (should get generated)
    public Fahrenheit(decimal value, bool validation = true) {{}}
}}
");
    }

    [Fact]
    public async Task AllowsNewingUpInValueObjectWithDefaultConstructor()
    {
        await Declare<DoNotUseNewAnalyzer>($@"
{GetAttributesForTest()}

[ValueObject<System.Decimal>]
public readonly partial record struct Fahrenheit {{
    public static readonly Fahrenheit AbsoluteZero = new();
}}
");
    }

    [Fact]
    public async Task AllowsCreationUsingFrom()
    {
        await Run<DoNotUseNewAnalyzer>(
            "var actual = Dalion.ValueObjects.Samples.Celsius.From(24.4m);"
        );
    }

    [Fact]
    public async Task DoesNotAllowNewingUpNonPublicField()
    {
        await Declare<DoNotUseNewAnalyzer>($@"
{GetAttributesForTest()}

[ValueObject<System.Decimal>]
public readonly partial record struct Fahrenheit {{
    private const decimal AbsoluteZeroValue = -459.67m;
    private static Fahrenheit AbsoluteZero = [|new(AbsoluteZeroValue, validation: false)|];

    // For testing purposes only (should get generated)
    public Fahrenheit(decimal value, bool validation = true) {{}}
}}
");
    }

    [Fact]
    public async Task DoesNotAllowNewingUpOtherValueObjects()
    {
        await Declare<DoNotUseNewAnalyzer>($@"
{GetAttributesForTest()}

[ValueObject<System.Decimal>]
public readonly partial record struct SomethingElse {{
}}

[ValueObject<System.Decimal>]
public readonly partial record struct Fahrenheit {{
    public static readonly SomethingElse CannotDoThis = [|new SomethingElse()|];
}}
");
    }
}