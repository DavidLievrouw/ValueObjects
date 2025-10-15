
#nullable enable

namespace Dalion.ValueObjects.Samples {
    
    [System.Diagnostics.DebuggerDisplay("LegacyPhoneNumber {Value}")]
    [System.Text.Json.Serialization.JsonConverter(typeof(LegacyPhoneNumberSystemTextJsonConverter))]
    [System.ComponentModel.TypeConverter(typeof(LegacyPhoneNumberTypeConverter))]
    public partial record struct LegacyPhoneNumber : IEquatable<LegacyPhoneNumber> {
        private readonly System.String _value;
        private readonly bool _initialized;
#pragma warning disable CS0414
        private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
        private readonly Validation _validation;
        private static readonly Type UnderlyingType = typeof(System.String);

        /// <summary>
        ///     Gets the underlying value of this <see cref="LegacyPhoneNumber"/>.
        /// </summary>
        public System.String Value => _value;

        /// <summary>
        ///     Creates a new <see cref="LegacyPhoneNumber"/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public LegacyPhoneNumber()
        {
            _value = System.String.Empty;
            _initialized = false;
            _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
            _validation ??= Validation.Ok;
        }

        /// <summary>
        ///     Creates a new <see cref="LegacyPhoneNumber"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private LegacyPhoneNumber(System.String? value) {
            
            if (value == default) {
                _initialized = false;
                _value = System.String.Empty;
            } else {
                _initialized = true;
                _value = value;
            }
            _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
            _validation ??= Validation.Ok;
        }

        /// <summary>
        ///     Creates a new <see cref="LegacyPhoneNumber"/> from the
        ///     given <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref="LegacyPhoneNumber"/>.</returns>
        public static LegacyPhoneNumber From(System.String? value) {
            if (value is null) {
                throw new System.InvalidOperationException("Cannot create an instance of LegacyPhoneNumber from null.");
            }

            

            var vo = new LegacyPhoneNumber(value);

            if (!vo.IsValid() && value is not null && !LegacyPhoneNumberPreSetValueCache.LegacyPhoneNumberPreSetValues.TryGetValue(value, out _)) {
                throw new System.InvalidOperationException(vo.GetValidationErrorMessage());
            }

            return vo;
        }

        /// <summary>
        ///     Tries to create a new <see cref="LegacyPhoneNumber"/> from the
        ///     given <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <param name="result">The resulting value object if the method returns <see langword="true"/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword="true"/> if the value object was created successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryFrom(System.String? value, out LegacyPhoneNumber result) {
            if (value is null) {
                result = new LegacyPhoneNumber();
                return false;
            }

