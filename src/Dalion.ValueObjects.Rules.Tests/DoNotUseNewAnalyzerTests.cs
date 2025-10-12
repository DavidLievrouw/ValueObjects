using Xunit;

namespace Dalion.ValueObjects.Rules;

public class DoNotUseNewAnalyzerTests : RuleTests
{
    [Fact]
    public async Task ProhibitsNewingUp()
    {
        await Run("var actual = [|new Dalion.ValueObjects.Samples.Celsius()|];");
    }

    [Fact]
    public async Task AllowsCreationUsingFrom()
    {
        await Run("var actual = Dalion.ValueObjects.Samples.Celsius.From(24.4m);");
    }
}