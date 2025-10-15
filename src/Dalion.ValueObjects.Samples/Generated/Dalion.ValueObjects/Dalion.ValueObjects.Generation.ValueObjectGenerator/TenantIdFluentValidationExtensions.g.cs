#nullable enable

using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods for FluentValidation to validate <see cref="TenantId"/> value objects.
    /// </summary>
    public static class TenantIdFluentValidationExtensions
    {
        
    
        /// <summary>
        ///     Validates that the value object is initialized and valid.
        /// </summary>
        public static FluentValidation.IRuleBuilderOptions<T, TenantId> MustBeInitializedAndValid<T>(
            this FluentValidation.IRuleBuilderInitial<T, TenantId> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(TenantId)} must be initialized.")
                .Must(o => o.IsValid())
                .WithMessage((_, p) => p.GetValidationErrorMessage());
        }
    }
}