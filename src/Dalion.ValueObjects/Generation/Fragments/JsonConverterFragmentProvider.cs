namespace Dalion.ValueObjects.Generation.Fragments;

internal class JsonConverterFragmentProvider : IFragmentProvider
{
    private const string JsonConverterTemplate =
        @"
        private class {{typeName}}SystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<{{typeName}}>
        {
            public override {{typeName}} Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
                    return new {{typeName}}();
                }
        
                var underlyingType = {{typeName}}.UnderlyingType;
                object? underlyingValue;
            
                switch (Type.GetTypeCode(underlyingType)) {
                    case TypeCode.Boolean:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetBoolean();
                        break;
                    case TypeCode.Byte:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetByte();
                        break;
                    case TypeCode.Char:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        var charStr = reader.GetString();
                        if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                            throw new System.Text.Json.JsonException($""Cannot convert '{charStr}' to char."");
                        underlyingValue = charStr[0];
                        break;
                    case TypeCode.Decimal:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetDecimal();
                        break;
                    case TypeCode.Double:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetDouble();
                        break;
                    case TypeCode.Single:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetSingle();
                        break;
                    case TypeCode.Int16:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetInt16();
                        break;
                    case TypeCode.Int32:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetInt32();
                        break;
                    case TypeCode.Int64:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetInt64();
                        break;
                    case TypeCode.String:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetString();
                        break;
                    case TypeCode.DateTime:
                        if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                            throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                        underlyingValue = reader.GetDateTime();
                        break;
                    default:
                        if (underlyingType == typeof(Guid)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                            var guidStr = reader.GetString();
                            if (!Guid.TryParse(guidStr, out var guidValue))
                                throw new System.Text.Json.JsonException($""Cannot convert '{guidStr}' to Guid."");
                            underlyingValue = guidValue;
                        } else if (underlyingType == typeof(DateTimeOffset)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                            underlyingValue = reader.GetDateTimeOffset();
                        } else if (underlyingType == typeof(TimeSpan)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                            var tsStr = reader.GetString();
                            if (!TimeSpan.TryParse(tsStr, out var tsValue))
                                throw new System.Text.Json.JsonException($""Cannot convert '{tsStr}' to TimeSpan."");
                            underlyingValue = tsValue;
                        } else if (underlyingType == typeof(TimeOnly)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                            var timeStr = reader.GetString();
                            if (!TimeOnly.TryParse(timeStr, out var timeValue))
                                throw new System.Text.Json.JsonException($""Cannot convert '{timeStr}' to TimeOnly."");
                            underlyingValue = timeValue;
                        } else if (underlyingType == typeof(Uri)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                            var uriStr = reader.GetString();
                            if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                                throw new System.Text.Json.JsonException($""Cannot convert '{uriStr}' to Uri."");
                            underlyingValue = uriValue;
                        } else if (underlyingType == typeof(DateOnly)) {
                            if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                                throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                            var dateStr = reader.GetString();
                            if (!DateOnly.TryParse(dateStr, out var dateValue))
                                throw new System.Text.Json.JsonException($""Cannot convert '{dateStr}' to DateOnly."");
                            underlyingValue = dateValue;
                        } else {
                            throw new System.Text.Json.JsonException($""Unsupported underlying type for {{typeName}}."");
                        }
                        break;
                }
            
                try {
                    var typedUnderlyingValue = ({{valueTypeName}})underlyingValue!;
                    if (typedUnderlyingValue.Equals({{typeName}}.{{emptyValueName}}.Value)) {
                        return {{typeName}}.{{emptyValueName}};
                    }
                    if ({{typeName}}.TryFrom(typedUnderlyingValue, out var result)) {
                        return result;
                    }
                    if ({{typeName}}PreSetValueCache.{{typeName}}PreSetValues.TryGetValue(typedUnderlyingValue, out var constant)) {
                        return constant;
                    }
                    throw new System.Text.Json.JsonException($""No matching {{typeName}} pre-set value found for value '{typedUnderlyingValue}'."");
                } catch (System.Exception e) {
                    throw new System.Text.Json.JsonException(""Could not create an initialized instance of {{typeName}}."", e);
                }
            }
            
            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                {{typeName}} value,
                System.Text.Json.JsonSerializerOptions options
            )
            {
                var underlyingType = {{typeName}}.UnderlyingType;
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
                            writer.WriteStringValue(((DateOnly)underlyingValue).ToString(""yyyy-MM-dd""));
                        } else {
                            throw new System.Text.Json.JsonException($""Unsupported underlying type for {{typeName}}."");
                        }
                        break;
                }
            }
        }
";
    
    public string ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return JsonConverterTemplate
            .Replace("{{typeName}}", config.TypeName)
            .Replace("{{valueTypeName}}", config.UnderlyingTypeName)
            .Replace("{{emptyValueName}}", config.EmptyValueName)
            .Trim();
    }
}