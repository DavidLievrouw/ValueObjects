using System.Text.RegularExpressions;

namespace Dalion.ValueObjects.Samples;

/// <summary>
///     A secure password.
/// </summary>
[ValueObject<string>(
    fromPrimitiveCasting: CastOperator.None,
    toPrimitiveCasting: CastOperator.Explicit,
    comparison: ComparisonGeneration.Omit,
    stringCaseSensitivity: StringCaseSensitivity.CaseSensitive
)]
public readonly partial record struct Password
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$";

    private static Validation Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Validation.Invalid(
                $"{nameof(Password)} cannot be null, empty, or whitespace."
            );
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