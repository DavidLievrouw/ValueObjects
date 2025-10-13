using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Dalion.ValueObjects.Generation;

/// <inheritdoc />
[Generator]
public class ValueObjectGenerator : IIncrementalGenerator
{
    private const string TypeConverterTemplate =
        @"
private class {{typeName}}TypeConverter : System.ComponentModel.TypeConverter
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

        return underlyingValue == default ? {{emptyValueName}} : From(({{valueTypeName}})underlyingValue);
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

        if (value is {{typeName}} vo)
        {
            return vo.Value;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
";

    private const string JsonConverterTemplate =
        @"
private class {{typeName}}SystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<{{typeName}}>
{
    public override {{typeName}} Read(
        ref System.Text.Json.Utf8JsonReader reader,
        Type typeToConvert,
        System.Text.Json.JsonSerializerOptions options
    )
    {
        if (reader.TokenType == System.Text.Json.JsonTokenType.Null) {
            return new {{typeName}}();
        }

        var underlyingType = {{typeName}}.UnderlyingType;
        object? underlyingValue;
    
        switch (Type.GetTypeCode(underlyingType)) {
            case TypeCode.Boolean:
                if (reader.TokenType != System.Text.Json.JsonTokenType.True && reader.TokenType != System.Text.Json.JsonTokenType.False)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetBoolean();
                break;
            case TypeCode.Byte:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetByte();
                break;
            case TypeCode.Char:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                var charStr = reader.GetString();
                if (string.IsNullOrEmpty(charStr) || charStr.Length != 1)
                    throw new System.Text.Json.JsonException($""Cannot convert '{charStr}' to char."");
                underlyingValue = charStr[0];
                break;
            case TypeCode.Decimal:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetDecimal();
                break;
            case TypeCode.Double:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetDouble();
                break;
            case TypeCode.Single:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetSingle();
                break;
            case TypeCode.Int16:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetInt16();
                break;
            case TypeCode.Int32:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetInt32();
                break;
            case TypeCode.Int64:
                if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetInt64();
                break;
            case TypeCode.String:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetString();
                break;
            case TypeCode.DateTime:
                if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                    throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                underlyingValue = reader.GetDateTime();
                break;
            default:
                if (underlyingType == typeof(Guid)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                    var guidStr = reader.GetString();
                    if (!Guid.TryParse(guidStr, out var guidValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{guidStr}' to Guid."");
                    underlyingValue = guidValue;
                } else if (underlyingType == typeof(DateTimeOffset)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                    underlyingValue = reader.GetDateTimeOffset();
                } else if (underlyingType == typeof(TimeSpan)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                    var tsStr = reader.GetString();
                    if (!TimeSpan.TryParse(tsStr, out var tsValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{tsStr}' to TimeSpan."");
                    underlyingValue = tsValue;
                } else if (underlyingType == typeof(TimeOnly)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                    var timeStr = reader.GetString();
                    if (!TimeOnly.TryParse(timeStr, out var timeValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{timeStr}' to TimeOnly."");
                    underlyingValue = timeValue;
                } else if (underlyingType == typeof(Uri)) {
                    if (reader.TokenType != System.Text.Json.JsonTokenType.String)
                        throw new System.Text.Json.JsonException($""Unsupported JSON token type for {{typeName}}."");
                    var uriStr = reader.GetString();
                    if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uriValue))
                        throw new System.Text.Json.JsonException($""Cannot convert '{uriStr}' to Uri."");
                    underlyingValue = uriValue;
                } else {
                    throw new System.Text.Json.JsonException($""Unsupported underlying type for {{typeName}}."");
                }
                break;
        }
    
        try {
            var typedUnderlyingValue = ({{valueTypeName}})underlyingValue!;
            if (typedUnderlyingValue == default || underlyingValue is System.String suv && suv == System.String.Empty) {
                return {{typeName}}.{{emptyValueName}};
            }
            return {{typeName}}.From(typedUnderlyingValue);
        } catch (System.Exception e) {
            throw new System.Text.Json.JsonException(""Could not create an initialized instance of {{typeName}}."", e);
        }
    }
    
    public override void Write(
        System.Text.Json.Utf8JsonWriter writer,
        {{typeName}} value,
        System.Text.Json.JsonSerializerOptions options
    )
    {
        var underlyingType = {{typeName}}.UnderlyingType;
        object? underlyingValue = value.IsInitialized()
            ? value.Value
            : null;

        if (underlyingValue == null) {
            writer.WriteNullValue();
            return;
        }
    
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
                    throw new System.Text.Json.JsonException($""Unsupported underlying type for {{typeName}}."");
                }
                break;
        }
    }
}
";

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

    private void AddGeneratedValueObject(GenerationTarget target, SourceProductionContext context)
    {
        var typeName = target.SymbolInformation.Name;
        var namespaceName = target.SymbolInformation.ContainingNamespace.ToDisplayString();

        var config = target.GetAttributeConfiguration();
        var valueType = config.UnderlyingType;
        var valueTypeName = valueType.FullName;
        var emptyValueName = config.EmptyValueName;

        var defaultValue = valueType == typeof(string) ? "System.String.Empty" : "default";

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
                    target.SemanticModel.Compilation.GetTypeByMetadataName(valueType.FullName!)
                )
            );

        var validationFieldAssignment =
            validateMethod == null
                ? "_validation = Validation.Ok;"
                : "_validation = Validate(_value);";

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
                    target.SemanticModel.Compilation.GetTypeByMetadataName(valueType.FullName!)
                )
                && SymbolEqualityComparer.Default.Equals(
                    target.SemanticModel.GetTypeInfo(member.ReturnType).Type!,
                    target.SemanticModel.Compilation.GetTypeByMetadataName(valueType.FullName!)
                )
            );
        var inputNormalization = normalizeMethod == null ? "" : "value = NormalizeInput(value);";

