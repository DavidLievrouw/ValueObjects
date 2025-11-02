namespace Dalion.ValueObjects.Generation.Fragments;

internal class TypeConverterFragmentProvider : IFragmentProvider
{
    public string ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        var convertFromUnderlyingValue =
            config.UnderlyingType.SpecialType == Microsoft.CodeAnalysis.SpecialType.System_String
                ? "var underlyingValue = s;"
                : $"var underlyingValue = {config.UnderlyingTypeName}.Parse(s, culture ?? System.Globalization.CultureInfo.InvariantCulture);";

        return $@"
        private class {config.TypeName}TypeConverter : System.ComponentModel.TypeConverter
        {{
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
            {{
                return sourceType == UnderlyingType || sourceType == typeof(string);
            }}
            
            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            {{
                if (value == null) return {config.EmptyValueName};
        
                if (value.GetType() == UnderlyingType)
                {{
                    var underlyingValue = GetUnderlyingValue(value);
                    return underlyingValue == default ? {config.EmptyValueName} : From(({config.UnderlyingTypeName})underlyingValue);
                }}
        
                if (value is string s)
                {{
                    {convertFromUnderlyingValue}
                    return underlyingValue == default ? {config.EmptyValueName} : From(({config.UnderlyingTypeName})underlyingValue);
                }}
    
                throw new NotSupportedException($@""Cannot convert from type '{{value?.GetType()}}'."");
            }}

            private object? GetUnderlyingValue(object? value) {{
                if (value == null) {{
                    return default({config.UnderlyingTypeName});
                }}
        
                if (value is {config.UnderlyingTypeName} v) {{
                    return v;
                }}
                
                if (Type.GetTypeCode(typeof({config.UnderlyingTypeName})) == TypeCode.Object) {{
                    throw new NotSupportedException($""Cannot convert value of type '{{value?.GetType()}}' to '{config.UnderlyingTypeName}'."");
                }}
                
                return Convert.ChangeType(value, typeof({config.UnderlyingTypeName}));
            }}
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
            {{
                return destinationType == UnderlyingType || destinationType == typeof(string);
            }}
            
            public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            {{
                if (destinationType == UnderlyingType)
                {{
                    if (value is {config.TypeName} vo)
                    {{
                        return vo.Value;
                    }}
                    return base.ConvertTo(context, culture ?? System.Globalization.CultureInfo.InvariantCulture, value, destinationType);
                }}

                if (destinationType == typeof(string))
                {{
                    if (value is {config.TypeName} vo)
                    {{
                        return vo.ToString(culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }}
                    if (value is System.IFormattable f)
                    {{
                        return f.ToString(format: null, formatProvider: culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }}
                    return value?.ToString();
                }}

                throw new NotSupportedException($@""Cannot convert to type '{{destinationType}}'."");
            }}
        }}
";
    }
}
