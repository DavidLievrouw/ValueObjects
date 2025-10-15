
#nullable enable

namespace Dalion.ValueObjects.Samples {
    
    [System.Diagnostics.DebuggerDisplay("Birthday {Value}")]
    [System.Text.Json.Serialization.JsonConverter(typeof(BirthdaySystemTextJsonConverter))]
    [System.ComponentModel.TypeConverter(typeof(BirthdayTypeConverter))]
    public partial record struct Birthday : IEquatable<Birthday>, IFormattable, IEquatable<System.DateOnly>, ISpanParsable<Birthday>, IUtf8SpanParsable<Birthday>, IComparable<Birthday>, IComparable {
        private readonly System.DateOnly _value;
        private readonly bool _initialized;
#pragma warning disable CS0414
        private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
        private readonly Validation _validation;
        private static readonly Type UnderlyingType = typeof(System.DateOnly);

        /// <summary>
        ///     Gets the underlying value of this <see cref="Birthday"/>.
        /// </summary>
        public System.DateOnly Value => _value;

        /// <summary>
        ///     Creates a new <see cref="Birthday"/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Birthday()
        {
            _value = default;
            _initialized = false;
            _isNullOrEmpty = false;
            _validation ??= Validate(_value);
        }

        /// <summary>
        ///     Creates a new <see cref="Birthday"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private Birthday(System.DateOnly value) {
            
            _initialized = true;
            _value = value;
            _isNullOrEmpty = false;
            _validation ??= Validate(_value);
        }

        /// <summary>
        ///     Creates a new <see cref="Birthday"/> from the
        ///     given <see cref="System.DateOnly"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref="Birthday"/>.</returns>
        public static Birthday From(System.DateOnly value) {
            if (value == default) {
                return None;
            }

            

            var vo = new Birthday(value);

            if (!vo.IsValid() && !BirthdayPreSetValueCache.BirthdayPreSetValues.TryGetValue(value, out _)) {
                throw new System.InvalidOperationException(vo.GetValidationErrorMessage());
            }

            return vo;
        }

