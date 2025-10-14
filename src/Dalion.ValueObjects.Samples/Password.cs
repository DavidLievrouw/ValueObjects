using System.Text.RegularExpressions;

namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A secure password.
/// </summary>
[ValueObject<string>(
    fromUnderlyingTypeCasting: CastOperator.None,
    toUnderlyingTypeCasting: CastOperator.Explicit,
    comparison: ComparisonGeneration.Omit,
    stringCaseSensitivity: StringCaseSensitivity.CaseSensitive,
    underlyingTypeEqualityGeneration: UnderlyingTypeEqualityGeneration.Omit,
    parsableGeneration: ParsableGeneration.Omit
)]
public readonly partial record struct Password
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$";

    private static Validation Validate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Validation.Invalid($"{nameof(Password)} cannot be null, empty, or whitespace.");
        }

        if (!ValidPassword().IsMatch(input))
        {
            return Validation.Invalid(
                $"{nameof(Password)} '{input}' is not valid. It must match the regex '{PasswordPattern}'."
            );
        }

        return Validation.Ok;
    }

    [GeneratedRegex(PasswordPattern)]
    private static partial Regex ValidPassword();
}
