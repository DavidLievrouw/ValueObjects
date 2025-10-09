using System;
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
        var className = target.SymbolInformation.Name;
        var namespaceName = target.SymbolInformation.ContainingNamespace.ToDisplayString();

        var attrs = TryGetValueObjectAttributes(target.SymbolInformation);
        var valueType =
            attrs
                .FirstOrDefault(a => a.AttributeClass != null)
                ?.AttributeClass?.GetTypeArguments()
                ?.FirstOrDefault() ?? "object";

        var initCheck =
            Type.GetType(valueType) == typeof(string)
                ? "!string.IsNullOrWhiteSpace(_value)"
                : "_value != default";

        var defaultValue =
            Type.GetType(valueType) == typeof(string) ? "System.String.Empty" : "default";

        /*
         *
        Conversion operators
        ToString override + DebuggerDisplay
        Validation on creation, allowing Empty

         */

        var equality =
            Type.GetType(valueType) == typeof(string)
                ? $@"
                /// <inheritdoc />
                public bool Equals({className}? other)
                {{
                    if (other == null) return false;

                    if (!other.Value.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.Value.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return {valueType}.IsNullOrWhiteSpace(other.Value.Value)
                        ? {valueType}.IsNullOrWhiteSpace(this._value)
                        : {valueType}.Equals(this._value, other.Value.Value, StringComparison.Ordinal);
                }}

                /// <inheritdoc />
                public bool Equals({className} other)
                {{
                    if (!other.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return {valueType}.IsNullOrWhiteSpace(other.Value)
                        ? {valueType}.IsNullOrWhiteSpace(this._value)
                        : {valueType}.Equals(this._value, other.Value, StringComparison.Ordinal);
                }}
            
                /// <inheritdoc />
                public bool Equals({valueType}? other)
                {{
                    return {valueType}.IsNullOrWhiteSpace(other)
                        ? {valueType}.IsNullOrWhiteSpace(this._value)
                        : {valueType}.Equals(this._value, other, StringComparison.Ordinal);
                }}
            
                public bool Equals({className}? other, IEqualityComparer<{className}> comparer)
                {{
                    if (other == null) return false;
                    return comparer.Equals(this, other.Value);
                }}
            
                public bool Equals({valueType}? primitive, StringComparer comparer)
                {{
                    return comparer.Equals(this.Value, primitive);
                }}
            
                /// <inheritdoc />
                public override int GetHashCode() {{
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<{valueType}>.Default.GetHashCode(this._value);
                }}"
                : $@"
                /// <inheritdoc />
                public bool Equals({className}? other)
                {{
                    if (other == null) return false;

                    if (!other.Value.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.Value.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return EqualityComparer<{valueType}>.Default.Equals(this._value, other.Value.Value);
                }}

                /// <inheritdoc />
                public bool Equals({className} other)
                {{
                    if (!other.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return EqualityComparer<{valueType}>.Default.Equals(this._value, other.Value);
                }}
            
                /// <inheritdoc />
                public bool Equals({valueType} other)
                {{
                    return EqualityComparer<{valueType}>.Default.Equals(this._value, other);
                }}
            
                public bool Equals({className}? other, IEqualityComparer<{className}> comparer)
                {{
                    if (other == null) return false;
                    return comparer.Equals(this, other.Value);
                }}
            
                /// <inheritdoc />
                public override int GetHashCode() {{
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<{valueType}>.Default.GetHashCode(this._value);
                }}"
            ;

var creation =
            Type.GetType(valueType) == typeof(string)
                ? $@"
                private {className}({valueType} value) {{ 
                    _value = value ?? {valueType}.Empty;
                }}

                public static {className} From({valueType}? value) {{
                    if (string.IsNullOrWhiteSpace(value)) {{
                        return Empty;
                    }}

                    return new {className}(value);
                }}

                public static bool TryFrom({valueType}? value, out {className} result) {{
                    result = string.IsNullOrWhiteSpace(value) ? Empty : new {className}(value);
                    return result.IsInitialized();
                }}
"
                : $@"
                private {className}({valueType} value) {{ 
                    _value = value;
                }}

                public static {className} From({valueType} value) {{
                    if (value == default) {{
                        return Empty;
                    }}

                    return new {className}(value);
                }}

                public static bool TryFrom({valueType} value, out {className} result) {{
                    result = value == default ? Empty : new {className}(value);
                    return result.IsInitialized();
                }}
";

        var comparison =
            Type.GetType(valueType) == typeof(string)
                ? $@"
                public int CompareTo({className} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({valueType}? other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {className} other1)
                        return this.CompareTo(other1);
                    if (other is {valueType} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {className}"",
                        nameof(other)
                    );
                }}
"
                : $@"
                public int CompareTo({className} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({valueType} other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {className} other1)
                        return this.CompareTo(other1);
                    if (other is {valueType} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {className}"",
                        nameof(other)
                    );
                }}
";

        var conversion =
            $@"
                /// <summary>
                ///     An implicit conversion from <see cref=""{className}"" /> to <see cref=""{valueType}"" />.
                /// </summary>
                /// <param name=""id"">The value to convert.</param>
                /// <returns>The {valueType} representation of the value object.</returns>
                public static implicit operator {valueType}({className} id)
                {{
                    return id.Value;
                }}
            
                /// <summary>
                ///     An explicit conversion from <see cref=""{valueType}"" /> to <see cref=""{className}"" />.
                /// </summary>
                /// <param name=""value"">The value to convert.</param>
                /// <returns>The <see cref=""{className}"" /> instance created from the input value.</returns>
                public static explicit operator {className}({valueType} value)
                {{
                    return {className}.From(value);
                }}";
        
        var generatedClass =
            $@"
        #nullable enable

        namespace {namespaceName} {{
            [System.Diagnostics.DebuggerDisplay(""{className} {{Value}}"")]
            public readonly partial record struct {className} : IEquatable<{className}>,
                                                                IEquatable<{valueType}>,
                                                                IComparable<{className}>,
                                                                IComparable {{
                private readonly {valueType} _value;

                public {valueType} Value => _value;

                {creation}

                public static {className} Empty => new {className}({defaultValue});

                public bool IsInitialized() => {initCheck};

                {equality}

                {comparison}

                {conversion}

                /// <inheritdoc />
                public override string ToString()
                {{
                    return Value.ToString();
                }}
            }}
        }}
        ";

        context.AddSource($"{className}.g.cs", generatedClass);

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
        var namespaceName = context.AnalyzerConfigOptionsProvider.Select(
            (p, _) =>
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

                if (
                    exitingType.GetTypeByMetadataName(ns + "." + nameof(ValueObjectAttribute))
                    is not null
                )
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
        INamedTypeSymbol symbolInformation
    )
    {
        var attrs = symbolInformation.GetAttributes();

        return attrs.Where(a =>
        {
            var ns = a.AttributeClass?.ContainingNamespace?.ToDisplayString();
            var fullName =
                string.IsNullOrEmpty(ns) || ns == "<global namespace>"
                    ? nameof(ValueObjectAttribute)
                    : ns + "." + nameof(ValueObjectAttribute);
            if (fullName.EndsWith("Attribute"))
            {
                fullName = fullName.Substring(0, fullName.Length - "Attribute".Length);
            }

            var typeArg = a.AttributeClass?.GetTypeArguments()?.FirstOrDefault();
            if (typeArg != null)
            {
                fullName += $"<{typeArg}>";
            }

            return a.AttributeClass?.EscapedFullName() == fullName
                || a.AttributeClass?.BaseType?.EscapedFullName() == fullName
                || a.AttributeClass?.BaseType?.BaseType?.EscapedFullName() == fullName;
        });
    }
}
