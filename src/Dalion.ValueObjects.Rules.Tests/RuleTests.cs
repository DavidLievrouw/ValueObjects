using Dalion.ValueObjects.Samples;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Dalion.ValueObjects.Rules;

public abstract class RuleTests
{
    private static readonly string SamplesAssemblyLocation = typeof(Celsius).Assembly.Location;

    protected async Task Run(string source, params DiagnosticResult[] expected)
    {
        var context = new CSharpAnalyzerTest<DoNotUseNewAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90.AddAssemblies(
            [
                "Dalion.ValueObjects",
                SamplesAssemblyLocation.Replace(".dll", string.Empty),
            ]),
            TestCode = $@"
public class Program {{
    public void Main() {{
        {source}
    }}
}}",
        };

        context.ExpectedDiagnostics.AddRange(expected);

        await context.RunAsync(TestContext.Current.CancellationToken);
    }
}