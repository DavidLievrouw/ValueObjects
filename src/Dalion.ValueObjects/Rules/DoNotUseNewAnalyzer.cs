using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dalion.ValueObjects.Rules;

/// <summary>
///     Analyzer that prohibits the use of 'new' to create value objects.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseNewAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        RuleIdentifiers.DoNotUseNew,
        "Using new to create Value Objects is prohibited - use the From method for creation",
        "Type '{0}' cannot be constructed with 'new' as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        true,
        "The value object is created with a new expression. This can lead to invalid value objects in your domain. Use the From method instead."
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterOperationAction(
                AnalyzeExpression,
                OperationKind.ObjectCreation
            );
        });
    }

    private static void AnalyzeExpression(OperationAnalysisContext context)
    {
        if (context.Operation is not IObjectCreationOperation c)
        {
            return;
        }

        if (c.Type is not INamedTypeSymbol symbol)
        {
            return;
        }

        if (!symbol.IsValueObjectTarget())
        {
            return;
        }

        var isAPublicStaticFieldInAValueObject = IsAPublicStaticFieldInAValueObject(context);
        if (isAPublicStaticFieldInAValueObject)
        {
            if (IsTypeOfValueObject(context, symbol))
            {
                return;
            }
        }

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(
            Rule,
            symbol.Name,
            context.Operation.Syntax.GetLocation()
        );

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsAPublicStaticFieldInAValueObject(OperationAnalysisContext context)
    {
        var cs = context.ContainingSymbol as IFieldSymbol;

        return cs is { DeclaredAccessibility: Accessibility.Public, IsStatic: true };
    }

    private static bool IsTypeOfValueObject(OperationAnalysisContext context, INamedTypeSymbol target)
    {
        if (context.ContainingSymbol is not IFieldSymbol cs)
        {
            return false;
        }
        
        var type = cs.ContainingType;
        return SymbolEqualityComparer.Default.Equals(type, target);
    }
}
