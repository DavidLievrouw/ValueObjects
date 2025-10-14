using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Dalion.ValueObjects.Generation.Fragments;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Dalion.ValueObjects.Generation;

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

                var attributeData = symbolInformation
                    .TryGetValueObjectAttributes()
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

                AddGeneratedValueObject(target, sourceProductionContext);
                AddGeneratedFluentValidationExtensions(target, sourceProductionContext);
                AddGeneratedUnderlyingTypeCreationExtensions(target, sourceProductionContext);
            }
        );
    }

    private void AddGeneratedFluentValidationExtensions(
        GenerationTarget target,
        SourceProductionContext context
    )
    {
        var typeName = target.SymbolInformation.Name;
        var namespaceName = target.SymbolInformation.ContainingNamespace.ToDisplayString();

        var config = target.GetAttributeConfiguration();

        if (
            config.FluentValidationExtensionsGeneration == FluentValidationExtensionsGeneration.Omit
        )
        {
            return;
        }

        var containingTypeNames = GetContainingTypeNames(target.SymbolInformation);
        var containingTypes =
            containingTypeNames == string.Empty ? string.Empty : containingTypeNames + ".";

        var mustBeInitialized =
        (
            config.FluentValidationExtensionsGeneration
            & FluentValidationExtensionsGeneration.GenerateMustBeInitialized
        ) == FluentValidationExtensionsGeneration.GenerateMustBeInitialized
            ? $@"
                public static FluentValidation.IRuleBuilderOptions<T, {containingTypes}{typeName}> MustBeInitialized<T>(
                    this FluentValidation.IRuleBuilderInitial<T, {containingTypes}{typeName}> ruleBuilder
                )
                {{
                    return ruleBuilder
                        .Cascade(FluentValidation.CascadeMode.Stop)
                        .Must(o => o.IsInitialized())
                        .WithMessage($""{{nameof({containingTypes}{typeName})}} must be initialized."");
                }}"
            : string.Empty;

        var mustBeInitializedAndValid =
        (
            config.FluentValidationExtensionsGeneration
            & FluentValidationExtensionsGeneration.GenerateMustBeInitializedAndValid
        ) == FluentValidationExtensionsGeneration.GenerateMustBeInitializedAndValid
            ? $@"
                public static FluentValidation.IRuleBuilderOptions<T, {containingTypes}{typeName}> MustBeInitializedAndValid<T>(
                    this FluentValidation.IRuleBuilderInitial<T, {containingTypes}{typeName}> ruleBuilder
                )
                {{
                    return ruleBuilder
                        .Cascade(FluentValidation.CascadeMode.Stop)
                        .Must(o => o.IsInitialized())
                        .WithMessage($""{{nameof({containingTypes}{typeName})}} must be initialized."")
                        .Must(o => o.IsValid())
                        .WithMessage((_, p) => p.GetValidationErrorMessage());
                }}"
            : string.Empty;

        var code =
            $@"
        #nullable enable

        using FluentValidation;

        namespace {namespaceName} {{
            public static class {typeName}FluentValidationExtensions
            {{
                {mustBeInitialized}
            
                {mustBeInitializedAndValid}
            }}
        }}
        ";

        context.AddSource($"{typeName}FluentValidationExtensions.g.cs", code);
    }

    private void AddGeneratedUnderlyingTypeCreationExtensions(
        GenerationTarget target,
        SourceProductionContext context
    )
    {
        var typeName = target.SymbolInformation.Name;
        var namespaceName = target.SymbolInformation.ContainingNamespace.ToDisplayString();

        var config = target.GetAttributeConfiguration();

        if (
            config.UnderlyingTypeCreationMethodGeneration == UnderlyingTypeCreationMethodGeneration.Omit
        )
        {
            return;
        }

        var containingTypeNames = GetContainingTypeNames(target.SymbolInformation);
        var containingTypes =
            containingTypeNames == string.Empty ? string.Empty : containingTypeNames + ".";

        var creationMethod =
            config.UnderlyingType.SpecialType == SpecialType.System_String
                ? $@"
                 public static {namespaceName}.{containingTypes}{typeName} {typeName}(this {config.UnderlyingTypeName}? value)
                 {{
                     return {namespaceName}.{containingTypes}{typeName}.From(value);
                 }}"
                : $@"
                 public static {namespaceName}.{containingTypes}{typeName} {typeName}(this {config.UnderlyingTypeName} value)
                 {{
                     return {namespaceName}.{containingTypes}{typeName}.From(value);
                 }}";

        var code =
            $@"
        #nullable enable

        namespace {namespaceName} {{
            public static class {typeName}UnderlyingTypeCreationExtensions
            {{
                {creationMethod}
            }}
        }}
        ";

        context.AddSource($"{typeName}UnderlyingTypeCreationExtensions.g.cs", code);
    }

    private void AddGeneratedValueObject(GenerationTarget target, SourceProductionContext context)
    {
        var typeName = target.SymbolInformation.Name;
        var namespaceName = target.SymbolInformation.ContainingNamespace.ToDisplayString();

        var config = target.GetAttributeConfiguration();
        var valueType = config.UnderlyingType;
        var emptyValueName = config.EmptyValueName;

        var defaultValue =
            valueType.SpecialType == SpecialType.System_String ? "System.String.Empty" : "default";

        var validateMethod = target
            .SyntaxInformation.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(member =>
                member.Identifier.Text == "Validate"
                && member.Modifiers.Any(SyntaxKind.PrivateKeyword)
                && member.Modifiers.Any(SyntaxKind.StaticKeyword)
                && member.ParameterList.Parameters.Count == 1
                && SymbolEqualityComparer.Default.Equals(
                    target
                        .SemanticModel.GetTypeInfo(member.ParameterList.Parameters[0].Type!)
                        .Type!,
                    target.SemanticModel.Compilation.GetTypeByMetadataName(config.UnderlyingTypeName)
                )
            );

        var validationFieldAssignment =
            validateMethod == null
                ? "_validation ??= Validation.Ok;"
                : "_validation ??= Validate(_value);";

        var tryFromValidation =
            validateMethod == null
                ? "return result.IsInitialized();"
                : $"return result.IsInitialized() && (Validate(result._value).IsSuccess || {typeName}PreSetValueCache.{typeName}PreSetValues.TryGetValue(value, out _));";

        var normalizeMethod = target
            .SyntaxInformation.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(member =>
                member.Identifier.Text == "NormalizeInput"
                && member.Modifiers.Any(SyntaxKind.PrivateKeyword)
                && member.Modifiers.Any(SyntaxKind.StaticKeyword)
                && member.ParameterList.Parameters.Count == 1
                && SymbolEqualityComparer.Default.Equals(
                    target
                        .SemanticModel.GetTypeInfo(member.ParameterList.Parameters[0].Type!)
                        .Type!,
                    target.SemanticModel.Compilation.GetTypeByMetadataName(config.UnderlyingTypeName)
                )
                && SymbolEqualityComparer.Default.Equals(
                    target.SemanticModel.GetTypeInfo(member.ReturnType).Type!,
                    target.SemanticModel.Compilation.GetTypeByMetadataName(config.UnderlyingTypeName)
                )
            );
        var inputNormalization = normalizeMethod == null ? "" : "value = NormalizeInput(value);";

        var stringComparison =
            config.CaseSensitivity == StringCaseSensitivity.CaseInsensitive
                ? "OrdinalIgnoreCase"
                : "Ordinal";

        var equality =
            valueType.SpecialType == SpecialType.System_String
                ? $@"
                /// <inheritdoc />
                public bool Equals({typeName}? other)
                {{
                    if (other is null) return false;

                    if (!other.Value.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.Value.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return other.Value._isNullOrEmpty
                        ? this._isNullOrEmpty
                        : {config.UnderlyingTypeName}.Equals(this._value, other.Value.Value, System.StringComparison.{stringComparison});
                }}

                /// <inheritdoc />
                public bool Equals({typeName} other)
                {{
                    if (!other.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return other._isNullOrEmpty
                        ? this._isNullOrEmpty
                        : {config.UnderlyingTypeName}.Equals(this._value, other.Value, System.StringComparison.{stringComparison});
                }}
            
                public bool Equals({typeName}? other, IEqualityComparer<{typeName}> comparer)
                {{
                    if (other is null) return false;
                    return comparer.Equals(this, other.Value);
                }}
            
                /// <inheritdoc />
                public override int GetHashCode() {{
                    if (!IsInitialized()) return 0;
                    return StringComparer.{stringComparison}.GetHashCode(this._value);
                }}"
                : $@"
                /// <inheritdoc />
                public bool Equals({typeName}? other)
                {{
                    if (other is null) return false;

                    if (!other.Value.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.Value.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return EqualityComparer<{config.UnderlyingTypeName}>.Default.Equals(this._value, other.Value.Value);
                }}

                /// <inheritdoc />
                public bool Equals({typeName} other)
                {{
                    if (!other.IsInitialized())
                    {{
                        return !IsInitialized();
                    }}

                    if (other.IsInitialized() != IsInitialized())
                    {{
                        return false;
                    }}
            
                    return EqualityComparer<{config.UnderlyingTypeName}>.Default.Equals(this._value, other.Value);
                }}
            
                public bool Equals({typeName}? other, IEqualityComparer<{typeName}> comparer)
                {{
                    if (other is null) return false;
                    return comparer.Equals(this, other.Value);
                }}
            
                /// <inheritdoc />
                public override int GetHashCode() {{
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<{config.UnderlyingTypeName}>.Default.GetHashCode(this._value);
                }}";

        var equalityUnderlyingType =
        (
            config.UnderlyingTypeEqualityGeneration
            & UnderlyingTypeEqualityGeneration.GenerateMethods
        ) == UnderlyingTypeEqualityGeneration.GenerateMethods
            ? valueType.SpecialType == SpecialType.System_String
                ? $@"
                /// <inheritdoc />
                public bool Equals({config.UnderlyingTypeName}? other)
                {{
                    return {config.UnderlyingTypeName}.IsNullOrEmpty(other)
                        ? this._isNullOrEmpty
                        : {config.UnderlyingTypeName}.Equals(this._value, other, System.StringComparison.{stringComparison});
                }}
            
                public bool Equals({config.UnderlyingTypeName}? underlyingValue, StringComparer comparer)
                {{
                    return comparer.Equals(this.Value, underlyingValue);
                }}"
                : $@"
                /// <inheritdoc />
                public bool Equals({config.UnderlyingTypeName} other)
                {{
                    return EqualityComparer<{config.UnderlyingTypeName}>.Default.Equals(this._value, other);
                }}"
            : "";

        var equalityOperators =
        (
            config.UnderlyingTypeEqualityGeneration
            & UnderlyingTypeEqualityGeneration.GenerateOperators
        ) == UnderlyingTypeEqualityGeneration.GenerateOperators
            ? valueType.SpecialType == SpecialType.System_String
                ? $@"
    public static bool operator ==({typeName} left, {config.UnderlyingTypeName}? right) => left.Value.Equals(right);

    public static bool operator ==({config.UnderlyingTypeName}? left, {typeName} right) => right.Value.Equals(left);

    public static bool operator !=({config.UnderlyingTypeName}? left, {typeName} right) => !(left == right);

    public static bool operator !=({typeName} left, {config.UnderlyingTypeName}? right) => !(left == right);
"
                : $@"
    public static bool operator ==({typeName} left, {config.UnderlyingTypeName} right) => left.Value.Equals(right);

    public static bool operator ==({config.UnderlyingTypeName} left, {typeName} right) => right.Value.Equals(left);

    public static bool operator !=({config.UnderlyingTypeName} left, {typeName} right) => !(left == right);

    public static bool operator !=({typeName} left, {config.UnderlyingTypeName} right) => !(left == right);
"
            : "";

        var creation =
            valueType.SpecialType == SpecialType.System_String
                ? $@"
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public {typeName}()
                {{
                    _value = {config.UnderlyingTypeName}.Empty;
                    _initialized = false;
                    _isNullOrEmpty = {config.UnderlyingTypeName}.IsNullOrEmpty(_value);
                    {validationFieldAssignment}
                }}

                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                private {typeName}({config.UnderlyingTypeName}? value) {{
                    {inputNormalization}
                    if (value == default) {{
                        _initialized = false;
                        _value = {config.UnderlyingTypeName}.Empty;
                    }} else {{
                        _initialized = true;
                        _value = value;
                    }}
                    _isNullOrEmpty = {config.UnderlyingTypeName}.IsNullOrEmpty(_value);
                    {validationFieldAssignment}
                }}

                public static {typeName} From({config.UnderlyingTypeName}? value) {{
                    if (value is null) {{
                        throw new System.InvalidOperationException(""Cannot create an instance of {typeName} from null."");
                    }}

                    {inputNormalization}

                    var vo = new {typeName}(value);

                    if (!vo.IsValid() && value is not null && !{typeName}PreSetValueCache.{typeName}PreSetValues.TryGetValue(value, out _)) {{
                        throw new System.InvalidOperationException(vo.GetValidationErrorMessage());
                    }}

                    return vo;
                }}

                public static bool TryFrom({config.UnderlyingTypeName}? value, out {typeName} result) {{
                    if (value is null) {{
                        result = new {typeName}();
                        return false;
                    }}

                    result = string.IsNullOrEmpty(value) ? {emptyValueName} : new {typeName}(value);
                    {tryFromValidation}
                }}
"
                : $@"
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public {typeName}()
                {{
                    _value = default;
                    _initialized = false;
                    _isNullOrEmpty = false;
                    {validationFieldAssignment}
                }}

                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                private {typeName}({config.UnderlyingTypeName} value) {{
                    {inputNormalization}
                    _initialized = true;
                    _value = value;
                    _isNullOrEmpty = false;
                    {validationFieldAssignment}
                }}

                public static {typeName} From({config.UnderlyingTypeName} value) {{
                    if (value == default) {{
                        return {emptyValueName};
                    }}

                    {inputNormalization}

                    var vo = new {typeName}(value);

                    if (!vo.IsValid() && !{typeName}PreSetValueCache.{typeName}PreSetValues.TryGetValue(value, out _)) {{
                        throw new System.InvalidOperationException(vo.GetValidationErrorMessage());
                    }}

                    return vo;
                }}

                public static bool TryFrom({config.UnderlyingTypeName} value, out {typeName} result) {{
                    result = value == default ? {emptyValueName} : new {typeName}(value);
                    {tryFromValidation}
                }}
";

        var comparison =
            config.Comparison == ComparisonGeneration.Omit
                ? ""
                : valueType.SpecialType == SpecialType.System_String
                    ? $@"
                public int CompareTo({typeName} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({config.UnderlyingTypeName}? other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {typeName} other1)
                        return this.CompareTo(other1);
                    if (other is {config.UnderlyingTypeName} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {typeName}"",
                        nameof(other)
                    );
                }}"
                    : $@"
                public int CompareTo({typeName} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({config.UnderlyingTypeName} other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {typeName} other1)
                        return this.CompareTo(other1);
                    if (other is {config.UnderlyingTypeName} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {typeName}"",
                        nameof(other)
                    );
                }}
";

        var conversionToUnderlyingTypeModifier =
            config.ToUnderlyingTypeCasting == CastOperator.Implicit ? "implicit" : "explicit";
        var conversionToUnderlyingType =
            config.ToUnderlyingTypeCasting == CastOperator.None
                ? ""
                : $@"
                /// <summary>
                ///     An implicit conversion from <see cref=""{typeName}"" /> to <see cref=""{config.UnderlyingTypeName}"" />.
                /// </summary>
                /// <param name=""id"">The value to convert.</param>
                /// <returns>The {config.UnderlyingTypeName} representation of the value object.</returns>
                public static {conversionToUnderlyingTypeModifier} operator {config.UnderlyingTypeName}({typeName} id)
                {{
                    return id.Value;
                }}";

        var conversionFromUnderlyingTypeModifier =
            config.FromUnderlyingTypeCasting == CastOperator.Implicit ? "implicit" : "explicit";
        var conversionFromUnderlyingType =
            config.FromUnderlyingTypeCasting == CastOperator.None
                ? ""
                : $@"
                /// <summary>
                ///     An explicit conversion from <see cref=""{config.UnderlyingTypeName}"" /> to <see cref=""{typeName}"" />.
                /// </summary>
                /// <param name=""value"">The value to convert.</param>
                /// <returns>The <see cref=""{typeName}"" /> instance created from the input value.</returns>
                public static {conversionFromUnderlyingTypeModifier} operator {typeName}({config.UnderlyingTypeName} value)
                {{
                    return {typeName}.From(value);
                }}";

        var validationClasses =
            @"
private class Validation
{
    public static readonly Validation Ok = new(string.Empty);
    private readonly bool _isSuccess;

    private Validation(string reason)
    {
        ErrorMessage = reason;
        _isSuccess = string.IsNullOrEmpty(reason);
    }

    public string ErrorMessage { get; }
    public bool IsSuccess => _isSuccess;

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

        var validationMembers =
            @"
public bool IsValid() => _validation.IsSuccess;
public string? GetValidationErrorMessage() => _validation.IsSuccess ? null : _validation.ErrorMessage;
";

        var formattableType = target.SemanticModel.Compilation.GetTypeByMetadataName(
            "System.IFormattable"
        );
        var isFormattable =
            formattableType != null && valueType.AllInterfaces.Contains(formattableType);

        var formattableToString = isFormattable
            ? @"
                /// <inheritdoc />
                public override string ToString()
                {
                    return Value.ToString();
                }

                /// <inheritdoc cref=""M:System.String.ToString(System.IFormatProvider)"" />
                public string ToString(IFormatProvider? provider)
                {
                    return Value.ToString(format: null, provider: provider) ?? """";
                }

                /// <inheritdoc />
                public string ToString(string? format, IFormatProvider? formatProvider)
                {{
                    return Value.ToString(format, formatProvider) ?? """";
                }}"
            : @"
                /// <inheritdoc />
                public override string ToString()
                {
                    return Value.ToString();
                }";

        var toStringOverrides =
            valueType.SpecialType == SpecialType.System_String
                ? @"
                /// <inheritdoc />
                public override string ToString()
                {{
                    return Value ?? """";
                }}

                /// <inheritdoc cref=""M:System.String.ToString(System.IFormatProvider)"" />
                public string ToString(IFormatProvider? provider)
                {{
                    return Value.ToString(provider: provider) ?? """";
                }}
"
                : formattableToString;

        var interfaceDefsBuilder = new StringBuilder();
        interfaceDefsBuilder.Append($": IEquatable<{typeName}>");

        if (isFormattable)
        {
            interfaceDefsBuilder.Append(", IFormattable");
        }

        if (
            (
                config.UnderlyingTypeEqualityGeneration
                & UnderlyingTypeEqualityGeneration.GenerateMethods
            ) == UnderlyingTypeEqualityGeneration.GenerateMethods
        )
        {
            interfaceDefsBuilder.Append($", IEquatable<{config.UnderlyingTypeName}>");
        }

        if (config.ParsableGeneration == ParsableGeneration.Generate)
        {
            interfaceDefsBuilder.Append(
                $", ISpanParsable<{typeName}>, IUtf8SpanParsable<{typeName}>"
            );
        }

        if (config.Comparison != ComparisonGeneration.Omit)
        {
            interfaceDefsBuilder.Append($", IComparable<{typeName}>");
            interfaceDefsBuilder.Append(", IComparable");
        }

        var interfaceDefs = interfaceDefsBuilder.ToString();

        var jsonConverter = new JsonConverterFragmentProvider().ProvideFragment(config);
        var typeConverter = new TypeConverterFragmentProvider().ProvideFragment(config);

        var preSetValues = target.SymbolInformation
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f.IsStatic
                        && f.DeclaredAccessibility == Accessibility.Public
                        && SymbolEqualityComparer.Default.Equals(f.Type, target.SymbolInformation))
            .ToList();

        var preSetValueCache =
            $@"
private static class {typeName}PreSetValueCache {{
    public static readonly Dictionary<{config.UnderlyingTypeName}, {typeName}> {typeName}PreSetValues = new();

    static {typeName}PreSetValueCache()
    {{
        {typeName}PreSetValues[{typeName}.{emptyValueName}.Value] = {typeName}.{emptyValueName};
{
    string.Join("\n", preSetValues.Select(f => $"        {typeName}PreSetValues[{typeName}.{f.Name}.Value] = {typeName}.{f.Name};"))
}
    }}
}}";

        var parsable =
            config.ParsableGeneration != ParsableGeneration.Generate
                ? ""
                : valueType.SpecialType == SpecialType.System_String
                    ? $@"
    /// <inheritdoc />
    public static {typeName} Parse(string s, IFormatProvider? provider)
    {{
        return From(s);
    }}

    /// <inheritdoc />
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out {typeName} result
    )
    {{
        return TryFrom(s, out result);
    }}

    /// <inheritdoc />
    public static {typeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {{
        return From(new string(s));
    }}

    /// <inheritdoc />
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out {typeName} result
    )
    {{
        return TryFrom(new string(s), out result);
    }}

    /// <inheritdoc />
    public static {typeName} Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
    {{
        var s = System.Text.Encoding.UTF8.GetString(utf8Text);
        return From(s);
    }}

    /// <inheritdoc />
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out {typeName} result
    )
    {{
        try
        {{
            var s = System.Text.Encoding.UTF8.GetString(utf8Text);
            return TryFrom(s, out result);
        }}
        catch (ArgumentException)
        {{
            result = default;
            return false;
        }}
    }}"
                    : $@"
    /// <inheritdoc />
    public static {typeName} Parse(string s, IFormatProvider? provider)
    {{
        var v = {config.UnderlyingTypeName}.Parse(s, provider);
        return From(v);
    }}

    /// <inheritdoc />
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out {typeName} result
    )
    {{
        try
        {{
            var v = s == null ? default : {config.UnderlyingTypeName}.Parse(s, provider);
            return TryFrom(v, out result);
        }}
        catch (ArgumentException)
        {{
            result = default;
            return false;
        }}
        catch (FormatException)
        {{
            result = default;
            return false;
        }}
    }}

    /// <inheritdoc />
    public static {typeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {{
        var v = {config.UnderlyingTypeName}.Parse(s, provider);
        return From(v);
    }}

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out {typeName} result)
    {{
        try
        {{
            var v = {config.UnderlyingTypeName}.Parse(new string(s), provider);
            return TryFrom(v, out result);
        }}
        catch (ArgumentException)
        {{
            result = default;
            return false;
        }}
        catch (FormatException)
        {{
            result = default;
            return false;
        }}
    }}

    /// <inheritdoc />
    public static {typeName} Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
    {{
        var s = System.Text.Encoding.UTF8.GetString(utf8Text);
        var v = {config.UnderlyingTypeName}.Parse(s, provider);
        return From(v);
    }}

    /// <inheritdoc />
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out {typeName} result
    )
    {{
        try
        {{
            var s = System.Text.Encoding.UTF8.GetString(utf8Text);
            var v = {config.UnderlyingTypeName}.Parse(s, provider);
            return TryFrom(v, out result);
        }}
        catch (ArgumentException)
        {{
            result = default;
            return false;
        }}
        catch (FormatException)
        {{
            result = default;
            return false;
        }}
    }}";

        var containingTypes = GetContainingTypesDeclarations(target.SymbolInformation);
        var closingBraces = GetClosingBraces(target.SymbolInformation);

        var generatedClass =
            $@"
        #nullable enable

        namespace {namespaceName} {{
            {containingTypes}
            [System.Diagnostics.DebuggerDisplay(""{typeName} {{Value}}"")]
            [System.Text.Json.Serialization.JsonConverter(typeof({typeName}SystemTextJsonConverter))]
            [System.ComponentModel.TypeConverter(typeof({typeName}TypeConverter))]
            public partial record struct {typeName} {interfaceDefs} {{
                private readonly {config.UnderlyingTypeName} _value;
                private readonly bool _initialized;
#pragma warning disable CS0414
                private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
                private readonly Validation _validation;
                private static readonly Type UnderlyingType = typeof({config.UnderlyingTypeName});

                public {config.UnderlyingTypeName} Value => _value;

                {creation}

                public static {typeName} {emptyValueName} {{ get; }} = new {typeName}({defaultValue});

                public bool IsInitialized() => _initialized;

                {equality}

                {equalityUnderlyingType}

                {equalityOperators}

                {comparison}

                {conversionToUnderlyingType}

                {conversionFromUnderlyingType}

                {toStringOverrides}

                {validationMembers}
                {validationClasses}

                {parsable}

                {jsonConverter}
                {typeConverter}
                {preSetValueCache}
            }}
            {closingBraces}
        }}
        ";

        context.AddSource($"{typeName}.g.cs", generatedClass);
    }

    private static string GetContainingTypeNames(INamedTypeSymbol symbol)
    {
        var types = new Stack<string>();
        var current = symbol.ContainingType;
        while (current != null)
        {
            types.Push(current.Name);
            current = current.ContainingType;
        }

        return string.Join(".", types);
    }

    private static string GetContainingTypesDeclarations(INamedTypeSymbol symbol)
    {
        var types = new Stack<string>();
        var current = symbol.ContainingType;
        while (current != null)
        {
            var kind = current.TypeKind switch
            {
                TypeKind.Class => "class",
                TypeKind.Struct => "struct",
                TypeKind.Interface => "interface",
                TypeKind.Enum => "enum",
                _ => "class",
            };
            var recordModifier = current.IsRecord ? "record " : "";
            types.Push($"public partial {recordModifier}{kind} {current.Name} {{");
            current = current.ContainingType;
        }

        return string.Join("\n", types);
    }

    private static string GetClosingBraces(INamedTypeSymbol symbol)
    {
        var count = 0;
        var current = symbol.ContainingType;
        while (current != null)
        {
            count++;
            current = current.ContainingType;
        }

        return new string('}', count);
    }

    private static SourceText WithNamespace(CompilationUnitSyntax root, string newNs)
    {
        // Remove existing file-scoped namespace declaration (if any)
        var members = root.Members;
        var fileScopedNamespace = members
            .OfType<FileScopedNamespaceDeclarationSyntax>()
            .FirstOrDefault();

        if (fileScopedNamespace != null)
        {
            members = fileScopedNamespace.Members;
        }

        // Create a new namespace declaration with the specified namespace name
        var newNamespace = SyntaxFactory
            .NamespaceDeclaration(SyntaxFactory.ParseName(newNs))
            .WithMembers(members)
            .NormalizeWhitespace();

        // Create a new compilation unit preserving existing usings but replacing members with the new namespace
        var newCompilationUnit = SyntaxFactory
            .CompilationUnit()
            .WithUsings(root.Usings)
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(newNamespace))
            .NormalizeWhitespace();

        // Convert to source text
        var sourceCode = newCompilationUnit.ToFullString();
        return SourceText.From(sourceCode, Encoding.UTF8);
    }

    private void EnsureValueObjectAttribute(IncrementalGeneratorInitializationContext context)
    {
        // Get namespace from build properties
        var namespaceName = context.AnalyzerConfigOptionsProvider.Select((p, _) =>
            p.GlobalOptions.TryGetValue("build_property.RootNamespace", out var ns)
            && !string.IsNullOrWhiteSpace(ns)
                ? ns
                : p.GlobalOptions.TryGetValue("build_property.AssemblyName", out var asm)
                  && !string.IsNullOrWhiteSpace(asm)
                    ? asm
                    : typeof(ValueObjectAttribute).Namespace
        );

        context.RegisterSourceOutput(
            namespaceName,
            (spc, ns) =>
            {
                var attributeContent = GetEmbeddedResource($"{nameof(ValueObjectAttribute)}.cs");
                if (attributeContent == null)
                {
                    return;
                }

                var tree = CSharpSyntaxTree.ParseText(attributeContent);
                var root = (CompilationUnitSyntax)tree.GetRoot();
                var fixedSource = WithNamespace(root, ns ?? "global::");

                spc.AddSource($"{nameof(ValueObjectAttribute)}.g.cs", fixedSource);
            }
        );
    }

    private static string? GetEmbeddedResource(string resourceName)
    {
        var assembly = typeof(ValueObjectAttribute).Assembly;
        var fullResourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceName));

        if (fullResourceName == null)
        {
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}