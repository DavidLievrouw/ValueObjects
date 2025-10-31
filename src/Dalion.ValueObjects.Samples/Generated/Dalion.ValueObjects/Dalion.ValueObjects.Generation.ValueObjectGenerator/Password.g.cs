
#nullable enable

using System;

namespace Dalion.ValueObjects.Samples {
    
    [System.Diagnostics.DebuggerDisplay("Password {Value}")]
    [System.Text.Json.Serialization.JsonConverter(typeof(PasswordSystemTextJsonConverter))]
    [System.ComponentModel.TypeConverter(typeof(PasswordTypeConverter))]
    public partial record struct Password : IEquatable<Password> {
        private readonly System.String _value;
        private readonly bool _initialized;
#pragma warning disable CS0414
        private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
        private readonly Validation _validation;
        private static readonly Type UnderlyingType = typeof(System.String);

        /// <summary>
        ///     Gets the underlying value of this <see cref="Password"/>.
        /// </summary>
        public System.String Value => _value;

        /// <summary>
        ///     Creates a new <see cref="Password"/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Password()
        {
            _value = System.String.Empty;
            _initialized = false;
            _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
            _validation ??= Validate(_value);
        }

        /// <summary>
        ///     Creates a new <see cref="Password"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private Password(System.String? value) {
            
            if (value == default) {
                _initialized = false;
                _value = System.String.Empty;
            } else {
                _initialized = true;
                _value = value;
            }
            _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
            _validation ??= Validate(_value);
        }

        /// <summary>
        ///     Creates a new <see cref="Password"/> from the
        ///     given <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref="Password"/>.</returns>
        public static Password From(System.String? value) {
            if (value is null) {
                throw new System.ArgumentException("Cannot create an instance of Password from null.");
            }

            

            var vo = new Password(value);

            if (!vo.IsValid() && value is not null && !PasswordPreSetValueCache.PasswordPreSetValues.TryGetValue(value, out _)) {
                throw new System.ArgumentException(vo.GetValidationErrorMessage());
            }

            return vo;
        }

        /// <summary>
        ///     Tries to create a new <see cref="Password"/> from the
        ///     given <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <param name="result">The resulting value object if the method returns <see langword="true"/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword="true"/> if the value object was created successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryFrom(System.String? value, out Password result) {
            if (value is null) {
                result = new Password();
                return false;
            }

            result = string.IsNullOrEmpty(value) ? Empty : new Password(value);
            return result.IsInitialized() && (Validate(result._value).IsSuccess || PasswordPreSetValueCache.PasswordPreSetValues.TryGetValue(value, out _));
        }

        /// <summary>
        ///     Represents a <see cref="Password"/> with a default underlying value.
        /// </summary>
        public static Password Empty { get; } = new Password(System.String.Empty);

        /// <summary>
        ///     Indicates whether this <see cref="Password"/> has been
        ///     initialized with a value.
        /// </summary>
        /// <returns><see langword="true" /> if this <see cref="Password"/> has been initialized; otherwise, <see langword="false" />.</returns>
        public bool IsInitialized() => _initialized;

        /// <inheritdoc />
        public bool Equals(Password? other)
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
        public bool Equals(Password other)
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
        public bool Equals(Password? other, System.Collections.Generic.IEqualityComparer<Password> comparer)
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
        ///     An implicit conversion from <see cref="Password" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="id">The value to convert.</param>
        /// <returns>The System.String representation of the value object.</returns>
        public static explicit operator System.String(Password id)
        {
            return id.Value;
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

        

        private class PasswordSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<Password>
        {
            public override Password Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
                    return new Password();
                }

                object? underlyingValue;
                underlyingValue = reader.GetString();

                try {
                    var typedUnderlyingValue = (System.String)underlyingValue!;
                    if (Password.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    throw new System.Text.Json.JsonException($"No matching Password pre-set value found for value '{typedUnderlyingValue}', or the underlying value is invalid.");
                } catch (System.Exception e) {
                    throw new System.Text.Json.JsonException("Could not create an initialized instance of Password.", e);
                }
            }

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                Password value,
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

        private class PasswordTypeConverter : System.ComponentModel.TypeConverter
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
        
                if (value is Password vo)
                {
                    return vo.Value;
                }
        
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        private static class PasswordPreSetValueCache {
            public static readonly System.Collections.Generic.Dictionary<System.String, Password> PasswordPreSetValues = new();
        
            static PasswordPreSetValueCache()
            {
                PasswordPreSetValues[Password.Empty.Value] = Password.Empty;

            }
        }
    }
    
}