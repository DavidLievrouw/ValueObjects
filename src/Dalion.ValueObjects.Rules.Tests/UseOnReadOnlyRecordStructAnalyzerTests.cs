using Xunit;

namespace Dalion.ValueObjects.Rules;

public class UseOnReadOnlyRecordStructAnalyzerTests : RuleTests
{
    [Fact]
    public async Task RequiresReadOnly()
    {
        await Declare<UseOnReadOnlyRecordStructAnalyzer>($@"
{GetAttributesForTest()}

[ValueObject<System.Guid>]
public partial record struct [|IdempotencyId|];
");
    }

    [Fact]
    public async Task RequiresRecord()
    {
        await Declare<UseOnReadOnlyRecordStructAnalyzer>($@"
{GetAttributesForTest()}

[ValueObject<System.Guid>]
public partial struct [|IdempotencyId|];
");
    }

    [Fact]
    public async Task AllowsSpecifyingOnReadOnlyRecordStruct()
    {
        await Declare<UseOnReadOnlyRecordStructAnalyzer>($@"
{GetAttributesForTest()}

[ValueObject<System.Guid>]
public readonly partial record struct IdempotencyId;
");
    }
}