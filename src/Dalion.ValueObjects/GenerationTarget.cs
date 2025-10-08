using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dalion.ValueObjects;

internal class GenerationTarget
{
    public GenerationTarget(
        SemanticModel semanticModel,
        TypeDeclarationSyntax typeToAugment,
        INamedTypeSymbol symbolForType,
        ImmutableArray<AttributeData> dataForAttributes
    )
    {
        SemanticModel = semanticModel;
        SyntaxInformation = typeToAugment;
        SymbolInformation = symbolForType;
        DataForAttributes = dataForAttributes;
    }

    public SemanticModel SemanticModel { get; }

    public TypeDeclarationSyntax SyntaxInformation { get; set; }

    public INamedTypeSymbol SymbolInformation { get; set; }

    public ImmutableArray<AttributeData> DataForAttributes { get; }
}
