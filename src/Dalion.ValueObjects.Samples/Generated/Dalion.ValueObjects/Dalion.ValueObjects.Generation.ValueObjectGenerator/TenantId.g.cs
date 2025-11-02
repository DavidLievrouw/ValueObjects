
#nullable enable

using System;

namespace Dalion.ValueObjects.Samples {
    
    [System.Diagnostics.DebuggerDisplay("TenantId {Value}")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TenantIdSystemTextJsonConverter))]
    [System.ComponentModel.TypeConverter(typeof(TenantIdTypeConverter))]
    public partial record struct TenantId : IEquatable<TenantId>, IFormattable {
        private readonly System.Guid _value;
        private readonly bool _initialized;
#pragma warning disable CS0414
        private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
        private readonly Validation _validation;
        private static readonly Type UnderlyingType = typeof(System.Guid);

        /// <summary>
        ///     Gets the underlying value of this <see cref="TenantId"/>.
        /// </summary>
        public System.Guid Value => _value;

        /// <summary>
        ///     Creates a new <see cref="TenantId"/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public TenantId()
        {
            _value = default;
            _initialized = false;
            _isNullOrEmpty = false;
            _validation ??= Validation.Ok;
        }

        /// <summary>
        ///     Creates a new <see cref="TenantId"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private TenantId(System.Guid value) {
            
            _initialized = true;
            _value = value;
            _isNullOrEmpty = false;
            _validation ??= Validation.Ok;
        }

        /// <summary>
        ///     Creates a new <see cref="TenantId"/> from the
        ///     given <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref="TenantId"/>.</returns>
        public static TenantId From(System.Guid value) {
            if (value == default) {
                return Empty;
            }

            var vo = new TenantId(value);

            if (!vo.IsValid() && !TenantIdPreSetValueCache.TenantIdPreSetValues.TryGetValue(vo.Value, out _)) {
                throw new System.ArgumentException(vo.GetValidationErrorMessage());
            }

            return vo;
        }

        /// <summary>
        ///     Tries to create a new <see cref="TenantId"/> from the
        ///     given <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <param name="result">The resulting value object if the method returns <see langword="true"/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword="true"/> if the value object was created successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryFrom(System.Guid value, out TenantId result) {
            result = value == default ? Empty : new TenantId(value);
            return result.IsInitialized();
        }

        /// <summary>
        ///     Represents a <see cref="TenantId"/> with a default underlying value.
        /// </summary>
        public static TenantId Empty { get; } = new TenantId(default);

        /// <summary>
        ///     Indicates whether this <see cref="TenantId"/> has been
        ///     initialized with a value.
        /// </summary>
        /// <returns><see langword="true" /> if this <see cref="TenantId"/> has been initialized; otherwise, <see langword="false" />.</returns>
        public bool IsInitialized() => _initialized;

        /// <inheritdoc />
        public bool Equals(TenantId? other)
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
        
            return System.Collections.Generic.EqualityComparer<System.Guid>.Default.Equals(this._value, other.Value.Value);
        }

        /// <inheritdoc />
        public bool Equals(TenantId other)
        {
            if (!other.IsInitialized())
            {
                return !IsInitialized();
            }

            if (other.IsInitialized() != IsInitialized())
            {
                return false;
            }
        
            return System.Collections.Generic.EqualityComparer<System.Guid>.Default.Equals(this._value, other.Value);
        }
        
        /// <inheritdoc />
        public bool Equals(TenantId? other, System.Collections.Generic.IEqualityComparer<TenantId> comparer)
        {
            if (other is null) return false;
            return comparer.Equals(this, other.Value);
        }
        
        /// <inheritdoc />
        public override int GetHashCode() {
            if (!IsInitialized()) return 0;
            return System.Collections.Generic.EqualityComparer<System.Guid>.Default.GetHashCode(this._value);
        }

        

        

        

        /// <summary>
        ///     An explicit conversion from <see cref="System.Guid" /> to <see cref="TenantId" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The <see cref="TenantId" /> instance created from the input value.</returns>
        public static explicit operator TenantId(System.Guid value)
        {
            return TenantId.From(value);
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

        

        private class TenantIdSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<TenantId>
        {
            public override TenantId Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
                    return new TenantId();
                }

                object? underlyingValue;
                
                var guidStr = reader.GetString();
                if (!Guid.TryParse(guidStr, out var guidValue))
                    throw new System.Text.Json.JsonException($"Cannot convert '{guidStr}' to Guid.");
                underlyingValue = guidValue;

                try {
                    var typedUnderlyingValue = (System.Guid)underlyingValue!;
                    if (TenantId.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    throw new System.Text.Json.JsonException($"No matching TenantId pre-set value found for value '{typedUnderlyingValue}', or the underlying value is invalid.");
                } catch (System.Exception e) {
                    throw new System.Text.Json.JsonException("Could not create an initialized instance of TenantId.", e);
                }
            }

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                TenantId value,
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

                writer.WriteStringValue(((Guid)underlyingValue));
            }
        }

        private class TenantIdTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
            {
                return sourceType is not null && (sourceType.IsAssignableFrom(typeof(TenantId)) || sourceType.IsAssignableFrom(UnderlyingType) || sourceType == typeof(string));
            }
            
            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            {
                if (value is TenantId vo)
                {
                    return vo;
                }

                if (value == default) 
                {
                    return Empty;
                }
        
                if (value is System.Guid correctlyTypedValue)
                {
                    return correctlyTypedValue == default 
                        ? Empty 
                        : From(correctlyTypedValue);
                }
        
                if (value is string s)
                {
                    var underlyingValue = System.Guid.Parse(s, culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    return underlyingValue == default 
                        ? Empty 
                        : From((System.Guid)underlyingValue);
                }
    
                throw new NotSupportedException($@"Cannot convert from type '{value?.GetType()}'.");
            }
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
            {
                return destinationType is not null && (destinationType.IsAssignableFrom(typeof(TenantId)) || destinationType.IsAssignableFrom(UnderlyingType) || destinationType == typeof(string));
            }
            
            public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            {
                if (destinationType.IsAssignableFrom(typeof(TenantId)))
                {
                    if (value is TenantId vo)
                    {
                        return vo;
                    }
                    if (value is System.Guid correctlyTypedValue)
                    {
                        return From(correctlyTypedValue);
                    }
                    if (value is string s)
                    {
                        var underlyingValue = System.Guid.Parse(s, culture ?? System.Globalization.CultureInfo.InvariantCulture);
                        return From(underlyingValue);
                    }
                }

                if (destinationType.IsAssignableFrom(UnderlyingType))
                {
                    if (value is TenantId vo)
                    {
                        return vo.Value;
                    }
                    if (value is System.Guid correctlyTypedValue)
                    {
                        return correctlyTypedValue;
                    }
                    if (value is string s)
                    {
                        var underlyingValue = System.Guid.Parse(s, culture ?? System.Globalization.CultureInfo.InvariantCulture);
                        return underlyingValue;
                    }
                    return base.ConvertTo(context, culture ?? System.Globalization.CultureInfo.InvariantCulture, value, destinationType);
                }

                if (destinationType == typeof(string))
                {
                    if (value is TenantId vo)
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

        private static class TenantIdPreSetValueCache {
            public static readonly System.Collections.Generic.Dictionary<System.Guid, TenantId> TenantIdPreSetValues = new();
        
            static TenantIdPreSetValueCache()
            {
                TenantIdPreSetValues[TenantId.Empty.Value] = TenantId.Empty;

            }
        }
    }
    
}