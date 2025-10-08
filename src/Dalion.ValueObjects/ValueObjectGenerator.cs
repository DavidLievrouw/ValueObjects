using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dalion.ValueObjects;

/// <inheritdoc />
[Generator]
public class ValueObjectGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        EnsureValueObjectAttribute(context);

        var valueObjectsProvider = context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
            (ctx, _) =>
            {
                var syntaxInformation = (TypeDeclarationSyntax)ctx.Node;

                var semanticModel = ctx.SemanticModel;

                var declaredSymbol = semanticModel.GetDeclaredSymbol(ctx.Node)!;

                var symbolInformation = (INamedTypeSymbol)declaredSymbol;

                var attributeData = TryGetValueObjectAttributes(symbolInformation)
                    .ToImmutableArray();

                if (attributeData.Length > 0)
                {
                    return new GenerationTarget(
                        semanticModel,
                        syntaxInformation,
                        symbolInformation,
                        attributeData
                    );
                }

                return null;
            }
        );

        context.RegisterSourceOutput(
            valueObjectsProvider,
            (sourceProductionContext, target) =>
            {
                if (target is null)
                {
                    return;
                }

                Execute(target, sourceProductionContext);
            }
        );
    }

    private void Execute(GenerationTarget target, SourceProductionContext context)
    {
        var members = target.SyntaxInformation.Members;

        /*//check if the methods we want to add exist already
        var addMethod = calculatorClassMembers.FirstOrDefault(member =>
            member is MethodDeclarationSyntax method && method.Identifier.Text == "Add"
        );
        var subtractMethod = calculatorClassMembers.FirstOrDefault(member =>
            member is MethodDeclarationSyntax method && method.Identifier.Text == "Subtract"
        );
        var multiplyMethod = calculatorClassMembers.FirstOrDefault(member =>
            member is MethodDeclarationSyntax method && method.Identifier.Text == "Multiply"
        );
        var divideMethod = calculatorClassMembers.FirstOrDefault(member =>
            member is MethodDeclarationSyntax method && method.Identifier.Text == "Divide"
        );

        //this string builder will hold our source code for the methods we want to add
        StringBuilder calcGeneratedClassBuilder = new StringBuilder();
        //This will now correctly parse the Root of the tree for any using statements to add
        foreach (var usingStatement in calculatorClass.SyntaxTree.GetCompilationUnitRoot().Usings)
        {
            calcGeneratedClassBuilder.AppendLine(usingStatement.ToString());
        }
        calcGeneratedClassBuilder.AppendLine();
        //NOTE: This is not the correct way to do this and is used to help produce an error while logging
        SyntaxNode calcClassNamespace = calculatorClass.Parent;
        while (calcClassNamespace is not NamespaceDeclarationSyntax)
        {
            calcClassNamespace = calcClassNamespace.Parent;
        }

        calcGeneratedClassBuilder.AppendLine(
            $"namespace {((NamespaceDeclarationSyntax)calcClassNamespace).Name};"
        );
        calcGeneratedClassBuilder.AppendLine(
            $"public {calculatorClass.Modifiers} class {calculatorClass.Identifier}"
        );
        calcGeneratedClassBuilder.AppendLine("{");

        //if the methods do not exist, we will add them
        if (addMethod is null)
        {
            //when using a raw string, the first " is the far left margin in the file, so if you want the proper indentation on the methods, you will want to tab the string content at least once
            calcGeneratedClassBuilder.AppendLine(
                """
                public int Add(int a, int b)
                {
                var result = a + b;
                Console.WriteLine($"The result of adding {a} and {b} is {result}");
                return result;
                }
                """
            );
        }
        if (subtractMethod is null)
        {
            calcGeneratedClassBuilder.AppendLine(
                """
                public int Subtract(int a, int b)
                {
                var result = a - b;
                if(result < 0)
                {
                Console.WriteLine("Result of subtraction is negative");
                }
                return result;
                }
                """
            );
        }
        if (multiplyMethod is null)
        {
            calcGeneratedClassBuilder.AppendLine(
                """
                public int Multiply(int a, int b)
                {
                return a * b;
                }
                """
            );
        }
        if (divideMethod is null)
        {
            calcGeneratedClassBuilder.AppendLine(
                """
                public int Divide(int a, int b)
                {
                if(b == 0)
                {
                throw new DivideByZeroException();
                }
                return a / b;
                }
                """
            );
        }
        calcGeneratedClassBuilder.AppendLine("}");

        //while a bit crude, it is a simple way to add the methods to the class

        //to write our source file we can use the context object that was passed in
        //this will automatically use the path we provided in the target projects csproj file
        context.AddSource("Calculator.Generated.cs", calcGeneratedClassBuilder.ToString());*/
    }

    private static void EnsureValueObjectAttribute(
        IncrementalGeneratorInitializationContext context
    )
    {
        var namespaceName = context.AnalyzerConfigOptionsProvider.Select((p, _) =>
            p.GlobalOptions.TryGetValue("build_property.RootNamespace", out var ns)
            && !string.IsNullOrWhiteSpace(ns)
                ? ns
                : p.GlobalOptions.TryGetValue("build_property.AssemblyName", out var asm)
                  && !string.IsNullOrWhiteSpace(asm)
                    ? asm
                    : typeof(ValueObjectAttribute).Namespace
        );

        var alreadyExistsDetection = context.CompilationProvider.Select((p, _) => p);

        context.RegisterSourceOutput(
            namespaceName.Combine(alreadyExistsDetection),
            (spc, tuple) =>
            {
                var (ns, exitingType) = tuple;
                
                if (exitingType.GetTypeByMetadataName(ns + "." + nameof(ValueObjectAttribute)) is not null)
                {
                    return;
                }
                
                spc.AddSource(
                    $"{nameof(ValueObjectAttribute)}.g.cs",
                    $@"
#nullable disable

namespace {ns} {{
    public enum ComparisonGeneration {{
        Omit = 0,
        UseUnderlying = 1
    }}

    public enum CastOperator {{
        None = 0,
        Explicit = 1,
        Implicit = 2
    }}

    public enum StringCaseSensitivity {{
        CaseSensitive = 0,
        CaseInsensitive = 1
    }}

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class ValueObjectAttribute<T> : ValueObjectAttribute {{
        public ValueObjectAttribute(
            ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
            CastOperator toPrimitiveCasting = CastOperator.None,
            CastOperator fromPrimitiveCasting = CastOperator.None,
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive
        )
            : base(
                typeof(T),
                comparison,
                toPrimitiveCasting,
                fromPrimitiveCasting,
                stringCaseSensitivity
            ) {{ }}
    }}

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class ValueObjectAttribute : System.Attribute {{
        public ValueObjectAttribute(
            Type underlyingType = null,
            ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
            CastOperator toPrimitiveCasting = CastOperator.None,
            CastOperator fromPrimitiveCasting = CastOperator.None,
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive
        ) {{ }}
    }}
}}
"
                );
            }
        );
    }

    private static IEnumerable<AttributeData> TryGetValueObjectAttributes(
        INamedTypeSymbol voSymbolInformation
    )
    {
        var attrs = voSymbolInformation.GetAttributes();

        return attrs.Where(a =>
            {
                var fullName = a.AttributeClass?.ContainingNamespace + "." + nameof(ValueObjectAttribute);

                return a.AttributeClass?.EscapedFullName() == fullName
                       || a.AttributeClass?.BaseType?.EscapedFullName() == fullName
                       || a.AttributeClass?.BaseType?.BaseType?.EscapedFullName() == fullName;
            }
        );
    }
}