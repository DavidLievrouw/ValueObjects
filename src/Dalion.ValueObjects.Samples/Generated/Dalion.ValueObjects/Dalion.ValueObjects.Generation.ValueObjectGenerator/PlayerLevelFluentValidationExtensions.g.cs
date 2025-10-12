
        #nullable enable

        using FluentValidation;

        namespace Dalion.ValueObjects.Samples {
            
public static class PlayerLevelFluentValidationExtensions
{
    public static FluentValidation.IRuleBuilderOptions<T, PlayerLevel> MustBeInitialized<T>(
        this FluentValidation.IRuleBuilderInitial<T, PlayerLevel> ruleBuilder
    )
    {
        return ruleBuilder
            .Cascade(FluentValidation.CascadeMode.Stop)
            .Must(o => o.IsInitialized())
            .WithMessage($"{nameof(PlayerLevel)} must be initialized.");
    }

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
        