using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class CreationProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
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
                    target.SemanticModel.Compilation.GetTypeByMetadataName(
                        config.UnderlyingTypeName
                    )
                )
            );

        var validationFieldAssignment =
            validateMethod == null
                ? "_validation ??= Validation.Ok;"
                : "_validation ??= Validate(_value);";

        var tryFromValidation =
            validateMethod == null
                ? "return result.IsInitialized();"
                : $"return result.IsInitialized() && (Validate(result._value).IsSuccess || {config.TypeName}PreSetValueCache.{config.TypeName}PreSetValues.TryGetValue(value, out _));";

        var normalizeMethod = target
            .SyntaxInformation.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(member =>
                member.Identifier.Text == "Normalize"
                && member.Modifiers.Any(SyntaxKind.PrivateKeyword)
                && member.Modifiers.Any(SyntaxKind.StaticKeyword)
                && member.ParameterList.Parameters.Count == 1
                && SymbolEqualityComparer.Default.Equals(
                    target
                        .SemanticModel.GetTypeInfo(member.ParameterList.Parameters[0].Type!)
                        .Type!,
                    target.SemanticModel.Compilation.GetTypeByMetadataName(
                        config.UnderlyingTypeName
                    )
                )
                && SymbolEqualityComparer.Default.Equals(
                    target.SemanticModel.GetTypeInfo(member.ReturnType).Type!,
                    target.SemanticModel.Compilation.GetTypeByMetadataName(
                        config.UnderlyingTypeName
                    )
                )
            );
        var inputNormalization = normalizeMethod == null ? "" : "value = Normalize(value);";

        return config.UnderlyingType.SpecialType == SpecialType.System_String
            ? GetForString(
                config,
                validationFieldAssignment,
                tryFromValidation,
                inputNormalization
            ).Trim()
            : GetForValueType(
                config,
                validationFieldAssignment,
                tryFromValidation,
                inputNormalization
            ).Trim();
    }

    private static string GetForValueType(
        AttributeConfiguration config,
        string validationFieldAssignment,
        string tryFromValidation,
        string inputNormalization)
    {
        return $@"
        /// <summary>
        ///     Creates a new <see cref=""{config.TypeName}""/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public {config.TypeName}()
        {{
            _value = default;
            _initialized = false;
            _isNullOrEmpty = false;
            {validationFieldAssignment}
        }}

        /// <summary>
        ///     Creates a new <see cref=""{config.TypeName}""/>.
        /// </summary>
        /// <param name=""value"">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private {config.TypeName}({config.UnderlyingTypeName} value) {{
            {inputNormalization}
            _initialized = true;
            _value = value;
            _isNullOrEmpty = false;
            {validationFieldAssignment}
        }}

        /// <summary>
        ///     Creates a new <see cref=""{config.TypeName}""/> from the
        ///     given <see cref=""{config.UnderlyingTypeName}""/>.
        /// </summary>
        /// <param name=""value"">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref=""{config.TypeName}""/>.</returns>
        public static {config.TypeName} From({config.UnderlyingTypeName} value) {{
            if (value == default) {{
                return {config.EmptyValueName};
            }}

            var vo = new {config.TypeName}(value);

            if (!vo.IsValid() && !{config.TypeName}PreSetValueCache.{config.TypeName}PreSetValues.TryGetValue(vo.Value, out _)) {{
                throw new System.ArgumentException(vo.GetValidationErrorMessage());
            }}

            return vo;
        }}

        /// <summary>
        ///     Tries to create a new <see cref=""{config.TypeName}""/> from the
        ///     given <see cref=""{config.UnderlyingTypeName}""/>.
        /// </summary>
        /// <param name=""value"">The underlying value to create the value object from.</param>
        /// <param name=""result"">The resulting value object if the method returns <see langword=""true""/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword=""true""/> if the value object was created successfully; otherwise, <see langword=""false""/>.</returns>
        public static bool TryFrom({config.UnderlyingTypeName} value, out {config.TypeName} result) {{
            result = value == default ? {config.EmptyValueName} : new {config.TypeName}(value);
            {tryFromValidation}
        }}";
    }

    private static string GetForString(
        AttributeConfiguration config,
        string validationFieldAssignment,
        string tryFromValidation,
        string inputNormalization
    )
    {
        return $@"
        /// <summary>
        ///     Creates a new <see cref=""{config.TypeName}""/>.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public {config.TypeName}()
        {{
            _value = {config.UnderlyingTypeName}.Empty;
            _initialized = false;
            _isNullOrEmpty = {config.UnderlyingTypeName}.IsNullOrEmpty(_value);
            {validationFieldAssignment}
        }}

        /// <summary>
        ///     Creates a new <see cref=""{config.TypeName}""/>.
        /// </summary>
        /// <param name=""value"">The underlying value to create the value object from.</param>
        [System.Diagnostics.DebuggerStepThrough]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private {config.TypeName}({config.UnderlyingTypeName}? value) {{
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

        /// <summary>
        ///     Creates a new <see cref=""{config.TypeName}""/> from the
        ///     given <see cref=""{config.UnderlyingTypeName}""/>.
        /// </summary>
        /// <param name=""value"">The underlying value to create the value object from.</param>
        /// <returns>A new <see cref=""{config.TypeName}""/>.</returns>
        public static {config.TypeName} From({config.UnderlyingTypeName}? value) {{
            if (value is null) {{
                throw new System.ArgumentException(""Cannot create an instance of {config.TypeName} from null."");
            }}

            var vo = new {config.TypeName}(value);

            if (!vo.IsValid() && vo.Value is not null && !{config.TypeName}PreSetValueCache.{config.TypeName}PreSetValues.TryGetValue(vo.Value, out _)) {{
                throw new System.ArgumentException(vo.GetValidationErrorMessage());
            }}

            return vo;
        }}

        /// <summary>
        ///     Tries to create a new <see cref=""{config.TypeName}""/> from the
        ///     given <see cref=""{config.UnderlyingTypeName}""/>.
        /// </summary>
        /// <param name=""value"">The underlying value to create the value object from.</param>
        /// <param name=""result"">The resulting value object if the method returns <see langword=""true""/>; otherwise, an uninitialized value object.</param>
        /// <returns><see langword=""true""/> if the value object was created successfully; otherwise, <see langword=""false""/>.</returns>
        public static bool TryFrom({config.UnderlyingTypeName}? value, out {config.TypeName} result) {{
            if (value is null) {{
                result = new {config.TypeName}();
                return false;
            }}

            result = string.IsNullOrEmpty(value) ? {config.EmptyValueName} : new {config.TypeName}(value);
            {tryFromValidation}
        }}";
    }
}