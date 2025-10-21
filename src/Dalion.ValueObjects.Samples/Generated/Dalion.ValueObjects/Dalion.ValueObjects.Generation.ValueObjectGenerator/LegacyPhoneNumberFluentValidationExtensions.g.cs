#nullable enable

using System;
using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods for FluentValidation to validate <see cref="LegacyPhoneNumber"/> value objects.
    /// </summary>
    public static class LegacyPhoneNumberFluentValidationExtensions
    {
        /// <summary>
        ///     Validates that the value object is initialized.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, LegacyPhoneNumber> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, LegacyPhoneNumber> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(LegacyPhoneNumber)} must be initialized.");
        }
    
        
    }
}