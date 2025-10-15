namespace Dalion.ValueObjects.Generation.Fragments;

internal class TypeConverterFragmentProvider : IFragmentProvider
{
    private const string TypeConverterTemplate =
        @"
                private class {{typeName}}TypeConverter : System.ComponentModel.TypeConverter
                {
                    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
                    {
                        return sourceType == UnderlyingType;
                    }
                    
                    public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
                    {
                        if (value != null && !CanConvertFrom(context, value.GetType()))
                        {
                            throw new NotSupportedException($""Cannot convert from type '{value?.GetType()}'."");
                        }
                
                        var underlyingValue = GetUnderlyingValue(value);
                
                        return underlyingValue == default ? {{emptyValueName}} : From(({{valueTypeName}})underlyingValue);
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
                        return destinationType == UnderlyingType;
                    }
                    
                    public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
                    {
                        if (!CanConvertTo(context, destinationType))
                        {
                            throw new NotSupportedException($""Cannot convert to type '{destinationType}'."");
                        }
                
                        if (value is {{typeName}} vo)
                        {
                            return vo.Value;
                        }
                
                        return base.ConvertTo(context, culture, value, destinationType);
                    }
                }
";
    
    public string ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return TypeConverterTemplate
            .Replace("{{typeName}}", config.TypeName)
            .Replace("{{valueTypeName}}", config.UnderlyingTypeName)
            .Replace("{{emptyValueName}}", config.EmptyValueName);
    }
}