using System.Collections.Immutable;
using System.Linq;
using Dalion.ValueObjects.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dalion.ValueObjects.Rules;

/// <summary>
///     Analyzer that prohibits the use of two-way implicit conversions on value objects.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValueObjectImplicitConversionAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        RuleIdentifiers.DoNotUseTwoWayImplicitConversion,
        $"Value object cannot have two-way {CastOperator.Implicit} conversions",
        $"Value object attribute cannot have both fromUnderlyingTypeCasting and toUnderlyingTypeCasting set to {CastOperator.Implicit}.",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        true,
        $"The value object is configured to have two-way {CastOperator.Implicit} conversions. Use at least one {CastOperator.Explicit} conversion instead."
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

        if (
            config is
            {
                FromUnderlyingTypeCasting: CastOperator.Implicit,
                ToUnderlyingTypeCasting: CastOperator.Implicit,
            }
        )
        {
            var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(
                Rule,
                symbol.Name,
                symbol.Locations[0]
            );

            context.ReportDiagnostic(diagnostic);
        }
    }
}
