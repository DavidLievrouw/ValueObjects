namespace Dalion.ValueObjects.Generation.Fragments;

internal class ValidationMembersProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return @"
        /// <summary>
        ///     Indicates whether this value object is valid.
        /// </summary>
        /// <returns><see langword=""true"" /> if this value object is valid; otherwise, <see langword=""false"" />.</returns>
        public bool IsValid() => _validation.IsSuccess;

        /// <summary>
        ///     Gets the validation error message if this value object is not valid.
        /// </summary>
        /// <returns>The validation error message if this value object is not valid; otherwise, <see langword=""null"" />.</returns>
        public string? GetValidationErrorMessage() => _validation.IsSuccess ? null : _validation.ErrorMessage;
".Trim();
    }
}