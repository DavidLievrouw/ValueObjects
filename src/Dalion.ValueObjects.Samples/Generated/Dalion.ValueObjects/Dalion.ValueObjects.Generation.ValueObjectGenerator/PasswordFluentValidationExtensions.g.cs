
        #nullable enable

        using FluentValidation;

        namespace Dalion.ValueObjects.Samples {
            
public static class PasswordFluentValidationExtensions
{
    public static FluentValidation.IRuleBuilderOptions<T, Password> MustBeInitialized<T>(
        this FluentValidation.IRuleBuilderInitial<T, Password> ruleBuilder
    )
    {
        return ruleBuilder
            .Cascade(FluentValidation.CascadeMode.Stop)
            .Must(o => o.IsInitialized())
            .WithMessage($"{nameof(Password)} must be initialized.");
    }

    public static FluentValidation.IRuleBuilderOptions<T, Password> MustBeInitializedAndValid<T>(
        this FluentValidation.IRuleBuilderInitial<T, Password> ruleBuilder
    )
    {
        return ruleBuilder
            .Cascade(FluentValidation.CascadeMode.Stop)
            .Must(o => o.IsInitialized())
            .WithMessage($"{nameof(Password)} must be initialized.")
            .Must(o => o.IsValid())
            .WithMessage((_, p) => p.GetValidationErrorMessage());
    }
}
        }
        