#nullable enable

using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    public static class BirthdayFluentValidationExtensions
    {
        public static FluentValidation.IRuleBuilderOptions<T, Birthday> MustBeInitialized<T>(
            this FluentValidation.IRuleBuilderInitial<T, Birthday> ruleBuilder
        )
        {
            return ruleBuilder
                .Cascade(FluentValidation.CascadeMode.Stop)
                .Must(o => o.IsInitialized())
                .WithMessage($"{nameof(Birthday)} must be initialized.");
        }
    
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