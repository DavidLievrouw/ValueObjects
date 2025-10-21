#nullable enable

using System;
using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods for FluentValidation to validate <see cref="PlayerLevel"/> value objects.
    /// </summary>
    public static class PlayerLevelFluentValidationExtensions
    {
        /// <summary>
        ///     Validates that the value object is initialized.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, PlayerLevel> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, PlayerLevel> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(PlayerLevel)} must be initialized.");
        }
    
        /// <summary>
        ///     Validates that the value object is initialized and valid.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, PlayerLevel> MustBeInitializedAndValid<T>(
            this FluentValidation.IRuleBuilderInitial<T, PlayerLevel> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(PlayerLevel)} must be initialized.")
                .Must(o => o.IsValid())
                .WithMessage((_, p) => p.GetValidationErrorMessage());
        }
    }
}