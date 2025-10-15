using System.Linq;
using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class PreSetValueCacheProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        var preSetValues = target
            .SymbolInformation.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f =>
                f.IsStatic
                && f.DeclaredAccessibility == Accessibility.Public
                && SymbolEqualityComparer.Default.Equals(f.Type, target.SymbolInformation)
            )
            .ToList();

        return $@"
private static class {config.TypeName}PreSetValueCache {{
    public static readonly Dictionary<{config.UnderlyingTypeName}, {config.TypeName}> {config.TypeName}PreSetValues = new();

    static {config.TypeName}PreSetValueCache()
    {{
        {config.TypeName}PreSetValues[{config.TypeName}.{config.EmptyValueName}.Value] = {config.TypeName}.{config.EmptyValueName};
{
    string.Join("\n", preSetValues.Select(f => $"        {config.TypeName}PreSetValues[{config.TypeName}.{f.Name}.Value] = {config.TypeName}.{f.Name};"))
}
    }}
}}";
    }
}