            result = string.IsNullOrEmpty(value) ? Empty : new LegacyPhoneNumber(value);
            return result.IsInitialized();
        }

        /// <summary>
        ///     Represents a <see cref="LegacyPhoneNumber"/> with a default underlying value.
        /// </summary>
        public static LegacyPhoneNumber Empty { get; } = new LegacyPhoneNumber(System.String.Empty);

        /// <summary>
        ///     Indicates whether this <see cref="LegacyPhoneNumber"/> has been
        ///     initialized with a value.
        /// </summary>
        /// <returns><see langword="true" /> if this <see cref="LegacyPhoneNumber"/> has been initialized; otherwise, <see langword="false" />.</returns>
        public bool IsInitialized() => _initialized;

        /// <inheritdoc />
        public bool Equals(LegacyPhoneNumber? other)
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
        
            return other.Value._isNullOrEmpty
                ? this._isNullOrEmpty
                : System.String.Equals(this._value, other.Value.Value, System.StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public bool Equals(LegacyPhoneNumber other)
        {
            if (!other.IsInitialized())
            {
                return !IsInitialized();
            }

            if (other.IsInitialized() != IsInitialized())
            {
                return false;
            }
        
            return other._isNullOrEmpty
                ? this._isNullOrEmpty
                : System.String.Equals(this._value, other.Value, System.StringComparison.Ordinal);
        }
        
        /// <inheritdoc />
        public bool Equals(LegacyPhoneNumber? other, IEqualityComparer<LegacyPhoneNumber> comparer)
        {
            if (other is null) return false;
            return comparer.Equals(this, other.Value);
        }
        
        /// <inheritdoc />
        public override int GetHashCode() {
            if (!IsInitialized()) return 0;
            return StringComparer.Ordinal.GetHashCode(this._value);
        }

        

        

        

        /// <summary>
        ///     An implicit conversion from <see cref="LegacyPhoneNumber" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="id">The value to convert.</param>
        /// <returns>The System.String representation of the value object.</returns>
        public static implicit operator System.String(LegacyPhoneNumber id)
        {
            return id.Value;
        }

        /// <summary>
        ///     An explicit conversion from <see cref="System.String" /> to <see cref="LegacyPhoneNumber" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The <see cref="LegacyPhoneNumber" /> instance created from the input value.</returns>
        public static explicit operator LegacyPhoneNumber(System.String value)
        {
            return LegacyPhoneNumber.From(value);
        }

        /// <inheritdoc />
        public override string ToString()
        {{
            return Value;
        }}

        /// <inheritdoc cref="M:System.String.ToString(System.IFormatProvider)" />
        public string ToString(IFormatProvider? provider)
        {{
            return Value.ToString(provider: provider);
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
        
            public Dictionary<object, object>? Data { get; private set; }
        
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
                Data ??= new Dictionary<object, object>();
                Data[key] = value;
                return this;
            }
        }

        

        private class LegacyPhoneNumberSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<LegacyPhoneNumber>
        {
            public override LegacyPhoneNumber Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
                    return new LegacyPhoneNumber();
                }

                object? underlyingValue;
                underlyingValue = reader.GetString();

                try {
                    var typedUnderlyingValue = (System.String)underlyingValue!;
                    if (typedUnderlyingValue.Equals(LegacyPhoneNumber.Empty.Value)) {
                        return LegacyPhoneNumber.Empty;
                    }
                    if (LegacyPhoneNumberPreSetValueCache.LegacyPhoneNumberPreSetValues.TryGetValue(typedUnderlyingValue, out var constant)) {
                        return constant;
                    }
                    if (LegacyPhoneNumber.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    throw new System.Text.Json.JsonException($"No matching LegacyPhoneNumber pre-set value found for value '{typedUnderlyingValue}', or the underlying value is invalid.");
                } catch (System.Exception e) {
                    throw new System.Text.Json.JsonException("Could not create an initialized instance of LegacyPhoneNumber.", e);
                }
            }

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                LegacyPhoneNumber value,
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

                writer.WriteStringValue((string)underlyingValue);
            }
        }

        private class LegacyPhoneNumberTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
            {
                return sourceType == UnderlyingType;
            }
            
            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            {
                if (value != null && !CanConvertFrom(context, value.GetType()))
                {
                    throw new NotSupportedException($"Cannot convert from type '{value?.GetType()}'.");
                }
        
                var underlyingValue = GetUnderlyingValue(value);
        
                return underlyingValue == default ? Empty : From((System.String)underlyingValue);
            }
        
            private object? GetUnderlyingValue(object? value) {{
                if (value == null) {{
                    return default(System.String);
                }}
        
                if (value is System.String v) {
                    return v;
                }
                
                if (Type.GetTypeCode(typeof(System.String)) == TypeCode.Object) {
                    throw new NotSupportedException($"Cannot convert value of type '{value?.GetType()}' to 'System.String'.");
                }
                
                return Convert.ChangeType(value, typeof(System.String));
            }}
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
            {
                return destinationType == UnderlyingType;
            }
            
            public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            {
                if (!CanConvertTo(context, destinationType))
                {
                    throw new NotSupportedException($"Cannot convert to type '{destinationType}'.");
                }
        
                if (value is LegacyPhoneNumber vo)
                {
                    return vo.Value;
                }
        
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        private static class LegacyPhoneNumberPreSetValueCache {
            public static readonly Dictionary<System.String, LegacyPhoneNumber> LegacyPhoneNumberPreSetValues = new();
        
            static LegacyPhoneNumberPreSetValueCache()
            {
                LegacyPhoneNumberPreSetValues[LegacyPhoneNumber.Empty.Value] = LegacyPhoneNumber.Empty;

            }
        }
    }
    
}