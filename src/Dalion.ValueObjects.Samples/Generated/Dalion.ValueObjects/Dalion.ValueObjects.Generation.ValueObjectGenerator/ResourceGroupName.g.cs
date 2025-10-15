
#nullable enable

namespace Dalion.ValueObjects.Samples {
    
    [System.Diagnostics.DebuggerDisplay("ResourceGroupName {Value}")]
    [System.Text.Json.Serialization.JsonConverter(typeof(ResourceGroupNameSystemTextJsonConverter))]
    [System.ComponentModel.TypeConverter(typeof(ResourceGroupNameTypeConverter))]
    public partial record struct ResourceGroupName : IEquatable<ResourceGroupName>, IEquatable<System.String>, ISpanParsable<ResourceGroupName>, IUtf8SpanParsable<ResourceGroupName>, IComparable<ResourceGroupName>, IComparable {
        private readonly System.String _value;
        private readonly bool _initialized;
#pragma warning disable CS0414
        private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
        private readonly Validation _validation;
        private static readonly Type UnderlyingType = typeof(System.String);

        /// <summary>
        ///     Gets the underlying value of this <see cref="ResourceGroupName"/>.
        /// </summary>
        public System.String Value => _value;

        /// <summary>
        ///     Creates a new <see cref="ResourceGroupName"/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ResourceGroupName()
        {
            _value = System.String.Empty;
            _initialized = false;
            _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
            _validation ??= Validate(_value);
        }

        /// <summary>
        ///     Creates a new <see cref="ResourceGroupName"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private ResourceGroupName(System.String? value) {
            value = NormalizeInput(value);
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
        ///     Creates a new <see cref="ResourceGroupName"/> from the
        ///     given <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref="ResourceGroupName"/>.</returns>
        public static ResourceGroupName From(System.String? value) {
            if (value is null) {
                throw new System.InvalidOperationException("Cannot create an instance of ResourceGroupName from null.");
            }

            value = NormalizeInput(value);

            var vo = new ResourceGroupName(value);

            if (!vo.IsValid() && value is not null && !ResourceGroupNamePreSetValueCache.ResourceGroupNamePreSetValues.TryGetValue(value, out _)) {
                throw new System.InvalidOperationException(vo.GetValidationErrorMessage());
            }

            return vo;
        }

        /// <summary>
        ///     Tries to create a new <see cref="ResourceGroupName"/> from the
        ///     given <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <param name="result">The resulting value object if the method returns <see langword="true"/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword="true"/> if the value object was created successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryFrom(System.String? value, out ResourceGroupName result) {
            if (value is null) {
                result = new ResourceGroupName();
                return false;
            }

            result = string.IsNullOrEmpty(value) ? Empty : new ResourceGroupName(value);
            return result.IsInitialized() && (Validate(result._value).IsSuccess || ResourceGroupNamePreSetValueCache.ResourceGroupNamePreSetValues.TryGetValue(value, out _));
        }

        /// <summary>
        ///     Represents a <see cref="ResourceGroupName"/> with a default underlying value.
        /// </summary>
        public static ResourceGroupName Empty { get; } = new ResourceGroupName(System.String.Empty);

        /// <summary>
        ///     Indicates whether this <see cref="ResourceGroupName"/> has been
        ///     initialized with a value.
        /// </summary>
        /// <returns><see langword="true" /> if this <see cref="ResourceGroupName"/> has been initialized; otherwise, <see langword="false" />.</returns>
        public bool IsInitialized() => _initialized;

        /// <inheritdoc />
        public bool Equals(ResourceGroupName? other)
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
                : System.String.Equals(this._value, other.Value.Value, System.StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public bool Equals(ResourceGroupName other)
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
                : System.String.Equals(this._value, other.Value, System.StringComparison.OrdinalIgnoreCase);
        }
        
        /// <inheritdoc />
        public bool Equals(ResourceGroupName? other, IEqualityComparer<ResourceGroupName> comparer)
        {
            if (other is null) return false;
            return comparer.Equals(this, other.Value);
        }
        
        /// <inheritdoc />
        public override int GetHashCode() {
            if (!IsInitialized()) return 0;
            return StringComparer.OrdinalIgnoreCase.GetHashCode(this._value);
        }

        /// <inheritdoc />
        public bool Equals(System.String? other)
        {
            return System.String.IsNullOrEmpty(other)
                ? this._isNullOrEmpty
                : System.String.Equals(this._value, other, System.StringComparison.OrdinalIgnoreCase);
        }
        
