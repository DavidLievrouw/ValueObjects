using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class ToStringProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return config.UnderlyingType.SpecialType == SpecialType.System_String
            ? GetStringToString()
            : GetFormattableToString(target);
    }

    private string GetFormattableToString(GenerationTarget target)
    {
        if (target.IsFormattable())
        {
            return @"
                /// <inheritdoc />
                public override string ToString()
                {
                    return Value.ToString();
                }

                /// <inheritdoc cref=""M:System.String.ToString(System.IFormatProvider)"" />
                public string ToString(IFormatProvider? provider)
                {
                    return Value.ToString(format: null, provider: provider) ?? """";
                }

                /// <inheritdoc />
                public string ToString(string? format, IFormatProvider? formatProvider)
                {{
                    return Value.ToString(format, formatProvider) ?? """";
                }}";
        }

        return @"
                /// <inheritdoc />
                public override string ToString()
                {
                    return Value.ToString();
                }";
    }

    private string GetStringToString()
    {
        return @"
                /// <inheritdoc />
                public override string ToString()
                {{
                    return Value;
                }}

                /// <inheritdoc cref=""M:System.String.ToString(System.IFormatProvider)"" />
                public string ToString(IFormatProvider? provider)
                {{
                    return Value.ToString(provider: provider);
                }}
";
    }
}