        var stringComparison =
            config.CaseSensitivity == StringCaseSensitivity.CaseInsensitive
                ? "OrdinalIgnoreCase"
                : "Ordinal";

        var equality =
            valueType == typeof(string)
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
                        : {valueTypeName}.Equals(this._value, other.Value.Value, System.StringComparison.{stringComparison});
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
                        : {valueTypeName}.Equals(this._value, other.Value, System.StringComparison.{stringComparison});
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
            
                    return EqualityComparer<{valueTypeName}>.Default.Equals(this._value, other.Value.Value);
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
            
                    return EqualityComparer<{valueTypeName}>.Default.Equals(this._value, other.Value);
                }}
            
                public bool Equals({typeName}? other, IEqualityComparer<{typeName}> comparer)
                {{
                    if (other is null) return false;
                    return comparer.Equals(this, other.Value);
                }}
            
                /// <inheritdoc />
                public override int GetHashCode() {{
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<{valueTypeName}>.Default.GetHashCode(this._value);
                }}";

        var equalityUnderlyingType =
        (
            config.UnderlyingTypeEqualityGeneration
            & UnderlyingTypeEqualityGeneration.GenerateMethods
        ) == UnderlyingTypeEqualityGeneration.GenerateMethods
            ? valueType == typeof(string)
                ? $@"
                /// <inheritdoc />
                public bool Equals({valueTypeName}? other)
                {{
                    return {valueTypeName}.IsNullOrEmpty(other)
                        ? this._isNullOrEmpty
                        : {valueTypeName}.Equals(this._value, other, System.StringComparison.{stringComparison});
                }}
            
                public bool Equals({valueTypeName}? underlyingValue, StringComparer comparer)
                {{
                    return comparer.Equals(this.Value, underlyingValue);
                }}"
                : $@"
                /// <inheritdoc />
                public bool Equals({valueTypeName} other)
                {{
                    return EqualityComparer<{valueTypeName}>.Default.Equals(this._value, other);
                }}"
            : "";

        var equalityOperators =
        (
            config.UnderlyingTypeEqualityGeneration
            & UnderlyingTypeEqualityGeneration.GenerateOperators
        ) == UnderlyingTypeEqualityGeneration.GenerateOperators
            ? valueType == typeof(string)
                ? $@"
    public static bool operator ==({typeName} left, {valueTypeName}? right) => left.Value.Equals(right);

    public static bool operator ==({valueTypeName}? left, {typeName} right) => right.Value.Equals(left);

    public static bool operator !=({valueTypeName}? left, {typeName} right) => !(left == right);

    public static bool operator !=({typeName} left, {valueTypeName}? right) => !(left == right);
"
                : $@"
    public static bool operator ==({typeName} left, {valueTypeName} right) => left.Value.Equals(right);

    public static bool operator ==({valueTypeName} left, {typeName} right) => right.Value.Equals(left);

    public static bool operator !=({valueTypeName} left, {typeName} right) => !(left == right);

    public static bool operator !=({typeName} left, {valueTypeName} right) => !(left == right);