        /// <inheritdoc />
        public bool Equals(System.String? underlyingValue, StringComparer comparer)
        {
            return comparer.Equals(this.Value, underlyingValue);
        }

        /// <summary>
        ///     The equality operator for <see cref="ResourceGroupName" /> and <see cref="System.String" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(ResourceGroupName left, System.String? right) => left.Value.Equals(right);

        /// <summary>
        ///     The equality operator for <see cref="System.String" /> and <see cref="ResourceGroupName" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(System.String? left, ResourceGroupName right) => right.Value.Equals(left);

        /// <summary>
        ///     The inequality operator for <see cref="ResourceGroupName" /> and <see cref="System.String" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered not to be equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(ResourceGroupName left, System.String? right) => !(left == right);

        /// <summary>
        ///     The inequality operator for <see cref="System.String" /> and <see cref="ResourceGroupName" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered not to be equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(System.String? left, ResourceGroupName right) => !(left == right);

        /// <inheritdoc />
        public int CompareTo(ResourceGroupName other) => this.Value.CompareTo(other.Value);

        /// <inheritdoc />
        public int CompareTo(System.String? other) => this.Value.CompareTo(other);
        
        /// <inheritdoc />
        public int CompareTo(object? other)
        {
            if (other == null)
                return 1;
            if (other is ResourceGroupName other1)
                return this.CompareTo(other1);
            if (other is System.String v)
                return this.CompareTo(v);
            throw new System.ArgumentException(
                "Cannot compare to object as it is not of type ResourceGroupName",
                nameof(other)
            );
        }

        /// <summary>
        ///     An implicit conversion from <see cref="ResourceGroupName" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="id">The value to convert.</param>
        /// <returns>The System.String representation of the value object.</returns>
        public static implicit operator System.String(ResourceGroupName id)
        {
            return id.Value;
        }

        /// <summary>
        ///     An explicit conversion from <see cref="System.String" /> to <see cref="ResourceGroupName" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The <see cref="ResourceGroupName" /> instance created from the input value.</returns>
        public static explicit operator ResourceGroupName(System.String value)
        {
            return ResourceGroupName.From(value);
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

        /// <inheritdoc />
        public static ResourceGroupName Parse(string s, IFormatProvider? provider)
        {
            return From(s);
        }

        /// <inheritdoc />
        public static bool TryParse(
            string? s,
            IFormatProvider? provider,
            out ResourceGroupName result
        )
        {
            return TryFrom(s, out result);
        }

        /// <inheritdoc />
        public static ResourceGroupName Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            return From(new string(s));
        }

        /// <inheritdoc />
        public static bool TryParse(
            ReadOnlySpan<char> s,
            IFormatProvider? provider,
            out ResourceGroupName result
        )
        {
            return TryFrom(new string(s), out result);
        }

        /// <inheritdoc />
        public static ResourceGroupName Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        {
            var s = System.Text.Encoding.UTF8.GetString(utf8Text);
            return From(s);
        }

        /// <inheritdoc />
        public static bool TryParse(
            ReadOnlySpan<byte> utf8Text,
            IFormatProvider? provider,
            out ResourceGroupName result
        )
        {
            try
            {
                var s = System.Text.Encoding.UTF8.GetString(utf8Text);
                return TryFrom(s, out result);
            }
            catch (ArgumentException)
            {
                result = default;
                return false;
            }
        }

        private class ResourceGroupNameSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<ResourceGroupName>
        {
            public override ResourceGroupName Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
                    return new ResourceGroupName();
                }

                object? underlyingValue;
                underlyingValue = reader.GetString();

                try {
                    var typedUnderlyingValue = (System.String)underlyingValue!;
                    if (ResourceGroupName.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    throw new System.Text.Json.JsonException($"No matching ResourceGroupName pre-set value found for value '{typedUnderlyingValue}', or the underlying value is invalid.");
                } catch (System.Exception e) {
                    throw new System.Text.Json.JsonException("Could not create an initialized instance of ResourceGroupName.", e);
                }
            }

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                ResourceGroupName value,
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

        private class ResourceGroupNameTypeConverter : System.ComponentModel.TypeConverter
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
        
                if (value is ResourceGroupName vo)
                {
                    return vo.Value;
                }
        
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        private static class ResourceGroupNamePreSetValueCache {
            public static readonly Dictionary<System.String, ResourceGroupName> ResourceGroupNamePreSetValues = new();
        
            static ResourceGroupNamePreSetValueCache()
            {
                ResourceGroupNamePreSetValues[ResourceGroupName.Empty.Value] = ResourceGroupName.Empty;

            }
        }
    }
    
}