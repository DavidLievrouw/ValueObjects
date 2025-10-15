#nullable enable

using FluentValidation;

namespace Dalion.ValueObjects.Samples {
    public static class LegacyPhoneNumberFluentValidationExtensions
    {
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