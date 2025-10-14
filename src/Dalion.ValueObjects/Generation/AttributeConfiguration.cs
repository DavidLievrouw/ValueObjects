using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dalion.ValueObjects.Generation;

internal class AttributeConfiguration
{
    public AttributeConfiguration(
        Type underlyingType,
        ComparisonGeneration comparison,
        CastOperator toUnderlyingTypeCasting,
        CastOperator fromUnderlyingTypeCasting,
        StringCaseSensitivity stringCaseSensitivity,
        UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration,
        FluentValidationExtensionsGeneration fluentValidationExtensionsGeneration,
        ParsableGeneration parsableGeneration,
        string emptyValueName
    )
    {
        UnderlyingType = underlyingType;
        Comparison = comparison;
        ToUnderlyingTypeCasting = toUnderlyingTypeCasting;
        FromUnderlyingTypeCasting = fromUnderlyingTypeCasting;
        CaseSensitivity = stringCaseSensitivity;
        UnderlyingTypeEqualityGeneration = underlyingTypeEqualityGeneration;
        FluentValidationExtensionsGeneration = fluentValidationExtensionsGeneration;
        ParsableGeneration = parsableGeneration;
        EmptyValueName = emptyValueName;
    }

    public Type UnderlyingType { get; }
    public ComparisonGeneration Comparison { get; }
    public CastOperator ToUnderlyingTypeCasting { get; }
    public CastOperator FromUnderlyingTypeCasting { get; }
    public StringCaseSensitivity CaseSensitivity { get; }
    public UnderlyingTypeEqualityGeneration UnderlyingTypeEqualityGeneration { get; }
    public FluentValidationExtensionsGeneration FluentValidationExtensionsGeneration { get; }
    public ParsableGeneration ParsableGeneration { get; }
    public string EmptyValueName { get; }

    public static AttributeConfiguration FromAttributeData(AttributeData attributeData)
    {
        var syntaxRef = attributeData.ApplicationSyntaxReference!;
        var attributeSyntax = (AttributeSyntax)syntaxRef.GetSyntax();
        var argumentExpressions = attributeSyntax.ArgumentList?.Arguments!;

        var typeSymbol = attributeData.AttributeClass!.TypeArguments[0];
        var typeName = typeSymbol.ToDisplayString();
        var underlyingType = typeName switch
        {
            "decimal" or "System.Decimal" => typeof(decimal),
            "int" or "System.Int32" => typeof(int),
            "string" or "System.String" => typeof(string),
            "bool" or "System.Boolean" => typeof(bool),
            "double" or "System.Double" => typeof(double),
            "float" or "System.Single" => typeof(float),
            "long" or "System.Int64" => typeof(long),
            "short" or "System.Int16" => typeof(short),
            "byte" or "System.Byte" => typeof(byte),
            "char" or "System.Char" => typeof(char),
            "DateTime" or "System.DateTime" => typeof(DateTime),
            "DateTimeOffset" or "System.DateTimeOffset" => typeof(DateTimeOffset),
            "Guid" or "System.Guid" => typeof(Guid),
            "TimeSpan" or "System.TimeSpan" => typeof(TimeSpan),
            "Uri" or "System.Uri" => typeof(Uri),
            _ => Type.GetType(typeName) ?? typeof(string),
        };

        var comparison = ValueObjectAttribute.DefaultComparison;
        var toUnderlyingTypeCasting = ValueObjectAttribute.DefaultToUnderlyingTypeCasting;
        var fromUnderlyingTypeCasting = ValueObjectAttribute.DefaultFromUnderlyingTypeCasting;
        var stringCaseSensitivity = ValueObjectAttribute.DefaultStringCaseSensitivity;
        var underlyingTypeEqualityGeneration = ValueObjectAttribute.DefaultUnderlyingTypeEqualityGeneration;
        var fluentValidationExtensionsGeneration = ValueObjectAttribute.DefaultFluentValidationExtensionsGeneration;
        var parsableGeneration = ValueObjectAttribute.DefaultParsableGeneration;
        var emptyValueName = ValueObjectAttribute.DefaultEmptyValueName;
        
        var pos = 0;
        if (argumentExpressions.HasValue)
        {
            foreach (var arg in argumentExpressions.Value)
            {
                if (arg.NameColon != null)
                {
                    // Named argument
                    var name = arg.GetFirstToken().ValueText;
                    var value = arg.GetLastToken().ValueText;

                    switch (name)
                    {
                        case "comparison":
                            comparison = (ComparisonGeneration)
                                Enum.Parse(typeof(ComparisonGeneration), value);
                            break;
                        case "toUnderlyingTypeCasting":
                            toUnderlyingTypeCasting = (CastOperator)Enum.Parse(typeof(CastOperator), value);
                            break;
                        case "fromUnderlyingTypeCasting":
                            fromUnderlyingTypeCasting = (CastOperator)
                                Enum.Parse(typeof(CastOperator), value);
                            break;
                        case "underlyingTypeEqualityGeneration":
                            underlyingTypeEqualityGeneration = (UnderlyingTypeEqualityGeneration)
                                Enum.Parse(typeof(UnderlyingTypeEqualityGeneration), value);
                            break;
                        case "stringCaseSensitivity":
                            stringCaseSensitivity = (StringCaseSensitivity)
                                Enum.Parse(typeof(StringCaseSensitivity), value);
                            break;
                        case "fluentValidationExtensionsGeneration":
                            fluentValidationExtensionsGeneration = (FluentValidationExtensionsGeneration)
                                Enum.Parse(typeof(FluentValidationExtensionsGeneration), value);
                            break;
                        case "parsableGeneration":
                            parsableGeneration = (ParsableGeneration)
                                Enum.Parse(typeof(ParsableGeneration), value);
                            break;
                        case "emptyValueName":
                            emptyValueName = value;
                            break;
                    }
                }
                else
                {
                    // Positional argument
                    var value = arg.GetLastToken().ValueText;
                    switch (pos)
                    {
                        case 0:
                            comparison = (ComparisonGeneration)
                                Enum.Parse(typeof(ComparisonGeneration), value);
                            break;
                        case 1:
                            toUnderlyingTypeCasting = (CastOperator)Enum.Parse(typeof(CastOperator), value);
                            break;
                        case 2:
                            fromUnderlyingTypeCasting = (CastOperator)
                                Enum.Parse(typeof(CastOperator), value);
                            break;
                        case 3:
                            stringCaseSensitivity = (StringCaseSensitivity)
                                Enum.Parse(typeof(StringCaseSensitivity), value);
                            break;
                        case 4:
                            underlyingTypeEqualityGeneration = (UnderlyingTypeEqualityGeneration)
                                Enum.Parse(typeof(UnderlyingTypeEqualityGeneration), value);
                            break;
                        case 5:
                            fluentValidationExtensionsGeneration = (FluentValidationExtensionsGeneration)
                                Enum.Parse(typeof(FluentValidationExtensionsGeneration), value);
                            break;
                        case 6:
                            parsableGeneration = (ParsableGeneration)
                                Enum.Parse(typeof(ParsableGeneration), value);
                            break;
                        case 7:
                            emptyValueName = value;
                            break;
                    }
                }

                pos++;
            }
        }

        return new AttributeConfiguration(
            underlyingType,
            comparison,
            toUnderlyingTypeCasting,
            fromUnderlyingTypeCasting,
            stringCaseSensitivity,
            underlyingTypeEqualityGeneration,
            fluentValidationExtensionsGeneration,
            parsableGeneration,
            emptyValueName
        );
    }
}