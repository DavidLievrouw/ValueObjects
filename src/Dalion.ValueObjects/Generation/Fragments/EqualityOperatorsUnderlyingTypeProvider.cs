using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class EqualityOperatorsUnderlyingTypeProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (
            (
                config.UnderlyingTypeEqualityGeneration
                & UnderlyingTypeEqualityGeneration.GenerateOperators
            ) != UnderlyingTypeEqualityGeneration.GenerateOperators
        )
        {
            return null;
        }

        return config.UnderlyingType.SpecialType == SpecialType.System_String
            ? GetForString(config)
            : GetForValueType(config);
    }

    private static string GetForValueType(AttributeConfiguration config)
    {
        return $@"
    public static bool operator ==({config.TypeName} left, {config.UnderlyingTypeName} right) => left.Value.Equals(right);

    public static bool operator ==({config.UnderlyingTypeName} left, {config.TypeName} right) => right.Value.Equals(left);

    public static bool operator !=({config.UnderlyingTypeName} left, {config.TypeName} right) => !(left == right);

    public static bool operator !=({config.TypeName} left, {config.UnderlyingTypeName} right) => !(left == right);";
    }

    private static string GetForString(AttributeConfiguration config)
    {
        return $@"
    public static bool operator ==({config.TypeName} left, {config.UnderlyingTypeName}? right) => left.Value.Equals(right);

    public static bool operator ==({config.UnderlyingTypeName}? left, {config.TypeName} right) => right.Value.Equals(left);

    public static bool operator !=({config.UnderlyingTypeName}? left, {config.TypeName} right) => !(left == right);

    public static bool operator !=({config.TypeName} left, {config.UnderlyingTypeName}? right) => !(left == right);";
    }
}
