using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                        attributeData.First()
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

    /*
     * JsonConverter
     * TypeConverter
     */
    private void Execute(GenerationTarget target, SourceProductionContext context)
    {
        var className = target.SymbolInformation.Name;
        var namespaceName = target.SymbolInformation.ContainingNamespace.ToDisplayString();

        var config = target.GetAttributeConfiguration();
        var valueType = config.UnderlyingType;
        var valueTypeName = valueType.FullName;

        var initCheck =
            valueType == typeof(string)
                ? "!System.String.IsNullOrWhiteSpace(_value)"
                : "_value != default";

        var defaultValue = valueType == typeof(string) ? "System.String.Empty" : "default";

        var validateMethod = target
            .SyntaxInformation.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(member =>
                member.Identifier.Text == "Validate"
                && member.Modifiers.Any(SyntaxKind.PrivateKeyword)
                && member.Modifiers.Any(SyntaxKind.StaticKeyword)
            );

        var ctorValidation =
            validateMethod == null
                ? ""
                : @"
                  var validationResult = Validate(value);
                  if (!validationResult.IsSuccess) {
                      throw new System.InvalidOperationException(validationResult.ErrorMessage);
                  }";

        var tryFromValidation =
            validateMethod == null
                ? "return result.IsInitialized();"
                : "return result.IsInitialized() && Validate(result._value).IsSuccess;";

        var stringComparison =
            config.CaseSensitivity == StringCaseSensitivity.CaseInsensitive
                ? "OrdinalIgnoreCase"
                : "Ordinal";

        var equality =
            valueType == typeof(string)
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
            
                    return {valueTypeName}.IsNullOrWhiteSpace(other.Value.Value)
                        ? {valueTypeName}.IsNullOrWhiteSpace(this._value)
                        : {valueTypeName}.Equals(this._value, other.Value.Value, System.StringComparison.{stringComparison});
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
            
                    return {valueTypeName}.IsNullOrWhiteSpace(other.Value)
                        ? {valueTypeName}.IsNullOrWhiteSpace(this._value)
                        : {valueTypeName}.Equals(this._value, other.Value, System.StringComparison.{stringComparison});
                }}
            
                public bool Equals({className}? other, IEqualityComparer<{className}> comparer)
                {{
                    if (other == null) return false;
                    return comparer.Equals(this, other.Value);
                }}
            
                /// <inheritdoc />
                public override int GetHashCode() {{
                    if (!IsInitialized()) return 0;
                    return StringComparer.{stringComparison}.GetHashCode(this._value);
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
            
                    return EqualityComparer<{valueTypeName}>.Default.Equals(this._value, other.Value.Value);
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
            
                    return EqualityComparer<{valueTypeName}>.Default.Equals(this._value, other.Value);
                }}
            
                public bool Equals({className}? other, IEqualityComparer<{className}> comparer)
                {{
                    if (other == null) return false;
                    return comparer.Equals(this, other.Value);
                }}
            
                /// <inheritdoc />
                public override int GetHashCode() {{
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<{valueTypeName}>.Default.GetHashCode(this._value);
                }}";
        
        var equalityUnderlyingType =
            valueType == typeof(string)
                ? $@"
                /// <inheritdoc />
                public bool Equals({valueTypeName}? other)
                {{
                    return {valueTypeName}.IsNullOrWhiteSpace(other)
                        ? {valueTypeName}.IsNullOrWhiteSpace(this._value)
                        : {valueTypeName}.Equals(this._value, other, System.StringComparison.{stringComparison});
                }}
            
                public bool Equals({valueTypeName}? primitive, StringComparer comparer)
                {{
                    return comparer.Equals(this.Value, primitive);
                }}"
                : $@"
                /// <inheritdoc />
                public bool Equals({valueTypeName} other)
                {{
                    return EqualityComparer<{valueTypeName}>.Default.Equals(this._value, other);
                }}";

        var equalityOperators =
            (config.PrimitiveEqualityGeneration & PrimitiveEqualityGeneration.GenerateOperators)
            == PrimitiveEqualityGeneration.GenerateOperators
                ? valueType == typeof(string)
                    ? $@"
    public static bool operator ==({className} left, {valueTypeName}? right) => left.Value.Equals(right);

    public static bool operator ==({valueTypeName}? left, {className} right) => right.Value.Equals(left);

    public static bool operator !=({valueTypeName}? left, {className} right) => !(left == right);

    public static bool operator !=({className} left, {valueTypeName}? right) => !(left == right);
