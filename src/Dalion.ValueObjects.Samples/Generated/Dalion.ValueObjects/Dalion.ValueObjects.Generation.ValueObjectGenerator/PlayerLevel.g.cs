
#nullable enable

using System;

namespace Dalion.ValueObjects.Samples {
    
    [System.Diagnostics.DebuggerDisplay("PlayerLevel {Value}")]
    [System.Text.Json.Serialization.JsonConverter(typeof(PlayerLevelSystemTextJsonConverter))]
    [System.ComponentModel.TypeConverter(typeof(PlayerLevelTypeConverter))]
    public partial record struct PlayerLevel : IEquatable<PlayerLevel>, IFormattable {
        private readonly System.Int32 _value;
        private readonly bool _initialized;
#pragma warning disable CS0414
        private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
        private readonly Validation _validation;
        private static readonly Type UnderlyingType = typeof(System.Int32);

        /// <summary>
        ///     Gets the underlying value of this <see cref="PlayerLevel"/>.
        /// </summary>
        public System.Int32 Value => _value;

        /// <summary>
        ///     Creates a new <see cref="PlayerLevel"/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public PlayerLevel()
        {
            _value = default;
            _initialized = false;
            _isNullOrEmpty = false;
            _validation ??= Validate(_value);
        }

        /// <summary>
        ///     Creates a new <see cref="PlayerLevel"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private PlayerLevel(System.Int32 value) {
            
            _initialized = true;
            _value = value;
            _isNullOrEmpty = false;
            _validation ??= Validate(_value);
        }

        /// <summary>
        ///     Creates a new <see cref="PlayerLevel"/> from the
        ///     given <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref="PlayerLevel"/>.</returns>
        public static PlayerLevel From(System.Int32 value) {
            if (value == default) {
                return Unspecified;
            }

            var vo = new PlayerLevel(value);

            if (!vo.IsValid() && !PlayerLevelPreSetValueCache.PlayerLevelPreSetValues.TryGetValue(vo.Value, out _)) {
                throw new System.ArgumentException(vo.GetValidationErrorMessage());
            }

            return vo;
        }

