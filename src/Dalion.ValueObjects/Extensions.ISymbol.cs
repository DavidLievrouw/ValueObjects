using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects;

internal static partial class Extensions
{
    public static string EscapedFullName(this ISymbol symbol)
    {
        var prefix = FullNamespace(symbol);
        var suffix = "";

        if (symbol is INamedTypeSymbol nts)
        {
            if (nts.Arity > 0)
            {
                suffix =
                    $"<{string.Join(", ", nts.TypeArguments.Select(GetEscapedFullNameForTypeArgument))}>";
            }
        }

        if (prefix != string.Empty)
        {
            return $"{prefix}.{EscapeKeywordsIfRequired(symbol.Name)}{suffix}";
        }

        return EscapeKeywordsIfRequired(symbol.Name) + suffix;

        static string? GetEscapedFullNameForTypeArgument(ITypeSymbol a)
        {
            if (!a.CanBeReferencedByName)
            {
                return null;
            }
            if (a is not ITypeSymbol nts)
            {
                return null;
            }
            return EscapedFullName(nts);
        }
    }

    public static string FullNamespace(this ISymbol symbol)
    {
        var parts = new Stack<string>();
        INamespaceSymbol? iterator = symbol as INamespaceSymbol ?? symbol.ContainingNamespace;

        while (iterator is not null)
        {
            if (!string.IsNullOrEmpty(iterator.Name))
            {
                parts.Push(EscapeKeywordsIfRequired(iterator.Name));
            }

            iterator = iterator.ContainingNamespace;
        }

        return string.Join(".", parts);
    }
}
