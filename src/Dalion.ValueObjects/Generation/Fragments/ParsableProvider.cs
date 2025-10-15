using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class ParsableProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (config.ParsableGeneration == ParsableGeneration.Omit)
        {
            return null;
        }

        return config.UnderlyingType.SpecialType == SpecialType.System_String
            ? GetForString(config).Trim()
            : GetForValueType(config).Trim();
    }

    private static string GetForValueType(AttributeConfiguration config)
    {
        return $@"
        /// <inheritdoc />
        public static {config.TypeName} Parse(string s, IFormatProvider? provider)
        {{
            var v = {config.UnderlyingTypeName}.Parse(s, provider);
            return From(v);
        }}

        /// <inheritdoc />
        public static bool TryParse(
            string? s,
            IFormatProvider? provider,
            out {config.TypeName} result
        )
        {{
            try
            {{
                var v = s == null ? default : {config.UnderlyingTypeName}.Parse(s, provider);
                return TryFrom(v, out result);
            }}
            catch (ArgumentException)
            {{
                result = default;
                return false;
            }}
            catch (FormatException)
            {{
                result = default;
                return false;
            }}
        }}

        /// <inheritdoc />
        public static {config.TypeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {{
            var v = {config.UnderlyingTypeName}.Parse(s, provider);
            return From(v);
        }}

        /// <inheritdoc />
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out {config.TypeName} result)
        {{
            try
            {{
                var v = {config.UnderlyingTypeName}.Parse(new string(s), provider);
                return TryFrom(v, out result);
            }}
            catch (ArgumentException)
            {{
                result = default;
                return false;
            }}
            catch (FormatException)
            {{
                result = default;
                return false;
            }}
        }}

        /// <inheritdoc />
        public static {config.TypeName} Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        {{
            var s = System.Text.Encoding.UTF8.GetString(utf8Text);
            var v = {config.UnderlyingTypeName}.Parse(s, provider);
            return From(v);
        }}

        /// <inheritdoc />
        public static bool TryParse(
            ReadOnlySpan<byte> utf8Text,
            IFormatProvider? provider,
            out {config.TypeName} result
        )
        {{
            try
            {{
                var s = System.Text.Encoding.UTF8.GetString(utf8Text);
                var v = {config.UnderlyingTypeName}.Parse(s, provider);
                return TryFrom(v, out result);
            }}
            catch (ArgumentException)
            {{
                result = default;
                return false;
            }}
            catch (FormatException)
            {{
                result = default;
                return false;
            }}
        }}";
    }

    private static string GetForString(AttributeConfiguration config)
    {
        return $@"
        /// <inheritdoc />
        public static {config.TypeName} Parse(string s, IFormatProvider? provider)
        {{
            return From(s);
        }}

        /// <inheritdoc />
        public static bool TryParse(
            string? s,
            IFormatProvider? provider,
            out {config.TypeName} result
        )
        {{
            return TryFrom(s, out result);
        }}

        /// <inheritdoc />
        public static {config.TypeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {{
            return From(new string(s));
        }}

        /// <inheritdoc />
        public static bool TryParse(
            ReadOnlySpan<char> s,
            IFormatProvider? provider,
            out {config.TypeName} result
        )
        {{
            return TryFrom(new string(s), out result);
        }}

        /// <inheritdoc />
        public static {config.TypeName} Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        {{
            var s = System.Text.Encoding.UTF8.GetString(utf8Text);
            return From(s);
        }}

        /// <inheritdoc />
        public static bool TryParse(
            ReadOnlySpan<byte> utf8Text,
            IFormatProvider? provider,
            out {config.TypeName} result
        )
        {{
            try
            {{
                var s = System.Text.Encoding.UTF8.GetString(utf8Text);
                return TryFrom(s, out result);
            }}
            catch (ArgumentException)
            {{
                result = default;
                return false;
            }}
        }}";
    }
}