"
                    : $@"
    public static bool operator ==({className} left, {valueTypeName} right) => left.Value.Equals(right);

    public static bool operator ==({valueTypeName} left, {className} right) => right.Value.Equals(left);

    public static bool operator !=({valueTypeName} left, {className} right) => !(left == right);

    public static bool operator !=({className} left, {valueTypeName} right) => !(left == right);
"
                : "";

        var creation =
            valueType == typeof(string)
                ? $@"
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public {className}()
                {{
                    _value = {valueTypeName}.Empty;
                }}

                [System.Diagnostics.DebuggerStepThrough]
                private {className}({valueTypeName} value, bool validation = true) {{
                    if (validation) {{
                        {ctorValidation}
                    }}
                    _value = value ?? {valueTypeName}.Empty;
                }}

                public static {className} From({valueTypeName}? value) {{
                    if (string.IsNullOrWhiteSpace(value)) {{
                        return Empty;
                    }}

                    return new {className}(value);
                }}

                public static bool TryFrom({valueTypeName}? value, out {className} result) {{
                    result = string.IsNullOrWhiteSpace(value) ? Empty : new {className}(value, validation: false);
                    {tryFromValidation}
                }}
"
                : $@"
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public {className}()
                {{
                    _value = default;
                }}

                private {className}({valueTypeName} value, bool validation = true) {{
                    if (validation) {{
                        {ctorValidation}
                    }}
                    _value = value;
                }}

                public static {className} From({valueTypeName} value) {{
                    if (value == default) {{
                        return Empty;
                    }}

                    return new {className}(value);
                }}

                public static bool TryFrom({valueTypeName} value, out {className} result) {{
                    result = value == default ? Empty : new {className}(value, validation: false);
                    {tryFromValidation}
                }}
";

        var comparison =
            config.Comparison == ComparisonGeneration.Omit ? ""
            : valueType == typeof(string)
                ? $@"
                public int CompareTo({className} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({valueTypeName}? other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {className} other1)
                        return this.CompareTo(other1);
                    if (other is {valueTypeName} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {className}"",
                        nameof(other)
                    );
                }}
"
            : $@"
                public int CompareTo({className} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({valueTypeName} other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {className} other1)
                        return this.CompareTo(other1);
                    if (other is {valueTypeName} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {className}"",
                        nameof(other)
                    );
                }}
";

        var conversionToPrimitiveModifier =
            config.ToPrimitiveCasting == CastOperator.Implicit ? "implicit" : "explicit";
        var conversionToPrimitive =
            config.ToPrimitiveCasting == CastOperator.None
                ? ""
                : $@"
                /// <summary>
                ///     An implicit conversion from <see cref=""{className}"" /> to <see cref=""{valueTypeName}"" />.
                /// </summary>
                /// <param name=""id"">The value to convert.</param>
                /// <returns>The {valueTypeName} representation of the value object.</returns>
                public static {conversionToPrimitiveModifier} operator {valueTypeName}({className} id)
                {{
                    return id.Value;
                }}";

        var conversionFromPrimitiveModifier =
            config.FromPrimitiveCasting == CastOperator.Implicit ? "implicit" : "explicit";
        var conversionFromPrimitive =
            config.FromPrimitiveCasting == CastOperator.None
                ? ""
                : $@"
                /// <summary>
                ///     An explicit conversion from <see cref=""{valueTypeName}"" /> to <see cref=""{className}"" />.
                /// </summary>
                /// <param name=""value"">The value to convert.</param>
                /// <returns>The <see cref=""{className}"" /> instance created from the input value.</returns>
                public static {conversionFromPrimitiveModifier} operator {className}({valueTypeName} value)
                {{
                    return {className}.From(value);
                }}";

        var validationClasses =
            @"
