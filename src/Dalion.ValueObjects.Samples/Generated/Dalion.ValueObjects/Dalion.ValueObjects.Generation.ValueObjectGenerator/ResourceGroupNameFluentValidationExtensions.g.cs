
        #nullable enable

        using FluentValidation;

        namespace Dalion.ValueObjects.Samples {
            
public static class ResourceGroupNameFluentValidationExtensions
{
    public static FluentValidation.IRuleBuilderOptions<T, ResourceGroupName> MustBeInitialized<T>(
        this FluentValidation.IRuleBuilderInitial<T, ResourceGroupName> ruleBuilder
    )
    {
        return ruleBuilder
            .Cascade(FluentValidation.CascadeMode.Stop)
            .Must(o => o.IsInitialized())
            .WithMessage($"{nameof(ResourceGroupName)} must be initialized.");
    }

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
        