        /// <summary>
        ///     Tries to create a new <see cref="PlayerLevel"/> from the
        ///     given <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <param name="result">The resulting value object if the method returns <see langword="true"/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword="true"/> if the value object was created successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryFrom(System.Int32 value, out PlayerLevel result) {
            result = value == default ? Unspecified : new PlayerLevel(value);
            return result.IsInitialized() && (Validate(result._value).IsSuccess || PlayerLevelPreSetValueCache.PlayerLevelPreSetValues.TryGetValue(value, out _));
        }

        /// <summary>
        ///     Represents a <see cref="PlayerLevel"/> with a default underlying value.
        /// </summary>
        public static PlayerLevel Unspecified { get; } = new PlayerLevel(default);

        /// <summary>
        ///     Indicates whether this <see cref="PlayerLevel"/> has been
        ///     initialized with a value.
        /// </summary>
        /// <returns><see langword="true" /> if this <see cref="PlayerLevel"/> has been initialized; otherwise, <see langword="false" />.</returns>
        public bool IsInitialized() => _initialized;

        /// <inheritdoc />
        public bool Equals(PlayerLevel? other)
        {
            if (other is null) return false;

            if (!other.Value.IsInitialized())
            {
                return !IsInitialized();
            }

            if (other.Value.IsInitialized() != IsInitialized())
            {
                return false;
            }
        
            return System.Collections.Generic.EqualityComparer<System.Int32>.Default.Equals(this._value, other.Value.Value);
        }

        /// <inheritdoc />
        public bool Equals(PlayerLevel other)
        {
            if (!other.IsInitialized())
            {
                return !IsInitialized();
            }

            if (other.IsInitialized() != IsInitialized())
            {
                return false;
            }
        
            return System.Collections.Generic.EqualityComparer<System.Int32>.Default.Equals(this._value, other.Value);
        }
        
        /// <inheritdoc />
        public bool Equals(PlayerLevel? other, System.Collections.Generic.IEqualityComparer<PlayerLevel> comparer)
        {
            if (other is null) return false;
            return comparer.Equals(this, other.Value);
        }
        
        /// <inheritdoc />
        public override int GetHashCode() {
            if (!IsInitialized()) return 0;
            return System.Collections.Generic.EqualityComparer<System.Int32>.Default.GetHashCode(this._value);
        }

        

        

        

        

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <inheritdoc cref="M:System.String.ToString(System.IFormatProvider)" />
        public string ToString(IFormatProvider? provider)
        {
            return Value.ToString(format: null, provider: provider) ?? "";
        }

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider)
        {{
            return Value.ToString(format, formatProvider) ?? "";
        }}

        /// <summary>
        ///     Indicates whether this value object is valid.
        /// </summary>
        /// <returns><see langword="true" /> if this value object is valid; otherwise, <see langword="false" />.</returns>
        public bool IsValid() => _validation.IsSuccess;

        /// <summary>
        ///     Gets the validation error message if this value object is not valid.
        /// </summary>
        /// <returns>The validation error message if this value object is not valid; otherwise, <see langword="null" />.</returns>
        public string? GetValidationErrorMessage() => _validation.IsSuccess ? null : _validation.ErrorMessage;

        private class Validation
        {
            public static readonly Validation Ok = new(string.Empty);
            private readonly bool _isSuccess;
        
            private Validation(string reason)
            {
                ErrorMessage = reason;
                _isSuccess = string.IsNullOrEmpty(reason);
            }
        
            public string ErrorMessage { get; }
            public bool IsSuccess => _isSuccess;
        
            public System.Collections.Generic.Dictionary<object, object>? Data { get; private set; }
        
            public static Validation Invalid(string reason = "")
            {
                if (string.IsNullOrEmpty(reason))
                {
                    return new Validation("[none provided]");
                }
        
                return new Validation(reason);
            }
        
            public Validation WithData(object key, object value)
            {
                Data ??= new System.Collections.Generic.Dictionary<object, object>();
                Data[key] = value;
                return this;
            }
        }

        

        private class PlayerLevelSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<PlayerLevel>
        {
            public override PlayerLevel Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
                    return new PlayerLevel();
                }

                object? underlyingValue;
                underlyingValue = reader.GetInt32();

                try {
                    var typedUnderlyingValue = (System.Int32)underlyingValue!;
                    if (PlayerLevel.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    throw new System.Text.Json.JsonException($"No matching PlayerLevel pre-set value found for value '{typedUnderlyingValue}', or the underlying value is invalid.");
                } catch (System.Exception e) {
                    throw new System.Text.Json.JsonException("Could not create an initialized instance of PlayerLevel.", e);
                }
            }

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                PlayerLevel value,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                object? underlyingValue = value.IsInitialized()
                    ? value.Value
                    : null;

                if (underlyingValue == null) {
                    writer.WriteNullValue();
                    return;
                }

                writer.WriteNumberValue((int)underlyingValue);
            }
        }

        private class PlayerLevelTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
            {
                return sourceType == UnderlyingType || sourceType == typeof(string);
            }
            
            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            {
                if (value == null) return Unspecified;
        
                if (value.GetType() == UnderlyingType)
                {
                    var underlyingValue = GetUnderlyingValue(value);
                    return underlyingValue == default ? Unspecified : From((System.Int32)underlyingValue);
                }
        
                if (value is string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return Unspecified;
                    object underlyingValue;
                    if (UnderlyingType == typeof(Guid)) {
                        underlyingValue = Guid.Parse(s);
                    } else if (UnderlyingType == typeof(DateOnly)) {
                        underlyingValue = DateOnly.Parse(s, culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    } else {
                        underlyingValue = Convert.ChangeType(s, UnderlyingType, culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }
                    return From((System.Int32)underlyingValue);
                }
    
                throw new NotSupportedException($@"Cannot convert from type '{value?.GetType()}'.");
            }

            private object? GetUnderlyingValue(object? value) {{
                if (value == null) {{
                    return default(System.Int32);
                }}
        
                if (value is System.Int32 v) {
                    return v;
                }
                
                if (Type.GetTypeCode(typeof(System.Int32)) == TypeCode.Object) {
                    throw new NotSupportedException($"Cannot convert value of type '{value?.GetType()}' to 'System.Int32'.");
                }
                
                return Convert.ChangeType(value, typeof(System.Int32));
            }}
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
            {
                return destinationType == UnderlyingType || destinationType == typeof(string);
            }
            
            public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            {
                if (destinationType == UnderlyingType)
                {
                    if (value is PlayerLevel vo)
                    {
                        return vo.Value;
                    }
                    return base.ConvertTo(context, culture ?? System.Globalization.CultureInfo.InvariantCulture, value, destinationType);
                }

                if (destinationType == typeof(string))
                {
                    if (value is PlayerLevel vo)
                    {
                        return vo.ToString(culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (value is System.IFormattable f)
                    {
                        return f.ToString(format: null, formatProvider: culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }
                    return value?.ToString();
                }

                throw new NotSupportedException($@"Cannot convert to type '{destinationType}'.");
            }
        }

        private static class PlayerLevelPreSetValueCache {
            public static readonly System.Collections.Generic.Dictionary<System.Int32, PlayerLevel> PlayerLevelPreSetValues = new();
        
            static PlayerLevelPreSetValueCache()
            {
                PlayerLevelPreSetValues[PlayerLevel.Unspecified.Value] = PlayerLevel.Unspecified;
                PlayerLevelPreSetValues[PlayerLevel.Invalid.Value] = PlayerLevel.Invalid;
            }
        }
    }
    
}