
        #nullable enable

        namespace Dalion.ValueObjects.Samples {
            
            [System.Diagnostics.DebuggerDisplay("Password {Value}")]
            [System.Text.Json.Serialization.JsonConverter(typeof(PasswordSystemTextJsonConverter))]
            [System.ComponentModel.TypeConverter(typeof(PasswordTypeConverter))]
            public partial record struct Password : IEquatable<Password>
 {
                private readonly System.String _value;
                private static readonly Type UnderlyingType = typeof(System.String);

                public System.String Value => _value;

                
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Password()
                {
                    _value = System.String.Empty;
                }

                [System.Diagnostics.DebuggerStepThrough]
                private Password(System.String value, bool validation = true) {
                    if (validation) {
                        
                  var validationResult = Validate(value);
                  if (!validationResult.IsSuccess) {
                      throw new System.InvalidOperationException(validationResult.ErrorMessage);
                  }
                    }
                    _value = value ?? System.String.Empty;
                }

                public static Password From(System.String? value) {
                    if (string.IsNullOrWhiteSpace(value)) {
                        return Empty;
                    }

                    return new Password(value);
                }

                public static bool TryFrom(System.String? value, out Password result) {
                    result = string.IsNullOrWhiteSpace(value) ? Empty : new Password(value, validation: false);
                    return result.IsInitialized() && Validate(result._value).IsSuccess;
                }


                public static Password Empty => new Password(System.String.Empty, validation: false);

                public bool IsInitialized() => !System.String.IsNullOrWhiteSpace(_value);

                
                /// <inheritdoc />
                public bool Equals(Password? other)
                {
                    if (other == null) return false;

                    if (!other.Value.IsInitialized())
                    {
                        return !IsInitialized();
                    }

                    if (other.Value.IsInitialized() != IsInitialized())
                    {
                        return false;
                    }
            
                    return System.String.IsNullOrWhiteSpace(other.Value.Value)
                        ? System.String.IsNullOrWhiteSpace(this._value)
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
            
                    return System.String.IsNullOrWhiteSpace(other.Value)
                        ? System.String.IsNullOrWhiteSpace(this._value)
                        : System.String.Equals(this._value, other.Value, System.StringComparison.Ordinal);
                }
            
                public bool Equals(Password? other, IEqualityComparer<Password> comparer)
                {
                    if (other == null) return false;
                    return comparer.Equals(this, other.Value);
                }
            
                /// <inheritdoc />
                public override int GetHashCode() {
                    if (!IsInitialized()) return 0;
                    return StringComparer.Ordinal.GetHashCode(this._value);
                }

                
                /// <inheritdoc />
                public bool Equals(System.String? other)
                {
                    return System.String.IsNullOrWhiteSpace(other)
                        ? System.String.IsNullOrWhiteSpace(this._value)
                        : System.String.Equals(this._value, other, System.StringComparison.Ordinal);
                }
            
                public bool Equals(System.String? primitive, StringComparer comparer)
                {
                    return comparer.Equals(this.Value, primitive);
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
                    return Value.ToString() ?? "";
                }}

                /// <inheritdoc cref="M:System.String.ToString(System.IFormatProvider)" />
                public string ToString(IFormatProvider? provider)
                {{
                    return Value.ToString(provider: provider) ?? "";
                }}


                
private class Validation
{
    public static readonly Validation Ok = new(string.Empty);

    private Validation(string reason)
    {
        ErrorMessage = reason;
    }

    public string ErrorMessage { get; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

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

                
private class PasswordSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<Password>
{
    public override Password Read(
        ref System.Text.Json.Utf8JsonReader reader,
        Type typeToConvert,
        System.Text.Json.JsonSerializerOptions options
    )
    {
        var underlyingType = Password.UnderlyingType;
        object? underlyingValue;
    
        switch (Type.GetTypeCode(underlyingType)) {
            case TypeCode.Boolean:
                if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetBoolean();
                break;
            case TypeCode.Byte:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetByte();
                break;
            case TypeCode.Char:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                var charStr = reader.GetString();
                if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                    throw new System.Text.Json.JsonException($"Cannot convert '{charStr}' to char.");
                underlyingValue = charStr[0];
                break;
            case TypeCode.Decimal:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetDecimal();
                break;
            case TypeCode.Double:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetDouble();
                break;
            case TypeCode.Single:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetSingle();
                break;
            case TypeCode.Int16:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetInt16();
                break;
            case TypeCode.Int32:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetInt32();
                break;
            case TypeCode.Int64:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetInt64();
                break;
            case TypeCode.String:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetString();
                break;
            case TypeCode.DateTime:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                underlyingValue = reader.GetDateTime();
                break;
            default:
                if (underlyingType == typeof(Guid)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                    var guidStr = reader.GetString();
                    if (!Guid.TryParse(guidStr, out var guidValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{guidStr}' to Guid.");
                    underlyingValue = guidValue;
                } else if (underlyingType == typeof(DateTimeOffset)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                    underlyingValue = reader.GetDateTimeOffset();
                } else if (underlyingType == typeof(TimeSpan)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                    var tsStr = reader.GetString();
                    if (!TimeSpan.TryParse(tsStr, out var tsValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{tsStr}' to TimeSpan.");
                    underlyingValue = tsValue;
                } else if (underlyingType == typeof(TimeOnly)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                    var timeStr = reader.GetString();
                    if (!TimeOnly.TryParse(timeStr, out var timeValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{timeStr}' to TimeOnly.");
                    underlyingValue = timeValue;
                } else if (underlyingType == typeof(Uri)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($"Unsupported JSON token type for Password.");
                    var uriStr = reader.GetString();
                    if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                        throw new System.Text.Json.JsonException($"Cannot convert '{uriStr}' to Uri.");
                    underlyingValue = uriValue;
                } else {
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for Password.");
                }
                break;
        }
    
        try {
            return Password.From((System.String)underlyingValue!);
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
        var underlyingType = Password.UnderlyingType;
        object underlyingValue = value.IsInitialized()
            ? value.Value
            : Password.Empty.Value;
    
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
                    throw new System.Text.Json.JsonException($"Unsupported underlying type for Password.");
                }
                break;
        }
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

            }
            
        }
        