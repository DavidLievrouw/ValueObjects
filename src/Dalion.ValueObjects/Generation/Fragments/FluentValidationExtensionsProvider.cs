namespace Dalion.ValueObjects.Generation.Fragments;

internal class FluentValidationExtensionsProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (
            config.FluentValidationExtensionsGeneration == FluentValidationExtensionsGeneration.Omit
        )
        {
            return null;
        }

        var containingTypeNames = target.SymbolInformation.GetContainingTypeNames();
        var containingTypes =
            containingTypeNames == string.Empty ? string.Empty : containingTypeNames + ".";

        var mustBeInitialized =
            (
                config.FluentValidationExtensionsGeneration
                & FluentValidationExtensionsGeneration.GenerateMustBeInitialized
            ) == FluentValidationExtensionsGeneration.GenerateMustBeInitialized
        ? $@"
        /// <summary>
        ///     Validates that the value object is initialized.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, {containingTypes}{config.TypeName}> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, {containingTypes}{config.TypeName}> ruleBuilder
        )
        {{
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($""{{nameof({containingTypes}{config.TypeName})}} must be initialized."");
        }}"
                : string.Empty;

        var mustBeInitializedAndValid =
            (
                config.FluentValidationExtensionsGeneration
                & FluentValidationExtensionsGeneration.GenerateMustBeInitializedAndValid
            ) == FluentValidationExtensionsGeneration.GenerateMustBeInitializedAndValid
                ? $@"
        /// <summary>
        ///     Validates that the value object is initialized and valid.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, {containingTypes}{config.TypeName}> MustBeInitializedAndValid<T>(
            this FluentValidation.IRuleBuilderInitial<T, {containingTypes}{config.TypeName}> ruleBuilder
        )
        {{
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($""{{nameof({containingTypes}{config.TypeName})}} must be initialized."")
                .Must(o => o.IsValid())
                .WithMessage((_, p) => p.GetValidationErrorMessage());
        }}"
                : string.Empty;

        return $@"
#nullable enable

using FluentValidation;

namespace {config.Namespace} {{
    /// <summary>
    ///     Extension methods for FluentValidation to validate <see cref=""{containingTypes}{config.TypeName}""/> value objects.
    /// </summary>
    public static class {config.TypeName}FluentValidationExtensions
    {{
        {mustBeInitialized.Trim()}
    
        {mustBeInitializedAndValid.Trim()}
    }}
}}
        ".Trim();
    }
}
