#nullable enable

using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    public static class CelsiusFluentValidationExtensions
    {
        public static FluentValidation.IRuleBuilderOptions<T, Celsius> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, Celsius> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(Celsius)} must be initialized.");
        }
    
        public static FluentValidation.IRuleBuilderOptions<T, Celsius> MustBeInitializedAndValid<T>(
            this FluentValidation.IRuleBuilderInitial<T, Celsius> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(Celsius)} must be initialized.")
                .Must(o => o.IsValid())
                .WithMessage((_, p) => p.GetValidationErrorMessage());
        }
    }
}