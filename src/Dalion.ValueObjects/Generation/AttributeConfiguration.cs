using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dalion.ValueObjects.Generation;

internal class AttributeConfiguration
{
    public AttributeConfiguration(
        Type underlyingType,
        ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
        CastOperator toUnderlyingTypeCasting = CastOperator.None,
        CastOperator fromUnderlyingTypeCasting = CastOperator.None,
        StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
        UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration = UnderlyingTypeEqualityGeneration.Omit,
        string emptyValueName = "Empty"
    )
    {
        UnderlyingType = underlyingType;
        Comparison = comparison;
        ToUnderlyingTypeCasting = toUnderlyingTypeCasting;
        FromUnderlyingTypeCasting = fromUnderlyingTypeCasting;
        CaseSensitivity = stringCaseSensitivity;
        UnderlyingTypeEqualityGeneration = underlyingTypeEqualityGeneration;
        EmptyValueName = emptyValueName;
    }

    public Type UnderlyingType { get; }
    public ComparisonGeneration Comparison { get; }
    public CastOperator ToUnderlyingTypeCasting { get; }
    public CastOperator FromUnderlyingTypeCasting { get; }
    public StringCaseSensitivity CaseSensitivity { get; }
    public UnderlyingTypeEqualityGeneration UnderlyingTypeEqualityGeneration { get; }
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

        var comparison = ComparisonGeneration.UseUnderlying;
        var toUnderlyingTypeCasting = CastOperator.None;
        var fromUnderlyingTypeCasting = CastOperator.None;
        var stringCaseSensitivity = StringCaseSensitivity.CaseSensitive;
        var underlyingTypeEqualityGeneration = UnderlyingTypeEqualityGeneration.Omit;
        var emptyValueName = "Empty";
        foreach (var arg in argumentExpressions.Value)
        {
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
                case "emptyValueName":
                    emptyValueName = value;
                    break;
            }
        }

        return new AttributeConfiguration(
            underlyingType,
            comparison,
            toUnderlyingTypeCasting,
            fromUnderlyingTypeCasting,
            stringCaseSensitivity,
            underlyingTypeEqualityGeneration,
            emptyValueName
        );
    }
}
