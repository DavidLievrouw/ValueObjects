using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dalion.ValueObjects.Rules;

/// <summary>
///     Analyzer that prohibits the use of 'new' to create value objects.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseOnReadonlyRecordStructAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule1 = new(
        RuleIdentifiers.UseOnReadonlyRecordStruct,
        "Only readonly record structs can be used as Value Objects",
        "Only readonly record structs can be used as Value Objects. Type '{0}' is not a readonly record struct.",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        true,
        "The value object is not a readonly record struct. Change the declaration to be a readonly record struct."
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule1);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration);
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

        if (namedTypeSymbol is { IsRecord: true, TypeKind: TypeKind.Struct, IsReadOnly: true })
        {
            return;
        }

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(
            Rule1,
            symbol.Name,
            symbol.Locations[0]
        );

        context.ReportDiagnostic(diagnostic);
    }
}