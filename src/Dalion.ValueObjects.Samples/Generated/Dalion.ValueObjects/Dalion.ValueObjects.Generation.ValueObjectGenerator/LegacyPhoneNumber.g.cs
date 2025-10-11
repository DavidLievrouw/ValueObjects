
        #nullable enable

        namespace Dalion.ValueObjects.Samples {
            
            [System.Diagnostics.DebuggerDisplay("LegacyPhoneNumber {Value}")]
            [System.Text.Json.Serialization.JsonConverter(typeof(LegacyPhoneNumberSystemTextJsonConverter))]
            [System.ComponentModel.TypeConverter(typeof(LegacyPhoneNumberTypeConverter))]
            public partial record struct LegacyPhoneNumber : IEquatable<LegacyPhoneNumber>
 {
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
                public LegacyPhoneNumber()
                {
                    _value = System.String.Empty;
                    _initialized = false;
                    _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
                    _validation = Validation.Ok;
                }

                [System.Diagnostics.DebuggerStepThrough]
                private LegacyPhoneNumber(System.String? value, bool validation = true) {
                    
                    if (validation) {
                        
                    }
                    if (value == default) {
                        _initialized = false;
                        _value = System.String.Empty;
                    } else {
                        _initialized = true;
                        _value = value;
                    }
                    _isNullOrEmpty = System.String.IsNullOrEmpty(_value);
                    _validation = Validation.Ok;
                }

                public static LegacyPhoneNumber From(System.String? value) {
                    if (value is null) {
                        
                        var instance = new LegacyPhoneNumber();
                        return instance;
                    }

                    return new LegacyPhoneNumber(value);
                }

                public static bool TryFrom(System.String? value, out LegacyPhoneNumber result) {
                    if (value is null) {
                        result = new LegacyPhoneNumber();
                        return result.IsInitialized();
                    }

                    result = string.IsNullOrEmpty(value) ? Empty : new LegacyPhoneNumber(value, validation: false);
                    return result.IsInitialized();
                }


                public static LegacyPhoneNumber Empty { get; } = new LegacyPhoneNumber(System.String.Empty, validation: false);

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
                    return Value ?? "";
                }}

                /// <inheritdoc cref="M:System.String.ToString(System.IFormatProvider)" />
                public string ToString(IFormatProvider? provider)
                {{
                    return Value.ToString(provider: provider) ?? "";
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
private class ValueObjectValidationException : Exception
{
    private const string DefaultMessage = "Validation of the value object failed.";

    public ValueObjectValidationException()
        : base(DefaultMessage) { }

    public ValueObjectValidationException(string message)
        : base(message) { }

    public ValueObjectValidationException(Exception innerException)
        : base(DefaultMessage, innerException) { }

    public ValueObjectValidationException(string message, Exception innerException)
        : base(message, innerException) { }
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

        var underlyingType = LegacyPhoneNumber.UnderlyingType;
        object? underlyingValue;
    
        switch (Type.GetTypeCode(underlyingType)) {
            case TypeCode.Boolean:
                if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetBoolean();
                break;
            case TypeCode.Byte:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetByte();
                break;
            case TypeCode.Char:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                var charStr = reader.GetString();
                if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                    throw new System.Text.Json.JsonException($"Cannot convert '{charStr}' to char.");
                underlyingValue = charStr[0];
                break;
            case TypeCode.Decimal:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetDecimal();
                break;
            case TypeCode.Double:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetDouble();
                break;
            case TypeCode.Single:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetSingle();
                break;
            case TypeCode.Int16:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetInt16();
                break;
            case TypeCode.Int32:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetInt32();
                break;
            case TypeCode.Int64:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetInt64();
                break;
            case TypeCode.String:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetString();
                break;
            case TypeCode.DateTime:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                underlyingValue = reader.GetDateTime();
                break;
            default:
                if (underlyingType == typeof(Guid)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                    var guidStr = reader.GetString();
                    if (!Guid.TryParse(guidStr, out var guidValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{guidStr}' to Guid.");
                    underlyingValue = guidValue;
                } else if (underlyingType == typeof(DateTimeOffset)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                    underlyingValue = reader.GetDateTimeOffset();
                } else if (underlyingType == typeof(TimeSpan)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                    var tsStr = reader.GetString();
                    if (!TimeSpan.TryParse(tsStr, out var tsValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{tsStr}' to TimeSpan.");
                    underlyingValue = tsValue;
                } else if (underlyingType == typeof(TimeOnly)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                    var timeStr = reader.GetString();
                    if (!TimeOnly.TryParse(timeStr, out var timeValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{timeStr}' to TimeOnly.");
                    underlyingValue = timeValue;
                } else if (underlyingType == typeof(Uri)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for LegacyPhoneNumber.");
                    var uriStr = reader.GetString();
                    if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{uriStr}' to Uri.");
                    underlyingValue = uriValue;
                } else {
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for LegacyPhoneNumber.");
                }
                break;
        }
    
        try {
            var typedUnderlyingValue = (System.String)underlyingValue!;
            if (typedUnderlyingValue == default || underlyingValue is System.String suv && suv == System.String.Empty) {
                return LegacyPhoneNumber.Empty;
            }
            return LegacyPhoneNumber.From(typedUnderlyingValue);
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
        var underlyingType = LegacyPhoneNumber.UnderlyingType;
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
                } else {
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for LegacyPhoneNumber.");
                }
                break;
        }
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

            }
            
        }
        