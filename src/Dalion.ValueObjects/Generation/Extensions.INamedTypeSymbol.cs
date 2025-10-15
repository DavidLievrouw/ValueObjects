using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation;

internal static partial class Extensions
{
    public static IEnumerable<AttributeData> TryGetValueObjectAttributes(
        this INamedTypeSymbol symbolInformation
    )
    {
        var attrs = symbolInformation.GetAttributes();

        return attrs.Where(a =>
        {
            var ns = a.AttributeClass?.ContainingNamespace?.ToDisplayString();
            var hasNamespace = !string.IsNullOrEmpty(ns) && ns != "<global namespace>";

            var fullName = hasNamespace
                ? ns + "." + nameof(ValueObjectAttribute)
                : nameof(ValueObjectAttribute);

            if (!hasNamespace)
            {
                // Part of this compilation run, there is no metadata yet (so no base class)
                // We need to add the generic type argument manually
                fullName = fullName.Substring(0, fullName.Length - "Attribute".Length);
                var typeArg = a.AttributeClass?.GetTypeArguments()?.FirstOrDefault();
                if (typeArg != null)
                {
                    fullName += $"<{typeArg}>";
                }
            }

            return a.AttributeClass?.EscapedFullName() == fullName
                || a.AttributeClass?.BaseType?.EscapedFullName() == fullName
                || a.AttributeClass?.BaseType?.BaseType?.EscapedFullName() == fullName;
        });
    }

    public static string GetContainingTypeNames(this INamedTypeSymbol symbol)
    {
        var types = new Stack<string>();
        var current = symbol.ContainingType;
        while (current != null)
        {
            types.Push(current.Name);
            current = current.ContainingType;
        }

        return string.Join(".", types);
    }

    public static string GetContainingTypesDeclarations(this INamedTypeSymbol symbol)
    {
        var types = new Stack<string>();
        var current = symbol.ContainingType;
        while (current != null)
        {
            var kind = current.TypeKind switch
            {
                TypeKind.Class => "class",
                TypeKind.Struct => "struct",
                TypeKind.Interface => "interface",
                TypeKind.Enum => "enum",
                _ => "class",
            };
            var recordModifier = current.IsRecord ? "record " : "";
            types.Push($"public partial {recordModifier}{kind} {current.Name} {{");
            current = current.ContainingType;
        }

        return string.Join("\n", types);
    }

    public static string GetClosingBraces(this INamedTypeSymbol symbol)
    {
        var count = 0;
        var current = symbol.ContainingType;
        while (current != null)
        {
            count++;
            current = current.ContainingType;
        }

        return new string('}', count);
    }
}
