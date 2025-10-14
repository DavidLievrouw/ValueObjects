
        #nullable enable

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

                public System.Guid Value => _value;

                
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public TenantId()
                {
                    _value = default;
                    _initialized = false;
                    _isNullOrEmpty = false;
                    _validation ??= Validation.Ok;
                }

                private TenantId(System.Guid value) {
                    
                    _initialized = true;
                    _value = value;
                    _isNullOrEmpty = false;
                    _validation ??= Validation.Ok;
                }

                private TenantId(System.Guid value, bool validation) {
                    
                    if (validation) {
                        
                    }
                    _initialized = true;
                    _value = value;
                    _isNullOrEmpty = false;
                    _validation ??= Validation.Ok;
                }

                public static TenantId From(System.Guid value) {
                    if (value == default) {
                        
                        return Empty;
                    }

                    return new TenantId(value, validation: true);
                }

                public static bool TryFrom(System.Guid value, out TenantId result) {
                    result = value == default ? Empty : new TenantId(value, validation: false);
                    return result.IsInitialized();
                }


                public static TenantId Empty { get; } = new TenantId(default, validation: false);

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
            
                    return EqualityComparer<System.Guid>.Default.Equals(this._value, other.Value.Value);
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
            
                    return EqualityComparer<System.Guid>.Default.Equals(this._value, other.Value);
                }
            
                public bool Equals(TenantId? other, IEqualityComparer<TenantId> comparer)
                {
                    if (other is null) return false;
                    return comparer.Equals(this, other.Value);
                }
            
                /// <inheritdoc />
                public override int GetHashCode() {
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<System.Guid>.Default.GetHashCode(this._value);
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
                    return Value.ToString(format: null, provider: System.Globalization.CultureInfo.InvariantCulture);
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

        var underlyingType = TenantId.UnderlyingType;
        object? underlyingValue;
    
        switch (Type.GetTypeCode(underlyingType)) {
            case TypeCode.Boolean:
                if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetBoolean();
                break;
            case TypeCode.Byte:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetByte();
                break;
            case TypeCode.Char:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                var charStr = reader.GetString();
                if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                    throw new System.Text.Json.JsonException($"Cannot convert '{charStr}' to char.");
                underlyingValue = charStr[0];
                break;
            case TypeCode.Decimal:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetDecimal();
                break;
            case TypeCode.Double:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetDouble();
                break;
            case TypeCode.Single:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetSingle();
                break;
            case TypeCode.Int16:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetInt16();
                break;
            case TypeCode.Int32:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetInt32();
                break;
            case TypeCode.Int64:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetInt64();
                break;
            case TypeCode.String:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetString();
                break;
            case TypeCode.DateTime:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                underlyingValue = reader.GetDateTime();
                break;
            default:
                if (underlyingType == typeof(Guid)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                    var guidStr = reader.GetString();
                    if (!Guid.TryParse(guidStr, out var guidValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{guidStr}' to Guid.");
                    underlyingValue = guidValue;
                } else if (underlyingType == typeof(DateTimeOffset)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                    underlyingValue = reader.GetDateTimeOffset();
                } else if (underlyingType == typeof(TimeSpan)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                    var tsStr = reader.GetString();
                    if (!TimeSpan.TryParse(tsStr, out var tsValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{tsStr}' to TimeSpan.");
                    underlyingValue = tsValue;
                } else if (underlyingType == typeof(TimeOnly)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                    var timeStr = reader.GetString();
                    if (!TimeOnly.TryParse(timeStr, out var timeValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{timeStr}' to TimeOnly.");
                    underlyingValue = timeValue;
                } else if (underlyingType == typeof(Uri)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for TenantId.");
                    var uriStr = reader.GetString();
                    if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{uriStr}' to Uri.");
                    underlyingValue = uriValue;
                } else {
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for TenantId.");
                }
                break;
        }
    
        try {
            var typedUnderlyingValue = (System.Guid)underlyingValue!;
            if (typedUnderlyingValue.Equals(TenantId.Empty.Value)) {
                return TenantId.Empty;
            }
            if (TenantId.TryFrom(typedUnderlyingValue, out var result)) {
                return result;
            }
            if (TenantIdPreSetValueCache.TenantIdPreSetValues.TryGetValue(typedUnderlyingValue, out var constant)) {
                return constant;
            }
            throw new System.Text.Json.JsonException($"No matching TenantId pre-set value found for value '{typedUnderlyingValue}'.");
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
        var underlyingType = TenantId.UnderlyingType;
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
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for TenantId.");
                }
                break;
        }
    }
}

                
private class TenantIdTypeConverter : System.ComponentModel.TypeConverter
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

        return underlyingValue == default ? Empty : From((System.Guid)underlyingValue);
    }

    private object? GetUnderlyingValue(object? value) {{
        if (value == null) {{
            return default(System.Guid);
        }}

        if (value is System.Guid v) {
            return v;
        }
        
        if (Type.GetTypeCode(typeof(System.Guid)) == TypeCode.Object) {
            throw new NotSupportedException($"Cannot convert value of type '{value?.GetType()}' to 'System.Guid'.");
        }
        
        return Convert.ChangeType(value, typeof(System.Guid));
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

        if (value is TenantId vo)
        {
            return vo.Value;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}

                
private static class TenantIdPreSetValueCache {
    public static readonly Dictionary<System.Guid, TenantId> TenantIdPreSetValues;

    static TenantIdPreSetValueCache()
    {
        TenantIdPreSetValues = typeof(TenantId)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(TenantId) && f.IsInitOnly)
            .Select(f => {
                var val = f.GetValue(null);
                if (val is null) return TenantId.Empty;
                return (TenantId)val;
            })
            .Where(o => o.IsInitialized())
            .ToDictionary(o => o.Value, o => o);
        TenantIdPreSetValues[TenantId.Empty.Value] = TenantId.Empty;
    }
}
            }
            
        }
        