        /// <summary>
        ///     Tries to create a new <see cref="Birthday"/> from the
        ///     given <see cref="System.DateOnly"/>.
        /// </summary>
        /// <param name="value">The underlying value to create the value object from.</param>
        /// <param name="result">The resulting value object if the method returns <see langword="true"/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword="true"/> if the value object was created successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryFrom(System.DateOnly value, out Birthday result) {
            result = value == default ? None : new Birthday(value);
            return result.IsInitialized() && (Validate(result._value).IsSuccess || BirthdayPreSetValueCache.BirthdayPreSetValues.TryGetValue(value, out _));
        }

        /// <summary>
        ///     Represents a <see cref="Birthday"/> with a default underlying value.
        /// </summary>
        public static Birthday None { get; } = new Birthday(default);

        /// <summary>
        ///     Indicates whether this <see cref="Birthday"/> has been
        ///     initialized with a value.
        /// </summary>
        /// <returns><see langword="true" /> if this <see cref="Birthday"/> has been initialized; otherwise, <see langword="false" />.</returns>
        public bool IsInitialized() => _initialized;

        /// <inheritdoc />
        public bool Equals(Birthday? other)
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
        
            return EqualityComparer<System.DateOnly>.Default.Equals(this._value, other.Value.Value);
        }

        /// <inheritdoc />
        public bool Equals(Birthday other)
        {
            if (!other.IsInitialized())
            {
                return !IsInitialized();
            }

            if (other.IsInitialized() != IsInitialized())
            {
                return false;
            }
        
            return EqualityComparer<System.DateOnly>.Default.Equals(this._value, other.Value);
        }
        
        /// <inheritdoc />
        public bool Equals(Birthday? other, IEqualityComparer<Birthday> comparer)
        {
            if (other is null) return false;
            return comparer.Equals(this, other.Value);
        }
        
        /// <inheritdoc />
        public override int GetHashCode() {
            if (!IsInitialized()) return 0;
            return EqualityComparer<System.DateOnly>.Default.GetHashCode(this._value);
        }

        /// <inheritdoc />
        public bool Equals(System.DateOnly other)
        {
            return EqualityComparer<System.DateOnly>.Default.Equals(this._value, other);
        }

        /// <summary>
        ///     The equality operator for <see cref="Birthday" /> and <see cref="System.DateOnly" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Birthday left, System.DateOnly right) => left.Value.Equals(right);

        /// <summary>
        ///     The equality operator for <see cref="System.DateOnly" /> and <see cref="Birthday" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(System.DateOnly left, Birthday right) => right.Value.Equals(left);

        /// <summary>
        ///     The inequality operator for <see cref="Birthday" /> and <see cref="System.DateOnly" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered not to be equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(Birthday left, System.DateOnly right) => !(left == right);

        /// <summary>
        ///     The inequality operator for <see cref="System.DateOnly" /> and <see cref="Birthday" />.
        /// </summary>
        /// <returns><see langword="true" /> if the specified items are considered not to be equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(System.DateOnly left, Birthday right) => !(left == right);

        /// <inheritdoc />
        public int CompareTo(Birthday other) => this.Value.CompareTo(other.Value);

        /// <inheritdoc />
        public int CompareTo(System.DateOnly other) => this.Value.CompareTo(other);
        
        /// <inheritdoc />
        public int CompareTo(object? other)
        {
            if (other == null)
                return 1;
            if (other is Birthday other1)
                return this.CompareTo(other1);
            if (other is System.DateOnly v)
                return this.CompareTo(v);
            throw new System.ArgumentException(
                "Cannot compare to object as it is not of type Birthday",
                nameof(other)
            );
        }

        /// <summary>
        ///     An implicit conversion from <see cref="Birthday" /> to <see cref="System.DateOnly" />.
        /// </summary>
        /// <param name="id">The value to convert.</param>
        /// <returns>The System.DateOnly representation of the value object.</returns>
        public static explicit operator System.DateOnly(Birthday id)
        {
            return id.Value;
        }

        /// <summary>
        ///     An explicit conversion from <see cref="System.DateOnly" /> to <see cref="Birthday" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The <see cref="Birthday" /> instance created from the input value.</returns>
        public static explicit operator Birthday(System.DateOnly value)
        {
            return Birthday.From(value);
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
        public static Birthday Parse(string s, IFormatProvider? provider)
        {
            var v = System.DateOnly.Parse(s, provider);
            return From(v);
        }

        /// <inheritdoc />
        public static bool TryParse(
            string? s,
            IFormatProvider? provider,
            out Birthday result
        )
        {
            try
            {
                var v = s == null ? default : System.DateOnly.Parse(s, provider);
                return TryFrom(v, out result);
            }
            catch (ArgumentException)
            {
                result = default;
                return false;
            }
            catch (FormatException)
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc />
        public static Birthday Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            var v = System.DateOnly.Parse(s, provider);
            return From(v);
        }

        /// <inheritdoc />
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Birthday result)
        {
            try
            {
                var v = System.DateOnly.Parse(new string(s), provider);
                return TryFrom(v, out result);
            }
            catch (ArgumentException)
            {
                result = default;
                return false;
            }
            catch (FormatException)
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc />
        public static Birthday Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        {
            var s = System.Text.Encoding.UTF8.GetString(utf8Text);
            var v = System.DateOnly.Parse(s, provider);
            return From(v);
        }

        /// <inheritdoc />
        public static bool TryParse(
            ReadOnlySpan<byte> utf8Text,
            IFormatProvider? provider,
            out Birthday result
        )
        {
            try
            {
                var s = System.Text.Encoding.UTF8.GetString(utf8Text);
                var v = System.DateOnly.Parse(s, provider);
                return TryFrom(v, out result);
            }
            catch (ArgumentException)
            {
                result = default;
                return false;
            }
            catch (FormatException)
            {
                result = default;
                return false;
            }
        }

        private class BirthdaySystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<Birthday>
        {
            public override Birthday Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
                    return new Birthday();
                }
        
                var underlyingType = Birthday.UnderlyingType;
                object? underlyingValue;
            
                switch (Type.GetTypeCode(underlyingType)) {
                    case TypeCode.Boolean:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetBoolean();
                        break;
                    case TypeCode.Byte:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetByte();
                        break;
                    case TypeCode.Char:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        var charStr = reader.GetString();
                        if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                            throw new System.Text.Json.JsonException($"Cannot convert '{charStr}' to char.");
                        underlyingValue = charStr[0];
                        break;
                    case TypeCode.Decimal:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetDecimal();
                        break;
                    case TypeCode.Double:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetDouble();
                        break;
                    case TypeCode.Single:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetSingle();
                        break;
                    case TypeCode.Int16:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetInt16();
                        break;
                    case TypeCode.Int32:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetInt32();
                        break;
                    case TypeCode.Int64:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetInt64();
                        break;
                    case TypeCode.String:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetString();
                        break;
                    case TypeCode.DateTime:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                        underlyingValue = reader.GetDateTime();
                        break;
                    default:
                        if (underlyingType == typeof(Guid)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                            var guidStr = reader.GetString();
                            if (!Guid.TryParse(guidStr, out var guidValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{guidStr}' to Guid.");
                            underlyingValue = guidValue;
                        } else if (underlyingType == typeof(DateTimeOffset)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                            underlyingValue = reader.GetDateTimeOffset();
                        } else if (underlyingType == typeof(TimeSpan)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                            var tsStr = reader.GetString();
                            if (!TimeSpan.TryParse(tsStr, out var tsValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{tsStr}' to TimeSpan.");
                            underlyingValue = tsValue;
                        } else if (underlyingType == typeof(TimeOnly)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                            var timeStr = reader.GetString();
                            if (!TimeOnly.TryParse(timeStr, out var timeValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{timeStr}' to TimeOnly.");
                            underlyingValue = timeValue;
                        } else if (underlyingType == typeof(Uri)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                            var uriStr = reader.GetString();
                            if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{uriStr}' to Uri.");
                            underlyingValue = uriValue;
                        } else if (underlyingType == typeof(DateOnly)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($"Unsupported JSON token type for Birthday.");
                            var dateStr = reader.GetString();
                            if (!DateOnly.TryParse(dateStr, out var dateValue))
                                throw new System.Text.Json.JsonException($"Cannot convert '{dateStr}' to DateOnly.");
                            underlyingValue = dateValue;
                        } else {
                            throw new System.Text.Json.JsonException($"Unsupported underlying type for Birthday.");
                        }
                        break;
                }
            
                try {
                    var typedUnderlyingValue = (System.DateOnly)underlyingValue!;
                    if (typedUnderlyingValue.Equals(Birthday.None.Value)) {
                        return Birthday.None;
                    }
                    if (Birthday.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    if (BirthdayPreSetValueCache.BirthdayPreSetValues.TryGetValue(typedUnderlyingValue, out var constant)) {
                        return constant;
                    }
                    throw new System.Text.Json.JsonException($"No matching Birthday pre-set value found for value '{typedUnderlyingValue}'.");
                } catch (System.Exception e) {
                    throw new System.Text.Json.JsonException("Could not create an initialized instance of Birthday.", e);
                }
            }
            
            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                Birthday value,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                var underlyingType = Birthday.UnderlyingType;
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
                            throw new System.Text.Json.JsonException($"Unsupported underlying type for Birthday.");
                        }
                        break;
                }
            }
        }

        private class BirthdayTypeConverter : System.ComponentModel.TypeConverter
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
        
                return underlyingValue == default ? None : From((System.DateOnly)underlyingValue);
            }
        
            private object? GetUnderlyingValue(object? value) {{
                if (value == null) {{
                    return default(System.DateOnly);
                }}
        
                if (value is System.DateOnly v) {
                    return v;
                }
                
                if (Type.GetTypeCode(typeof(System.DateOnly)) == TypeCode.Object) {
                    throw new NotSupportedException($"Cannot convert value of type '{value?.GetType()}' to 'System.DateOnly'.");
                }
                
                return Convert.ChangeType(value, typeof(System.DateOnly));
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
        
                if (value is Birthday vo)
                {
                    return vo.Value;
                }
        
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        private static class BirthdayPreSetValueCache {
            public static readonly Dictionary<System.DateOnly, Birthday> BirthdayPreSetValues = new();
        
            static BirthdayPreSetValueCache()
            {
                BirthdayPreSetValues[Birthday.None.Value] = Birthday.None;
                BirthdayPreSetValues[Birthday.Patrick.Value] = Birthday.Patrick;
                BirthdayPreSetValues[Birthday.Sandra.Value] = Birthday.Sandra;
                BirthdayPreSetValues[Birthday.InvalidFuture.Value] = Birthday.InvalidFuture;
            }
        }
    }
    
}