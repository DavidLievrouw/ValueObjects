using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Rules;

internal static class DiagnosticsCatalogue
{
    public static Diagnostic BuildDiagnostic(DiagnosticDescriptor descriptor, string name, Location location)
    {
        return Diagnostic.Create(descriptor, location, name);
    }
}