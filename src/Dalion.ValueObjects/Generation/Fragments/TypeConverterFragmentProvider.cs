using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class TypeConverterFragmentProvider : IFragmentProvider
{
    public string ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        var getUnderlyingValue =
            config.UnderlyingType.SpecialType == SpecialType.System_String
                ? "var underlyingValue = s;"
                : $"var underlyingValue = {config.UnderlyingTypeName}.Parse(s, culture ?? System.Globalization.CultureInfo.InvariantCulture);";

        string getUnderlyingStringValue;
        if (config.UnderlyingType.SpecialType == SpecialType.System_String)
        {
            getUnderlyingStringValue = "return vo.Value;";
        }
        else if (config.UnderlyingType.Name == "DateOnly")
        {
            getUnderlyingStringValue = "return vo.Value.ToString(\"yyyy-MM-dd\");";
        }
        else
        {
            getUnderlyingStringValue =
                "return vo.ToString(culture ?? System.Globalization.CultureInfo.InvariantCulture);";
        }

        return $@"
        private class {config.TypeName}TypeConverter : System.ComponentModel.TypeConverter
        {{
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
            {{
                return sourceType is not null && (sourceType.IsAssignableFrom(typeof({config.TypeName})) || sourceType.IsAssignableFrom(UnderlyingType) || sourceType == typeof(string));
            }}
            
            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            {{
                if (value is {config.TypeName} vo)
                {{
                    return vo;
                }}

                if (value == default) 
                {{
                    return {config.EmptyValueName};
                }}
        
                if (value is {config.UnderlyingTypeName} correctlyTypedValue)
                {{
                    return correctlyTypedValue == default 
                        ? {config.EmptyValueName} 
                        : From(correctlyTypedValue);
                }}
        
                if (value is string s)
                {{
                    {getUnderlyingValue}
                    return underlyingValue == default 
                        ? {config.EmptyValueName} 
                        : From(({config.UnderlyingTypeName})underlyingValue);
                }}
    
                throw new NotSupportedException($@""Cannot convert from type '{{value?.GetType()}}'."");
            }}
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
            {{
                return destinationType is not null && (destinationType.IsAssignableFrom(typeof({config.TypeName})) || destinationType.IsAssignableFrom(UnderlyingType) || destinationType == typeof(string));
            }}
            
            public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            {{
                if (destinationType.IsAssignableFrom(typeof({config.TypeName})))
                {{
                    if (value is {config.TypeName} vo)
                    {{
                        return vo;
                    }}
                    if (value is {config.UnderlyingTypeName} correctlyTypedValue)
                    {{
                        return From(correctlyTypedValue);
                    }}
                    if (value is string s)
                    {{
                        {getUnderlyingValue}
                        return From(underlyingValue);
                    }}
                }}

                if (destinationType.IsAssignableFrom(UnderlyingType))
                {{
                    if (value is {config.TypeName} vo)
                    {{
                        return vo.Value;
                    }}
                    if (value is {config.UnderlyingTypeName} correctlyTypedValue)
                    {{
                        return correctlyTypedValue;
                    }}
                    if (value is string s)
                    {{
                        {getUnderlyingValue}
                        return underlyingValue;
                    }}
                    return base.ConvertTo(context, culture ?? System.Globalization.CultureInfo.InvariantCulture, value, destinationType);
                }}

                if (destinationType == typeof(string))
                {{
                    if (value is {config.TypeName} vo)
                    {{
                        {getUnderlyingStringValue}
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
".Trim();
    }
}
