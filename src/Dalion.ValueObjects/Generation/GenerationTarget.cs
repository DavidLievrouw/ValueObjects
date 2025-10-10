using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dalion.ValueObjects.Generation;

internal class GenerationTarget
{
    public GenerationTarget(
        SemanticModel semanticModel,
        TypeDeclarationSyntax typeToAugment,
        INamedTypeSymbol symbolForType,
        AttributeData attributeData
    )
    {
        SemanticModel = semanticModel;
        SyntaxInformation = typeToAugment;
        SymbolInformation = symbolForType;
        AttributeData = attributeData;
    }

    public SemanticModel SemanticModel { get; }

    public TypeDeclarationSyntax SyntaxInformation { get; set; }

    public INamedTypeSymbol SymbolInformation { get; set; }

    public AttributeData AttributeData { get; }

    public AttributeConfiguration GetAttributeConfiguration()
    {
        var syntaxRef = AttributeData.ApplicationSyntaxReference!;
        var attributeSyntax = (AttributeSyntax)syntaxRef.GetSyntax();
        var argumentExpressions = attributeSyntax.ArgumentList?.Arguments!;

        var typeSymbol = AttributeData.AttributeClass!.TypeArguments[0];
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
        var toPrimitiveCasting = CastOperator.None;
        var fromPrimitiveCasting = CastOperator.None;
        var stringCaseSensitivity = StringCaseSensitivity.CaseSensitive;
        var primitiveEqualityGeneration = PrimitiveEqualityGeneration.GenerateOperatorsAndMethods;
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
                case "toPrimitiveCasting":
                    toPrimitiveCasting = (CastOperator)Enum.Parse(typeof(CastOperator), value);
                    break;
                case "fromPrimitiveCasting":
                    fromPrimitiveCasting = (CastOperator)Enum.Parse(typeof(CastOperator), value);
                    break;
                case "primitiveEqualityGeneration":
                    primitiveEqualityGeneration = (PrimitiveEqualityGeneration)
                        Enum.Parse(typeof(PrimitiveEqualityGeneration), value);
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
            toPrimitiveCasting,
            fromPrimitiveCasting,
            stringCaseSensitivity,
            primitiveEqualityGeneration,
            emptyValueName
        );
    }
}
