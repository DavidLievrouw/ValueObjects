#nullable enable

using System;
using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods for FluentValidation to validate <see cref="Birthday"/> value objects.
    /// </summary>
    public static class BirthdayFluentValidationExtensions
    {
        /// <summary>
        ///     Validates that the value object is initialized.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, Birthday> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, Birthday> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(Birthday)} must be initialized.");
        }
    
        /// <summary>
        ///     Validates that the value object is initialized and valid.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, Birthday> MustBeInitializedAndValid<T>(
            this FluentValidation.IRuleBuilderInitial<T, Birthday> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(Birthday)} must be initialized.")
                .Must(o => o.IsValid())
                .WithMessage((_, p) => p.GetValidationErrorMessage());
        }
    }
}