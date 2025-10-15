namespace Dalion.ValueObjects.Generation.Fragments;

internal class ValidationMembersProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return @"
        public bool IsValid() => _validation.IsSuccess;
        public string? GetValidationErrorMessage() => _validation.IsSuccess ? null : _validation.ErrorMessage;
".Trim();
    }
}