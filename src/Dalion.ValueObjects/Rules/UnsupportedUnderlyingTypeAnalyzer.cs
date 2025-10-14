using System.Collections.Immutable;
using System.Linq;
using Dalion.ValueObjects.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dalion.ValueObjects.Rules;

/// <summary>
///     Analyzer that ensures that only supported underlying types are used for value objects.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnsupportedUnderlyingTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        RuleIdentifiers.UnsupportedUnderlyingType,
        "The specified underlying type is not supported",
        "Type '{0}' cannot be used as underlying type",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        true,
        "The specified underlying type is not supported for value objects."
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(
            AnalyzeNode,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordStructDeclaration
        );
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeDeclaration)
        {
            return;
        }

        var symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclaration);

        if (symbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return;
        }

        if (!namedTypeSymbol.IsValueObjectTarget())
        {
            return;
        }

        var attributeData = namedTypeSymbol.TryGetValueObjectAttributes().First();
        var config = AttributeConfiguration.FromAttributeData(attributeData, namedTypeSymbol);

        TypeSyntax? typeArgSyntax = null;
        Location? location = null;
        var attributeSyntax = attributeData.ApplicationSyntaxReference?.GetSyntax(
            context.CancellationToken
        );
        if (
            attributeSyntax is AttributeSyntax
            {
                Name: GenericNameSyntax { TypeArgumentList.Arguments.Count: > 0 } genericName,
            }
        )
        {
            typeArgSyntax = genericName.TypeArgumentList.Arguments[0];
            location = typeArgSyntax.GetLocation();
        }

        if (typeArgSyntax is not null)
        {
            var typeSymbol = context
                .SemanticModel.GetTypeInfo(typeArgSyntax, context.CancellationToken)
                .Type;
            if (!IsSupportedUnderlyingType(typeSymbol))
            {
                var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(
                    Rule,
                    typeArgSyntax.GetFirstToken().Text,
                    location ?? symbol.Locations[0]
                );

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static bool IsSupportedUnderlyingType(ITypeSymbol? typeSymbol)
    {
        if (
            typeSymbol?.SpecialType
            is SpecialType.System_Byte
                or SpecialType.System_Char
                or SpecialType.System_Decimal
                or SpecialType.System_Double
                or SpecialType.System_Int16
                or SpecialType.System_Int32
                or SpecialType.System_Int64
                or SpecialType.System_SByte
                or SpecialType.System_Single
                or SpecialType.System_String
                or SpecialType.System_UInt16
                or SpecialType.System_UInt32
                or SpecialType.System_UInt64
                or SpecialType.System_DateTime
        )
        {
            return true;
        }

        if (
            typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.ContainingNamespace?.ToDisplayString() == "System"
            && namedTypeSymbol.Name is "Guid" or "TimeSpan" or "TimeOnly" or "DateTimeOffset" or "DateOnly"
        )
        {
            return true;
        }

        return false;
    }
}
