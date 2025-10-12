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
    public async Task AllowsCreationUsingFrom()
    {
        await Run<DoNotUseNewAnalyzer>(
            "var actual = Dalion.ValueObjects.Samples.Celsius.From(24.4m);"
        );
    }
}