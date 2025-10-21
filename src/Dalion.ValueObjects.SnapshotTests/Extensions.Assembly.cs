using System.Reflection;

namespace Dalion.ValueObjects.SnapshotTests;

internal static partial class Extensions
{
    public static string? GetEmbeddedResourceString(this Assembly assembly, string resourceName)
    {
        var fullResourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceName));

        if (fullResourceName == null)
        {
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}