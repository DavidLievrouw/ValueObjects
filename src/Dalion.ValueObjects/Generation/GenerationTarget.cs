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
        var underlyingType = Type.GetType(typeSymbol.ToDisplayString()) ?? typeof(string);

        var comparison = ComparisonGeneration.UseUnderlying;
        var toPrimitiveCasting = CastOperator.None;
        var fromPrimitiveCasting = CastOperator.None;
        var stringCaseSensitivity = StringCaseSensitivity.CaseSensitive;
        var primitiveEqualityGeneration = PrimitiveEqualityGeneration.GenerateOperatorsAndMethods;
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
                    primitiveEqualityGeneration = (PrimitiveEqualityGeneration)Enum.Parse(typeof(PrimitiveEqualityGeneration), value);
                    break;
                case "stringCaseSensitivity":
                    stringCaseSensitivity = (StringCaseSensitivity)
                        Enum.Parse(typeof(StringCaseSensitivity), value);
                    break;
            }
        }

        return new AttributeConfiguration(
            underlyingType,
            comparison,
            toPrimitiveCasting,
            fromPrimitiveCasting,
            stringCaseSensitivity,
            primitiveEqualityGeneration
        );
    }
}
