using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class DefaultValueProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return config.UnderlyingType.SpecialType == SpecialType.System_String
            ? "System.String.Empty"
            : "default";
    }
}