"
            : "";

        var creation =
            valueType == typeof(string)
                ? $@"
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public {typeName}()
                {{
                    _value = {valueTypeName}.Empty;
                    _initialized = false;
                    _isNullOrEmpty = {valueTypeName}.IsNullOrEmpty(_value);
                    {validationFieldAssignment}
                }}

                [System.Diagnostics.DebuggerStepThrough]
                private {typeName}({valueTypeName}? value) {{
                    {inputNormalization}
                    if (value == default) {{
                        _initialized = false;
                        _value = {valueTypeName}.Empty;
                    }} else {{
                        _initialized = true;
                        _value = value;
                    }}
                    _isNullOrEmpty = {valueTypeName}.IsNullOrEmpty(_value);
                    {validationFieldAssignment}
                }}

                [System.Diagnostics.DebuggerStepThrough]
                private {typeName}({valueTypeName}? value, bool validation) {{
                    {inputNormalization}
                    if (validation) {{
                        {ctorValidation}
                    }}
                    if (value == default) {{
                        _initialized = false;
                        _value = {valueTypeName}.Empty;
                    }} else {{
                        _initialized = true;
                        _value = value;
                    }}
                    _isNullOrEmpty = {valueTypeName}.IsNullOrEmpty(_value);
                    {validationFieldAssignment}
                }}

                public static {typeName} From({valueTypeName}? value) {{
                    if (value is null) {{
                        {ctorValidation}
                        var instance = new {typeName}();
                        return instance;
                    }}

                    return new {typeName}(value, validation: true);
                }}

                public static bool TryFrom({valueTypeName}? value, out {typeName} result) {{
                    if (value is null) {{
                        result = new {typeName}();
                        {tryFromValidation}
                    }}

                    result = string.IsNullOrEmpty(value) ? {emptyValueName} : new {typeName}(value, validation: false);
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

                private {typeName}({valueTypeName} value) {{
                    {inputNormalization}
                    _initialized = true;
                    _value = value;
                    _isNullOrEmpty = false;
                    {validationFieldAssignment}
                }}

                private {typeName}({valueTypeName} value, bool validation) {{
                    {inputNormalization}
                    if (validation) {{
                        {ctorValidation}
                    }}
                    _initialized = true;
                    _value = value;
                    _isNullOrEmpty = false;
                    {validationFieldAssignment}
                }}

                public static {typeName} From({valueTypeName} value) {{
                    if (value == default) {{
                        {ctorValidation}
                        return {emptyValueName};
                    }}

                    return new {typeName}(value, validation: true);
                }}

                public static bool TryFrom({valueTypeName} value, out {typeName} result) {{
                    result = value == default ? {emptyValueName} : new {typeName}(value, validation: false);
                    {tryFromValidation}
                }}
";

        var comparison =
            config.Comparison == ComparisonGeneration.Omit
                ? ""
                : valueType == typeof(string)
                    ? $@"
                public int CompareTo({typeName} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({valueTypeName}? other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {typeName} other1)
                        return this.CompareTo(other1);
                    if (other is {valueTypeName} v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        ""Cannot compare to object as it is not of type {typeName}"",
                        nameof(other)
                    );
                }}"
                    : $@"
                public int CompareTo({typeName} other) => this.Value.CompareTo(other.Value);

                public int CompareTo({valueTypeName} other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {{
                    if (other == null)
                        return 1;
                    if (other is {typeName} other1)
                        return this.CompareTo(other1);
                    if (other is {valueTypeName} v)
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
                ///     An implicit conversion from <see cref=""{typeName}"" /> to <see cref=""{valueTypeName}"" />.
                /// </summary>
                /// <param name=""id"">The value to convert.</param>
                /// <returns>The {valueTypeName} representation of the value object.</returns>
                public static {conversionToUnderlyingTypeModifier} operator {valueTypeName}({typeName} id)
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
                ///     An explicit conversion from <see cref=""{valueTypeName}"" /> to <see cref=""{typeName}"" />.
                /// </summary>
                /// <param name=""value"">The value to convert.</param>
                /// <returns>The <see cref=""{typeName}"" /> instance created from the input value.</returns>
                public static {conversionFromUnderlyingTypeModifier} operator {typeName}({valueTypeName} value)
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
        var toStringOverrides =
            valueType == typeof(string)
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
                : @"
                /// <inheritdoc />
                public override string ToString()
                {
                    return Value is IFormattable f 
                        ? f.ToString(format: null, formatProvider: System.Globalization.CultureInfo.InvariantCulture)
                        : Value.ToString() ?? """";
                }

                /// <inheritdoc cref=""M:System.String.ToString(System.IFormatProvider)"" />
                public string ToString(IFormatProvider? provider)
                {
                    return Value.ToString(format: null, provider: provider) ?? """";
                }
";

        var interfaceDefsBuilder = new StringBuilder();
        interfaceDefsBuilder.AppendLine($": IEquatable<{typeName}>");
        if (
            (
                config.UnderlyingTypeEqualityGeneration
                & UnderlyingTypeEqualityGeneration.GenerateMethods
            ) == UnderlyingTypeEqualityGeneration.GenerateMethods
        )
        {
            interfaceDefsBuilder.Append($", IEquatable<{valueTypeName}>");
        }

        if (config.Comparison != ComparisonGeneration.Omit)
        {
            interfaceDefsBuilder.Append($", IComparable<{typeName}>");
            interfaceDefsBuilder.Append(", IComparable");
        }

        var interfaceDefs = interfaceDefsBuilder.ToString();

        var jsonConverter = JsonConverterTemplate
            .Replace("{{typeName}}", typeName)
            .Replace("{{valueTypeName}}", valueTypeName)
            .Replace("{{emptyValueName}}", emptyValueName);

        var typeConverter = TypeConverterTemplate
            .Replace("{{typeName}}", typeName)
            .Replace("{{valueTypeName}}", valueTypeName)
            .Replace("{{emptyValueName}}", emptyValueName);

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
                private readonly {valueTypeName} _value;
                private readonly bool _initialized;
#pragma warning disable CS0414
                private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
                private readonly Validation _validation;
                private static readonly Type UnderlyingType = typeof({valueTypeName});

                public {valueTypeName} Value => _value;

                {creation}

                public static {typeName} {emptyValueName} {{ get; }} = new {typeName}({defaultValue}, validation: false);

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

                {jsonConverter}
                {typeConverter}
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
                var fixedSource = WithNamespace(root, ns);

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