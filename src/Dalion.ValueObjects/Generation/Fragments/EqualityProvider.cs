using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class EqualityProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return config.UnderlyingType.SpecialType == SpecialType.System_String
            ? GetForString(config).Trim()
            : GetForValueType(config).Trim();
    }

    private static string GetForValueType(AttributeConfiguration config)
    {
        return $@"
        /// <inheritdoc />
        public bool Equals({config.TypeName}? other)
        {{
            if (other is null) return false;

            if (!other.Value.IsInitialized())
            {{
                return !IsInitialized();
            }}

            if (other.Value.IsInitialized() != IsInitialized())
            {{
                return false;
            }}
        
            return EqualityComparer<{config.UnderlyingTypeName}>.Default.Equals(this._value, other.Value.Value);
        }}

        /// <inheritdoc />
        public bool Equals({config.TypeName} other)
        {{
            if (!other.IsInitialized())
            {{
                return !IsInitialized();
            }}

            if (other.IsInitialized() != IsInitialized())
            {{
                return false;
            }}
        
            return EqualityComparer<{config.UnderlyingTypeName}>.Default.Equals(this._value, other.Value);
        }}
        
        /// <inheritdoc />
        public bool Equals({config.TypeName}? other, IEqualityComparer<{config.TypeName}> comparer)
        {{
            if (other is null) return false;
            return comparer.Equals(this, other.Value);
        }}
        
        /// <inheritdoc />
        public override int GetHashCode() {{
            if (!IsInitialized()) return 0;
            return EqualityComparer<{config.UnderlyingTypeName}>.Default.GetHashCode(this._value);
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
        public bool Equals({config.TypeName}? other)
        {{
            if (other is null) return false;

            if (!other.Value.IsInitialized())
            {{
                return !IsInitialized();
            }}

            if (other.Value.IsInitialized() != IsInitialized())
            {{
                return false;
            }}
        
            return other.Value._isNullOrEmpty
                ? this._isNullOrEmpty
                : {config.UnderlyingTypeName}.Equals(this._value, other.Value.Value, System.StringComparison.{stringComparison});
        }}

        /// <inheritdoc />
        public bool Equals({config.TypeName} other)
        {{
            if (!other.IsInitialized())
            {{
                return !IsInitialized();
            }}

            if (other.IsInitialized() != IsInitialized())
            {{
                return false;
            }}
        
            return other._isNullOrEmpty
                ? this._isNullOrEmpty
                : {config.UnderlyingTypeName}.Equals(this._value, other.Value, System.StringComparison.{stringComparison});
        }}
        
        /// <inheritdoc />
        public bool Equals({config.TypeName}? other, IEqualityComparer<{config.TypeName}> comparer)
        {{
            if (other is null) return false;
            return comparer.Equals(this, other.Value);
        }}
        
        /// <inheritdoc />
        public override int GetHashCode() {{
            if (!IsInitialized()) return 0;
            return StringComparer.{stringComparison}.GetHashCode(this._value);
        }}";
    }
}
