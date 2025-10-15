#nullable enable

using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    public static class TenantIdFluentValidationExtensions
    {
        
    
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