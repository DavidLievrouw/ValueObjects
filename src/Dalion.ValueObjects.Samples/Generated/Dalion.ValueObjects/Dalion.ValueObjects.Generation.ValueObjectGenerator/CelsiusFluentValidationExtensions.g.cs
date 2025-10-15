#nullable enable

using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods for FluentValidation to validate <see cref="Celsius"/> value objects.
    /// </summary>
    public static class CelsiusFluentValidationExtensions
    {
        /// <summary>
        ///     Validates that the value object is initialized.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, Celsius> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, Celsius> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(Celsius)} must be initialized.");
        }
    
        /// <summary>
        ///     Validates that the value object is initialized and valid.
        /// </summary>
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