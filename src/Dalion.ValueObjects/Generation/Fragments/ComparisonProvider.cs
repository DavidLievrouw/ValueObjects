using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class ComparisonProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (config.Comparison == ComparisonGeneration.Omit)
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
                public int CompareTo({config.TypeName} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({config.UnderlyingTypeName} other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {config.TypeName} other1)
                        return this.CompareTo(other1);
                    if (other is {config.UnderlyingTypeName} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {config.TypeName}"",
                        nameof(other)
                    );
                }}";
    }

    private static string GetForString(AttributeConfiguration config)
    {
        return $@"
                public int CompareTo({config.TypeName} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({config.UnderlyingTypeName}? other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {config.TypeName} other1)
                        return this.CompareTo(other1);
                    if (other is {config.UnderlyingTypeName} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {config.TypeName}"",
                        nameof(other)
                    );
                }}";
    }
}
