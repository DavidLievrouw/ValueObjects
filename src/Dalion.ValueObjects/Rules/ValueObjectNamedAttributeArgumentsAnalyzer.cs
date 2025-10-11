using System.Collections.Immutable;
using System.Linq;
using Dalion.ValueObjects.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dalion.ValueObjects.Rules;

/// <summary>
///     Analyzer that ensures all arguments in the value object attribute are named arguments.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValueObjectNamedAttributeArgumentsAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        RuleIdentifiers.UseNamedAttributeArguments,
        "Value object attribute arguments must be named",
        "All arguments specified in the value object attribute must be named arguments. positional arguments are not allowed.",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        true,
        "The value object attribute is configured using positional arguments. Use named arguments instead."
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

        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration);
        if (symbol is not { } namedTypeSymbol)
        {
            return;
        }

        if (!namedTypeSymbol.IsValueObjectTarget())
        {
            return;
        }

        var attributeData = namedTypeSymbol.TryGetValueObjectAttributes().FirstOrDefault();

        if (
            attributeData?.ApplicationSyntaxReference?.GetSyntax()
            is not AttributeSyntax attributeSyntax
        )
        {
            return;
        }

        foreach (var arg in attributeSyntax.ArgumentList?.Arguments ?? default)
        {
            if (arg.NameColon == null)
            {
                var diagnostic = Diagnostic.Create(Rule, arg.GetLocation(), symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
