
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

        public System.String Value => _value;

        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ResourceGroupName()
        {
            _value = System.String.Empty;
            _initialized = false;
            _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
            _validation ??= Validate(_value);
        }

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

        public static bool TryFrom(System.String? value, out ResourceGroupName result) {
            if (value is null) {
                result = new ResourceGroupName();
                return false;
            }

            result = string.IsNullOrEmpty(value) ? Empty : new ResourceGroupName(value);
            return result.IsInitialized() && (Validate(result._value).IsSuccess || ResourceGroupNamePreSetValueCache.ResourceGroupNamePreSetValues.TryGetValue(value, out _));
        }

        public static ResourceGroupName Empty { get; } = new ResourceGroupName(System.String.Empty);

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
        
        public bool Equals(System.String? underlyingValue, StringComparer comparer)
        {
            return comparer.Equals(this.Value, underlyingValue);
        }

        public static bool operator ==(ResourceGroupName left, System.String? right) => left.Value.Equals(right);

        public static bool operator ==(System.String? left, ResourceGroupName right) => right.Value.Equals(left);

        public static bool operator !=(System.String? left, ResourceGroupName right) => !(left == right);

        public static bool operator !=(ResourceGroupName left, System.String? right) => !(left == right);

        public int CompareTo(ResourceGroupName other) => this.Value.CompareTo(other.Value);

        public int CompareTo(System.String? other) => this.Value.CompareTo(other);
        
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

        public bool IsValid() => _validation.IsSuccess;
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
        
                var underlyingType = ResourceGroupName.UnderlyingType;
                object? underlyingValue;
            
                switch (Type.GetTypeCode(underlyingType)) {
                    case TypeCode.Boolean:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetBoolean();
                        break;
                    case TypeCode.Byte:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetByte();
                        break;
                    case TypeCode.Char:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        var charStr = reader.GetString();
                        if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                            throw new System.Text.Json.JsonException($"Cannot convert '{charStr}' to char.");
                        underlyingValue = charStr[0];
                        break;
                    case TypeCode.Decimal:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetDecimal();
                        break;
                    case TypeCode.Double:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetDouble();
                        break;
                    case TypeCode.Single:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetSingle();
                        break;
                    case TypeCode.Int16:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetInt16();
                        break;
                    case TypeCode.Int32:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetInt32();
                        break;
                    case TypeCode.Int64:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetInt64();
                        break;
                    case TypeCode.String:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetString();
                        break;
                    case TypeCode.DateTime:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                        underlyingValue = reader.GetDateTime();
                        break;
                    default:
                        if (underlyingType == typeof(Guid)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                            var guidStr = reader.GetString();
                            if (!Guid.TryParse(guidStr, out var guidValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{guidStr}' to Guid.");
                            underlyingValue = guidValue;
                        } else if (underlyingType == typeof(DateTimeOffset)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                            underlyingValue = reader.GetDateTimeOffset();
                        } else if (underlyingType == typeof(TimeSpan)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                            var tsStr = reader.GetString();
                            if (!TimeSpan.TryParse(tsStr, out var tsValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{tsStr}' to TimeSpan.");
                            underlyingValue = tsValue;
                        } else if (underlyingType == typeof(TimeOnly)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                            var timeStr = reader.GetString();
                            if (!TimeOnly.TryParse(timeStr, out var timeValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{timeStr}' to TimeOnly.");
                            underlyingValue = timeValue;
                        } else if (underlyingType == typeof(Uri)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                            var uriStr = reader.GetString();
                            if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{uriStr}' to Uri.");
                            underlyingValue = uriValue;
                        } else if (underlyingType == typeof(DateOnly)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for ResourceGroupName.");
                            var dateStr = reader.GetString();
                            if (!DateOnly.TryParse(dateStr, out var dateValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{dateStr}' to DateOnly.");
                            underlyingValue = dateValue;
                        } else {
                            throw new System.Text.Json.JsonException($"Unsupported underlying type for ResourceGroupName.");
                        }
                        break;
                }
            
                try {
                    var typedUnderlyingValue = (System.String)underlyingValue!;
                    if (typedUnderlyingValue.Equals(ResourceGroupName.Empty.Value)) {
                        return ResourceGroupName.Empty;
                    }
                    if (ResourceGroupName.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    if (ResourceGroupNamePreSetValueCache.ResourceGroupNamePreSetValues.TryGetValue(typedUnderlyingValue, out var constant)) {
                        return constant;
                    }
                    throw new System.Text.Json.JsonException($"No matching ResourceGroupName pre-set value found for value '{typedUnderlyingValue}'.");
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
                var underlyingType = ResourceGroupName.UnderlyingType;
                object? underlyingValue = value.IsInitialized()
                    ? value.Value
                    : null;
        
                if (underlyingValue == null) {
                    writer.WriteNullValue();
                    return;
                }
            
                switch (Type.GetTypeCode(underlyingType)) {
                    case TypeCode.Boolean:
                        writer.WriteBooleanValue((bool)underlyingValue);
                        break;
                    case TypeCode.Byte:
                        writer.WriteNumberValue((byte)underlyingValue);
                        break;
                    case TypeCode.Char:
                        writer.WriteStringValue(((char)underlyingValue).ToString());
                        break;
                    case TypeCode.Decimal:
                        writer.WriteNumberValue((decimal)underlyingValue);
                        break;
                    case TypeCode.Double:
                        writer.WriteNumberValue((double)underlyingValue);
                        break;
                    case TypeCode.Single:
                        writer.WriteNumberValue((float)underlyingValue);
                        break;
                    case TypeCode.Int16:
                        writer.WriteNumberValue((short)underlyingValue);
                        break;
                    case TypeCode.Int32:
                        writer.WriteNumberValue((int)underlyingValue);
                        break;
                    case TypeCode.Int64:
                        writer.WriteNumberValue((long)underlyingValue);
                        break;
                    case TypeCode.String:
                        writer.WriteStringValue((string)underlyingValue);
                        break;
                    case TypeCode.DateTime:
                        writer.WriteStringValue(((DateTime)underlyingValue));
                        break;
                    default:
                        if (underlyingType == typeof(Guid)) {
                            writer.WriteStringValue(((Guid)underlyingValue));
                        } else if (underlyingType == typeof(DateTimeOffset)) {
                            writer.WriteStringValue(((DateTimeOffset)underlyingValue));
                        } else if (underlyingType == typeof(TimeSpan)) {
                            writer.WriteStringValue(((TimeSpan)underlyingValue).ToString());
                        } else if (underlyingType == typeof(TimeOnly)) {
                            writer.WriteStringValue(((TimeOnly)underlyingValue).ToString());
                        } else if (underlyingType == typeof(Uri)) {
                            writer.WriteStringValue(((Uri)underlyingValue).ToString());
                        } else if (underlyingType == typeof(DateOnly)) {
                            writer.WriteStringValue(((DateOnly)underlyingValue).ToString("yyyy-MM-dd"));
                        } else {
                            throw new System.Text.Json.JsonException($"Unsupported underlying type for ResourceGroupName.");
                        }
                        break;
                }
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