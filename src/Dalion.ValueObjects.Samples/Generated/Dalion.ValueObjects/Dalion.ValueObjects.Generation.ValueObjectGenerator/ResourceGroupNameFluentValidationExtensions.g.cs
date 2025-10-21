#nullable enable

using System;
using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods for FluentValidation to validate <see cref="ResourceGroupName"/> value objects.
    /// </summary>
    public static class ResourceGroupNameFluentValidationExtensions
    {
        /// <summary>
        ///     Validates that the value object is initialized.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, ResourceGroupName> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, ResourceGroupName> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(ResourceGroupName)} must be initialized.");
        }
    
        /// <summary>
        ///     Validates that the value object is initialized and valid.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, ResourceGroupName> MustBeInitializedAndValid<T>(
            this FluentValidation.IRuleBuilderInitial<T, ResourceGroupName> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(ResourceGroupName)} must be initialized.")
                .Must(o => o.IsValid())
                .WithMessage((_, p) => p.GetValidationErrorMessage());
        }
    }
}