using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class UnderlyingTypeCreationExtensionsProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (
            config.UnderlyingTypeCreationMethodGeneration
            == UnderlyingTypeCreationMethodGeneration.Omit
        )
        {
            return null;
        }

        var containingTypeNames = target.SymbolInformation.GetContainingTypeNames();
        var containingTypes =
            containingTypeNames == string.Empty ? string.Empty : containingTypeNames + ".";

        var creationMethod =
            config.UnderlyingType.SpecialType == SpecialType.System_String
                ? $@"
                 public static {config.Namespace}.{containingTypes}{config.TypeName} {config.TypeName}(this {config.UnderlyingTypeName}? value)
                 {{
                     return {config.Namespace}.{containingTypes}{config.TypeName}.From(value);
                 }}"
                : $@"
                 public static {config.Namespace}.{containingTypes}{config.TypeName} {config.TypeName}(this {config.UnderlyingTypeName} value)
                 {{
                     return {config.Namespace}.{containingTypes}{config.TypeName}.From(value);
                 }}";

        return $@"
        #nullable enable

        namespace {config.Namespace} {{
            public static class {config.TypeName}UnderlyingTypeCreationExtensions
            {{
                {creationMethod}
            }}
        }}
        ";
    }
}