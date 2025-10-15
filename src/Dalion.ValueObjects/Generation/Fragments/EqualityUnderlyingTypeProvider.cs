using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class EqualityUnderlyingTypeProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (
            (
                config.UnderlyingTypeEqualityGeneration
                & UnderlyingTypeEqualityGeneration.GenerateMethods
            ) != UnderlyingTypeEqualityGeneration.GenerateMethods
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
                /// <inheritdoc />
                public bool Equals({config.UnderlyingTypeName} other)
                {{
                    return EqualityComparer<{config.UnderlyingTypeName}>.Default.Equals(this._value, other);
                }}";
    }

    private static string GetForString(AttributeConfiguration config)
    {
        var stringComparison =
            config.CaseSensitivity == StringCaseSensitivity.CaseInsensitive
                ? "OrdinalIgnoreCase"
                : "Ordinal";

        return $@"
                /// <inheritdoc />
                public bool Equals({config.UnderlyingTypeName}? other)
                {{
                    return {config.UnderlyingTypeName}.IsNullOrEmpty(other)
                        ? this._isNullOrEmpty
                        : {config.UnderlyingTypeName}.Equals(this._value, other, System.StringComparison.{stringComparison});
                }}
            
                public bool Equals({config.UnderlyingTypeName}? underlyingValue, StringComparer comparer)
                {{
                    return comparer.Equals(this.Value, underlyingValue);
                }}";
    }
}
