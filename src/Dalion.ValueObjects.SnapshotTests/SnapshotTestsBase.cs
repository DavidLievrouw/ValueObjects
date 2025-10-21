using Dalion.ValueObjects.Generation;

namespace Dalion.ValueObjects.SnapshotTests;

public abstract class SnapshotTestsBase
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

        return new SnapshotRunner<ValueObjectGenerator>().WithSource(source).Run();
    }
}