private class Validation
{
    public static readonly Validation Ok = new(string.Empty);

    private Validation(string reason)
    {
        ErrorMessage = reason;
    }

    public string ErrorMessage { get; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public Dictionary<object, object>? Data { get; private set; }

    public static Validation Invalid(string reason = """")
    {
        if (string.IsNullOrEmpty(reason))
        {
            return new Validation(""[none provided]"");
        }

        return new Validation(reason);
    }

    public Validation WithData(object key, object value)
    {
        Data ??= new Dictionary<object, object>();
        Data[key] = value;
        return this;
    }
}
private class ValueObjectValidationException : Exception
{
    private const string DefaultMessage = ""Validation of the value object failed."";

    public ValueObjectValidationException()
        : base(DefaultMessage) { }

    public ValueObjectValidationException(string message)
        : base(message) { }

    public ValueObjectValidationException(Exception innerException)
        : base(DefaultMessage, innerException) { }

    public ValueObjectValidationException(string message, Exception innerException)
        : base(message, innerException) { }
}";

        var toStringOverrides =
            valueType == typeof(string)
                ? @"
                /// <inheritdoc />
                public override string ToString()
                {{
                    return Value.ToString() ?? """";
                }}

                /// <inheritdoc cref=""M:System.String.ToString(System.IFormatProvider)"" />
                public string ToString(IFormatProvider? provider)
                {{
                    return Value.ToString(provider: provider) ?? """";
                }}
"
                : @"
                /// <inheritdoc />
                public override string ToString()
                {
                    return Value.ToString() ?? """";
                }

                /// <inheritdoc cref=""M:System.String.ToString(System.IFormatProvider)"" />
                public string ToString(IFormatProvider? provider)
                {
                    return Value.ToString(format: null, provider: provider) ?? """";
                }
";

        var interfaceDefsBuilder = new StringBuilder();
        interfaceDefsBuilder.AppendLine($": IEquatable<{className}>");
        if (
            (config.PrimitiveEqualityGeneration & PrimitiveEqualityGeneration.GenerateMethods)
            == PrimitiveEqualityGeneration.GenerateMethods
        )
        {
            interfaceDefsBuilder.Append($", IEquatable<{valueTypeName}>");
        }

        if (config.Comparison != ComparisonGeneration.Omit)
        {
            interfaceDefsBuilder.Append($", IComparable<{className}>");
            interfaceDefsBuilder.Append(", IComparable");
        }

        var interfaceDefs = interfaceDefsBuilder.ToString();

        var generatedClass =
            $@"
        #nullable enable

        namespace {namespaceName} {{
            [System.Diagnostics.DebuggerDisplay(""{className} {{Value}}"")]
            public readonly partial record struct {className} {interfaceDefs} {{
                private readonly {valueTypeName} _value;

                public {valueTypeName} Value => _value;

                {creation}

                public static {className} Empty => new {className}({defaultValue}, validation: false);

                public bool IsInitialized() => {initCheck};

                {equality}

                {equalityUnderlyingType}

                {equalityOperators}

                {comparison}

                {conversionToPrimitive}

                {conversionFromPrimitive}

                {toStringOverrides}

                {validationClasses}
            }}
        }}
        ";

        context.AddSource($"{className}.g.cs", generatedClass);
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
    [System.Flags]
    public enum PrimitiveEqualityGeneration {{
        Omit = 0,
        GenerateOperators = 1 << 0,
        GenerateMethods = 1 << 1,
        GenerateOperatorsAndMethods = GenerateOperators | GenerateMethods
    }}

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
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
            PrimitiveEqualityGeneration primitiveEqualityGeneration = PrimitiveEqualityGeneration.GenerateOperatorsAndMethods
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
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
            PrimitiveEqualityGeneration primitiveEqualityGeneration = PrimitiveEqualityGeneration.GenerateOperatorsAndMethods
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
