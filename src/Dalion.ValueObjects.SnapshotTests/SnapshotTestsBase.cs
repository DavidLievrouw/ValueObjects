using System.Text.RegularExpressions;
using Dalion.ValueObjects.Generation;

namespace Dalion.ValueObjects.SnapshotTests;

public abstract partial class SnapshotTestsBase
{
    private static readonly string Namespace = typeof(SnapshotTestsBase).Namespace! + ".Samples";
    private readonly string _typeName;

    protected SnapshotTestsBase(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(typeName));
        }

        _typeName = typeName;
    }

    [Fact]
    public Task VerifyRecord()
    {
        var source = GetType().Assembly.GetEmbeddedResourceString($"{Namespace}.{_typeName}.cs")!;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(v =>
            {
                v.ScrubLinesWithReplace(line =>
                {
                    var trimmedLine = line.Trim();
                    if (
                        trimmedLine.StartsWith(
                            "[GeneratedCodeAttribute(\"System.Text.RegularExpressions.Generator\","
                        ) ||
                        trimmedLine.StartsWith(
                            "[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.Text.RegularExpressions.Generator\",")
                    )
                    {
                        return VersionNumberRegex().Replace(line, "<version>");
                    }

                    return line;
                });
            })
            .Run();
    }

    [GeneratedRegex(@"\d+\.\d+\.\d+\.\d+")]
    private static partial Regex VersionNumberRegex();
}