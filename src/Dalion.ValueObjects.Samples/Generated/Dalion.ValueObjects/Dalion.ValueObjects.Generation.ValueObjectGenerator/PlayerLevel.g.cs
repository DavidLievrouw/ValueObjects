
        #nullable enable

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

                public System.Int32 Value => _value;

                
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public PlayerLevel()
                {
                    _value = default;
                    _initialized = false;
                    _isNullOrEmpty = false;
                    _validation ??= Validate(_value);
                }

                private PlayerLevel(System.Int32 value) {
                    
                    _initialized = true;
                    _value = value;
                    _isNullOrEmpty = false;
                    _validation ??= Validate(_value);
                }

                private PlayerLevel(System.Int32 value, bool validation) {
                    
                    if (validation) {
                        
                  _validation = Validate(value);
                  if (!_validation.IsSuccess && value != default && !PlayerLevelPreSetValueCache.PlayerLevelPreSetValues.TryGetValue(value, out _)) {
                      throw new System.InvalidOperationException(_validation.ErrorMessage);
                  }
                    }
                    _initialized = true;
                    _value = value;
                    _isNullOrEmpty = false;
                    _validation ??= Validate(_value);
                }

                public static PlayerLevel From(System.Int32 value) {
                    if (value == default) {
                        
                  var validationResult = Validate(value);
                  if (!validationResult.IsSuccess && !PlayerLevelPreSetValueCache.PlayerLevelPreSetValues.TryGetValue(value, out _)) {
                      throw new System.InvalidOperationException(validationResult.ErrorMessage);
                  }
                        return Unspecified;
                    }

                    return new PlayerLevel(value, validation: true);
                }

                public static bool TryFrom(System.Int32 value, out PlayerLevel result) {
                    result = value == default ? Unspecified : new PlayerLevel(value, validation: false);
                    return result.IsInitialized() && (Validate(result._value).IsSuccess || PlayerLevelPreSetValueCache.PlayerLevelPreSetValues.TryGetValue(value, out _));
                }


                public static PlayerLevel Unspecified { get; } = new PlayerLevel(default, validation: false);

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
            
                    return EqualityComparer<System.Int32>.Default.Equals(this._value, other.Value.Value);
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
            
                    return EqualityComparer<System.Int32>.Default.Equals(this._value, other.Value);
                }
            
                public bool Equals(PlayerLevel? other, IEqualityComparer<PlayerLevel> comparer)
                {
                    if (other is null) return false;
                    return comparer.Equals(this, other.Value);
                }
            
                /// <inheritdoc />
                public override int GetHashCode() {
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<System.Int32>.Default.GetHashCode(this._value);
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

        var underlyingType = PlayerLevel.UnderlyingType;
        object? underlyingValue;
    
        switch (Type.GetTypeCode(underlyingType)) {
            case TypeCode.Boolean:
                if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetBoolean();
                break;
            case TypeCode.Byte:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetByte();
                break;
            case TypeCode.Char:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                var charStr = reader.GetString();
                if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                    throw new System.Text.Json.JsonException($"Cannot convert '{charStr}' to char.");
                underlyingValue = charStr[0];
                break;
            case TypeCode.Decimal:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetDecimal();
                break;
            case TypeCode.Double:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetDouble();
                break;
            case TypeCode.Single:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetSingle();
                break;
            case TypeCode.Int16:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetInt16();
                break;
            case TypeCode.Int32:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetInt32();
                break;
            case TypeCode.Int64:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetInt64();
                break;
            case TypeCode.String:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetString();
                break;
            case TypeCode.DateTime:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                underlyingValue = reader.GetDateTime();
                break;
            default:
                if (underlyingType == typeof(Guid)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                    var guidStr = reader.GetString();
                    if (!Guid.TryParse(guidStr, out var guidValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{guidStr}' to Guid.");
                    underlyingValue = guidValue;
                } else if (underlyingType == typeof(DateTimeOffset)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                    underlyingValue = reader.GetDateTimeOffset();
                } else if (underlyingType == typeof(TimeSpan)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                    var tsStr = reader.GetString();
                    if (!TimeSpan.TryParse(tsStr, out var tsValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{tsStr}' to TimeSpan.");
                    underlyingValue = tsValue;
                } else if (underlyingType == typeof(TimeOnly)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                    var timeStr = reader.GetString();
                    if (!TimeOnly.TryParse(timeStr, out var timeValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{timeStr}' to TimeOnly.");
                    underlyingValue = timeValue;
                } else if (underlyingType == typeof(Uri)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for PlayerLevel.");
                    var uriStr = reader.GetString();
                    if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{uriStr}' to Uri.");
                    underlyingValue = uriValue;
                } else {
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for PlayerLevel.");
                }
                break;
        }
    
        try {
            var typedUnderlyingValue = (System.Int32)underlyingValue!;
            if (typedUnderlyingValue.Equals(PlayerLevel.Unspecified.Value)) {
                return PlayerLevel.Unspecified;
            }
            if (PlayerLevel.TryFrom(typedUnderlyingValue, out var result)) {
                return result;
            }
            if (PlayerLevelPreSetValueCache.PlayerLevelPreSetValues.TryGetValue(typedUnderlyingValue, out var constant)) {
                return constant;
            }
            throw new System.Text.Json.JsonException($"No matching PlayerLevel pre-set value found for value '{typedUnderlyingValue}'.");
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
        var underlyingType = PlayerLevel.UnderlyingType;
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
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for PlayerLevel.");
                }
                break;
        }
    }
}

                
private class PlayerLevelTypeConverter : System.ComponentModel.TypeConverter
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

        return underlyingValue == default ? Unspecified : From((System.Int32)underlyingValue);
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
        return destinationType == UnderlyingType;
    }
    
    public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
    {
        if (!CanConvertTo(context, destinationType))
        {
            throw new NotSupportedException($"Cannot convert to type '{destinationType}'.");
        }

        if (value is PlayerLevel vo)
        {
            return vo.Value;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}

                
private static class PlayerLevelPreSetValueCache {
    public static readonly Dictionary<System.Int32, PlayerLevel> PlayerLevelPreSetValues;

    static PlayerLevelPreSetValueCache()
    {
        PlayerLevelPreSetValues = typeof(PlayerLevel)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(PlayerLevel) && f.IsInitOnly)
            .Select(f => {
                var val = f.GetValue(null);
                if (val is null) return PlayerLevel.Unspecified;
                return (PlayerLevel)val;
            })
            .Where(o => o.IsInitialized())
            .ToDictionary(o => o.Value, o => o);
        PlayerLevelPreSetValues[PlayerLevel.Unspecified.Value] = PlayerLevel.Unspecified;
    }
}
            }
            
        }
        