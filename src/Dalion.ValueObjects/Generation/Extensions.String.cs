using Microsoft.CodeAnalysis.CSharp;

namespace Dalion.ValueObjects.Generation;

internal static partial class Extensions
{
    public static string EscapeKeywordsIfRequired(this string name)
    {
        var match =
            SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None
            || SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None;

        return match ? "@" + name : name;
    }
}
