using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

                var attributeData = symbolInformation.TryGetValueObjectAttributes()
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

        var jsonConverter = JsonConverterTemplate
            .Replace("{{className}}", className)
            .Replace("{{valueTypeName}}", valueTypeName);

        var typeConverter = TypeConverterTemplate
            .Replace("{{className}}", className)
            .Replace("{{valueTypeName}}", valueTypeName);

        var generatedClass =
            $@"
        #nullable enable

        namespace {namespaceName} {{
            [System.Diagnostics.DebuggerDisplay(""{className} {{Value}}"")]
            [System.Text.Json.Serialization.JsonConverter(typeof({className}SystemTextJsonConverter))]
            [System.ComponentModel.TypeConverter(typeof({className}TypeConverter))]
            public partial record struct {className} {interfaceDefs} {{
                private readonly {valueTypeName} _value;
                private static readonly Type UnderlyingType = typeof({valueTypeName});

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

                {jsonConverter}
                {typeConverter}
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

    private const string TypeConverterTemplate =
        @"
private class {{className}}TypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == UnderlyingType;
    }
    
    public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        if (value != null && !CanConvertFrom(context, value.GetType()))
        {
            throw new NotSupportedException($""Cannot convert from type '{value?.GetType()}'."");
        }

        var underlyingValue = GetUnderlyingValue(value);

        return underlyingValue == default ? Empty : From(({{valueTypeName}})underlyingValue);
    }

    private object? GetUnderlyingValue(object? value) {{
        if (value == null) {{
            return default({{valueTypeName}});
        }}

        if (value is {{valueTypeName}} v) {
            return v;
        }
        
        if (Type.GetTypeCode(typeof({{valueTypeName}})) == TypeCode.Object) {
            throw new NotSupportedException($""Cannot convert value of type '{value?.GetType()}' to '{{valueTypeName}}'."");
        }
        
        return Convert.ChangeType(value, typeof({{valueTypeName}}));
    }}
    
    public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == UnderlyingType;
    }
    
    public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
    {
        if (!CanConvertTo(context, destinationType))
        {
            throw new NotSupportedException($""Cannot convert to type '{destinationType}'."");
        }

        if (value is {{className}} vo)
        {
            return vo.Value;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
";

    private const string JsonConverterTemplate =
        @"
private class {{className}}SystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<{{className}}>
{
    public override {{className}} Read(
        ref System.Text.Json.Utf8JsonReader reader,
        Type typeToConvert,
        System.Text.Json.JsonSerializerOptions options
    )
    {
        var underlyingType = {{className}}.UnderlyingType;
        object? underlyingValue;
    
        switch (Type.GetTypeCode(underlyingType)) {
            case TypeCode.Boolean:
                if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetBoolean();
                break;
            case TypeCode.Byte:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetByte();
                break;
            case TypeCode.Char:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                var charStr = reader.GetString();
                if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                    throw new System.Text.Json.JsonException($""Cannot convert '{charStr}' to char."");
                underlyingValue = charStr[0];
                break;
            case TypeCode.Decimal:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetDecimal();
                break;
            case TypeCode.Double:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetDouble();
                break;
            case TypeCode.Single:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetSingle();
                break;
            case TypeCode.Int16:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetInt16();
                break;
            case TypeCode.Int32:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetInt32();
                break;
            case TypeCode.Int64:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetInt64();
                break;
            case TypeCode.String:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetString();
                break;
            case TypeCode.DateTime:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                underlyingValue = reader.GetDateTime();
                break;
            default:
                if (underlyingType == typeof(Guid)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                    var guidStr = reader.GetString();
                    if (!Guid.TryParse(guidStr, out var guidValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{guidStr}' to Guid."");
                    underlyingValue = guidValue;
                } else if (underlyingType == typeof(DateTimeOffset)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                    underlyingValue = reader.GetDateTimeOffset();
                } else if (underlyingType == typeof(TimeSpan)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                    var tsStr = reader.GetString();
                    if (!TimeSpan.TryParse(tsStr, out var tsValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{tsStr}' to TimeSpan."");
                    underlyingValue = tsValue;
                } else if (underlyingType == typeof(TimeOnly)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                    var timeStr = reader.GetString();
                    if (!TimeOnly.TryParse(timeStr, out var timeValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{timeStr}' to TimeOnly."");
                    underlyingValue = timeValue;
                } else if (underlyingType == typeof(Uri)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{className}}."");
                    var uriStr = reader.GetString();
                    if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{uriStr}' to Uri."");
                    underlyingValue = uriValue;
                } else {
                    throw new System.Text.Json.JsonException($""Unsupported underlying type for {{className}}."");
                }
                break;
        }
    
        try {
            return {{className}}.From(({{valueTypeName}})underlyingValue!);
        } catch (System.Exception e) {
            throw new System.Text.Json.JsonException(""Could not create an initialized instance of {{className}}."", e);
        }
    }
    
    public override void Write(
        System.Text.Json.Utf8JsonWriter writer,
        {{className}} value,
        System.Text.Json.JsonSerializerOptions options
    )
    {
        var underlyingType = {{className}}.UnderlyingType;
        object underlyingValue = value.IsInitialized()
            ? value.Value
            : {{className}}.Empty.Value;
    
        switch (Type.GetTypeCode(underlyingType)) {
            case TypeCode.Boolean:
                writer.WriteBooleanValue((bool)underlyingValue);
                break;
            case TypeCode.Byte:
                writer.WriteNumberValue((byte)underlyingValue);
                break;
            case TypeCode.Char:
                writer.WriteStringValue(((char)underlyingValue).ToString());
                break;
            case TypeCode.Decimal:
                writer.WriteNumberValue((decimal)underlyingValue);
                break;
            case TypeCode.Double:
                writer.WriteNumberValue((double)underlyingValue);
                break;
            case TypeCode.Single:
                writer.WriteNumberValue((float)underlyingValue);
                break;
            case TypeCode.Int16:
                writer.WriteNumberValue((short)underlyingValue);
                break;
            case TypeCode.Int32:
                writer.WriteNumberValue((int)underlyingValue);
                break;
            case TypeCode.Int64:
                writer.WriteNumberValue((long)underlyingValue);
                break;
            case TypeCode.String:
                writer.WriteStringValue((string)underlyingValue);
                break;
            case TypeCode.DateTime:
                writer.WriteStringValue(((DateTime)underlyingValue));
                break;
            default:
                if (underlyingType == typeof(Guid)) {
                    writer.WriteStringValue(((Guid)underlyingValue));
                } else if (underlyingType == typeof(DateTimeOffset)) {
                    writer.WriteStringValue(((DateTimeOffset)underlyingValue));
                } else if (underlyingType == typeof(TimeSpan)) {
                    writer.WriteStringValue(((TimeSpan)underlyingValue).ToString());
                } else if (underlyingType == typeof(TimeOnly)) {
                    writer.WriteStringValue(((TimeOnly)underlyingValue).ToString());
                } else if (underlyingType == typeof(Uri)) {
                    writer.WriteStringValue(((Uri)underlyingValue).ToString());
                } else {
                    throw new System.Text.Json.JsonException($""Unsupported underlying type for {{className}}."");
                }
                break;
        }
    }
}
";
}
