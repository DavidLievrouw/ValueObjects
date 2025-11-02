using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class JsonConverterFragmentProvider : IFragmentProvider
{
    public string ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        var typeName = config.TypeName;
        var valueTypeName = config.UnderlyingTypeName;

        string readCode;
        string writeCode;

        switch (config.UnderlyingType.SpecialType)
        {
            case SpecialType.System_Boolean:
                readCode = "underlyingValue = reader.GetBoolean();";
                writeCode = "writer.WriteBooleanValue((bool)underlyingValue);";
                break;
            case SpecialType.System_Byte:
                readCode = "underlyingValue = reader.GetByte();";
                writeCode = "writer.WriteNumberValue((byte)underlyingValue);";
                break;
            case SpecialType.System_Char:
                readCode =
                    @"
                var charStr = reader.GetString();
                if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                    throw new System.Text.Json.JsonException($""Cannot convert '{charStr}' to char."");
                underlyingValue = charStr[0];";
                writeCode = "writer.WriteStringValue(((char)underlyingValue).ToString());";
                break;
            case SpecialType.System_Decimal:
                readCode = "underlyingValue = reader.GetDecimal();";
                writeCode = "writer.WriteNumberValue((decimal)underlyingValue);";
                break;
            case SpecialType.System_Double:
                readCode = "underlyingValue = reader.GetDouble();";
                writeCode = "writer.WriteNumberValue((double)underlyingValue);";
                break;
            case SpecialType.System_Single:
                readCode = "underlyingValue = reader.GetSingle();";
                writeCode = "writer.WriteNumberValue((float)underlyingValue);";
                break;
            case SpecialType.System_Int16:
                readCode = "underlyingValue = reader.GetInt16();";
                writeCode = "writer.WriteNumberValue((short)underlyingValue);";
                break;
            case SpecialType.System_Int32:
                readCode = "underlyingValue = reader.GetInt32();";
                writeCode = "writer.WriteNumberValue((int)underlyingValue);";
                break;
            case SpecialType.System_Int64:
                readCode = "underlyingValue = reader.GetInt64();";
                writeCode = "writer.WriteNumberValue((long)underlyingValue);";
                break;
            case SpecialType.System_String:
                readCode = "underlyingValue = reader.GetString();";
                writeCode = "writer.WriteStringValue((string)underlyingValue);";
                break;
            case SpecialType.System_DateTime:
                readCode = "underlyingValue = reader.GetDateTime();";
                writeCode = "writer.WriteStringValue(((DateTime)underlyingValue));";
                break;
            default:
                // Handle non-SpecialType types by name
                switch (config.UnderlyingType.Name)
                {
                    case "Guid":
                        readCode =
                            @"
                var guidStr = reader.GetString();
                if (!Guid.TryParse(guidStr, out var guidValue))
                    throw new System.Text.Json.JsonException($""Cannot convert '{guidStr}' to Guid."");
                underlyingValue = guidValue;";
                        writeCode = "writer.WriteStringValue(((Guid)underlyingValue));";
                        break;
                    case "DateTimeOffset":
                        readCode = "underlyingValue = reader.GetDateTimeOffset();";
                        writeCode = "writer.WriteStringValue(((DateTimeOffset)underlyingValue));";
                        break;
                    case "TimeSpan":
                        readCode =
                            @"
                var tsStr = reader.GetString();
                if (!TimeSpan.TryParse(tsStr, out var tsValue))
                    throw new System.Text.Json.JsonException($""Cannot convert '{tsStr}' to TimeSpan."");
                underlyingValue = tsValue;";
                        writeCode =
                            "writer.WriteStringValue(((TimeSpan)underlyingValue).ToString());";
                        break;
                    case "TimeOnly":
                        readCode =
                            @"
                var timeStr = reader.GetString();
                if (!TimeOnly.TryParse(timeStr, out var timeValue))
                    throw new System.Text.Json.JsonException($""Cannot convert '{timeStr}' to TimeOnly."");
                underlyingValue = timeValue;";
                        writeCode =
                            "writer.WriteStringValue(((TimeOnly)underlyingValue).ToString());";
                        break;
                    case "DateOnly":
                        readCode =
                            @"
                var dateStr = reader.GetString();
                if (!DateOnly.TryParse(dateStr, out var dateValue))
                    throw new System.Text.Json.JsonException($""Cannot convert '{dateStr}' to DateOnly."");
                underlyingValue = dateValue;";
                        writeCode =
                            "writer.WriteStringValue(((DateOnly)underlyingValue).ToString(\"yyyy-MM-dd\"));";
                        break;
                    default:
                        readCode =
                            "throw new System.Text.Json.JsonException($\"Unsupported underlying type for "
                            + typeName
                            + ".\");";
                        writeCode =
                            "throw new System.Text.Json.JsonException($\"Unsupported underlying type for "
                            + typeName
                            + ".\");";
                        break;
                }

                break;
        }

        var converterClass =
            $@"
        private class {typeName}SystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<{typeName}>
        {{
            public override {typeName} Read(
                ref System.Text.Json.Utf8JsonReader reader,
                Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options
            )
            {{
                if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {{
                    return new {typeName}();
                }}

                object? underlyingValue;
                {readCode}

                try {{
                    var typedUnderlyingValue = ({valueTypeName})underlyingValue!;
                    if ({typeName}.TryFrom(typedUnderlyingValue, out var result)) {{
                        return result;
                    }}
                    throw new System.Text.Json.JsonException($""No matching {typeName} pre-set value found for value '{{typedUnderlyingValue}}', or the underlying value is invalid."");
                }} catch (System.Exception e) {{
                    throw new System.Text.Json.JsonException(""Could not create an initialized instance of {typeName}."", e);
                }}
            }}

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                {typeName} value,
                System.Text.Json.JsonSerializerOptions options
            )
            {{
                object? underlyingValue = value.IsInitialized()
                    ? value.Value
                    : null;

                if (underlyingValue == null) {{
                    writer.WriteNullValue();
                    return;
                }}

                {writeCode}
            }}
        }}
    ";

        return converterClass.Trim();
    }
}