namespace Dalion.ValueObjects.Generation.Fragments;

internal class TypeConverterFragmentProvider : IFragmentProvider
{
    private const string TypeConverterTemplate =
        @"
        private class {{typeName}}TypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
            {
                return sourceType == UnderlyingType || sourceType == typeof(string);
            }
            
            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            {
                if (value == null) return {{emptyValueName}};
        
                if (value.GetType() == UnderlyingType)
                {
                    var underlyingValue = GetUnderlyingValue(value);
                    return underlyingValue == default ? {{emptyValueName}} : From(({{valueTypeName}})underlyingValue);
                }
        
                if (value is string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return {{emptyValueName}};
                    object underlyingValue;
                    if (UnderlyingType == typeof(Guid)) {
                        underlyingValue = Guid.Parse(s);
                    } else if (UnderlyingType == typeof(DateOnly)) {
                        underlyingValue = DateOnly.Parse(s, culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    } else {
                        underlyingValue = Convert.ChangeType(s, UnderlyingType, culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }
                    return From(({{valueTypeName}})underlyingValue);
                }
    
                throw new NotSupportedException($@""Cannot convert from type '{value?.GetType()}'."");
            }

            private object? GetUnderlyingValue(object? value) {{
                if (value == null) {{
                    return default({{valueTypeName}});
                }}
        
                if (value is {{valueTypeName}} v) {
                    return v;
                }
                
                if (Type.GetTypeCode(typeof({{valueTypeName}})) == TypeCode.Object) {
                    throw new NotSupportedException($""Cannot convert value of type '{value?.GetType()}' to '{{valueTypeName}}'."");
                }
                
                return Convert.ChangeType(value, typeof({{valueTypeName}}));
            }}
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
            {
                return destinationType == UnderlyingType || destinationType == typeof(string);
            }
            
            public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
            {
                if (destinationType == UnderlyingType)
                {
                    if (value is {{typeName}} vo)
                    {
                        return vo.Value;
                    }
                    return base.ConvertTo(context, culture ?? System.Globalization.CultureInfo.InvariantCulture, value, destinationType);
                }

                if (destinationType == typeof(string))
                {
                    if (value is {{typeName}} vo)
                    {
                        return vo.ToString(culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (value is System.IFormattable f)
                    {
                        return f.ToString(format: null, formatProvider: culture ?? System.Globalization.CultureInfo.InvariantCulture);
                    }
                    return value?.ToString();
                }

                throw new NotSupportedException($@""Cannot convert to type '{destinationType}'."");
            }
        }
";
    
    public string ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return TypeConverterTemplate
            .Replace("{{typeName}}", config.TypeName)
            .Replace("{{valueTypeName}}", config.UnderlyingTypeName)
            .Replace("{{emptyValueName}}", config.EmptyValueName)
            .Trim();
    }
}