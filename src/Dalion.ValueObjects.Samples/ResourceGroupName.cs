using System.Text.RegularExpressions;

namespace Dalion.ValueObjects.Samples;

/// <summary>
///     The name of an Azure Resource Group.
/// </summary>
[ValueObject<string>(
    fromPrimitiveCasting: CastOperator.Explicit,
    toPrimitiveCasting: CastOperator.Implicit,
    comparison: ComparisonGeneration.UseUnderlying,
    stringCaseSensitivity: StringCaseSensitivity.CaseInsensitive
)]
public readonly partial record struct ResourceGroupName
{
    private const string ResourceGroupNamePattern = "^[A-Za-z0-9](?:[A-Za-z0-9_-]{1,61}[A-Za-z0-9])$";
    
    private static Validation Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Validation.Invalid(
                $"{nameof(ResourceGroupName)} cannot be null, empty, or whitespace."
            );
        }
        
        if (!ValidResourceGroupName().IsMatch(input))
        {
            return Validation.Invalid(
                $"{nameof(ResourceGroupName)} '{input}' is not valid. It must match the regex '{ResourceGroupNamePattern}'."
            );
        }
        
        return Validation.Ok;
    }
    
    [GeneratedRegex(ResourceGroupNamePattern)]
    private static partial Regex ValidResourceGroupName();
}