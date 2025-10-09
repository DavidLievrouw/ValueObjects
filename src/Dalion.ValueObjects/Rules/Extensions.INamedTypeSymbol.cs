using System.Linq;
using Dalion.ValueObjects.Generation;
using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Rules;

internal static class Extensions
{
    public static bool IsValueObjectTarget(this INamedTypeSymbol? symbol)
    {
        return symbol is not null && symbol.TryGetValueObjectAttributes().Any();
    }
}
