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
        return AttributeConfiguration.FromAttributeData(AttributeData, SymbolInformation);
    }
}
