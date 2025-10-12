using Dalion.ValueObjects.Samples;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Dalion.ValueObjects.Rules;

public abstract class RuleTests
{
    private static readonly string SamplesAssemblyLocation = typeof(Celsius).Assembly.Location;

    protected static string GetAttributesForTest()
    {
        var content = GetEmbeddedResourceContent($"{nameof(ValueObjectAttribute)}.cs");
        content = content.Replace("namespace Dalion.ValueObjects;", string.Empty);
        return content;
    }

    private static string GetEmbeddedResourceContent(string resourceName)
    {
        var assembly = typeof(RuleTests).Assembly;
        var fullResourceName =
            assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName))
            ?? throw new InvalidOperationException($"Resource {resourceName} not found.");

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        using var reader = new StreamReader(
            stream
            ?? throw new InvalidOperationException($"Could not load stream for {resourceName}")
        );
        return reader.ReadToEnd();
    }

    protected async Task Run<TAnalyzer>(string source, params DiagnosticResult[] expected)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var context = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90.AddAssemblies(
                ["Dalion.ValueObjects", SamplesAssemblyLocation.Replace(".dll", string.Empty),]
            ),
            TestCode =
                $@"
namespace RuleTests;

public class Program {{
    public void Main() {{
        {source}
    }}
}}",
        };

        context.ExpectedDiagnostics.AddRange(expected);

        await context.RunAsync(TestContext.Current.CancellationToken);
    }

    protected async Task Declare<TAnalyzer>(string source, params DiagnosticResult[] expected)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var context = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90.AddAssemblies(
                ["Dalion.ValueObjects", SamplesAssemblyLocation.Replace(".dll", string.Empty),]
            ),
            TestCode =
                $@"
namespace RuleTests;
{source}",
        };

        context.ExpectedDiagnostics.AddRange(expected);

        await context.RunAsync(TestContext.Current.CancellationToken);
    }
}