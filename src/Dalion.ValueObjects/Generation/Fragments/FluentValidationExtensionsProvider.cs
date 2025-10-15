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
    public static class {config.TypeName}FluentValidationExtensions
    {{
        {mustBeInitialized.Trim()}
    
        {mustBeInitializedAndValid.Trim()}
    }}
}}
        ".Trim();
    }
}
