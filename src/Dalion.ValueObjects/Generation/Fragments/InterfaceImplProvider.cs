using System.Text;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class InterfaceImplProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        var interfaceDefsBuilder = new StringBuilder();
        
        interfaceDefsBuilder.Append($": IEquatable<{config.TypeName}>");

        if (target.IsFormattable())
        {
            interfaceDefsBuilder.Append(", IFormattable");
        }

        if (
            (
                config.UnderlyingTypeEqualityGeneration
                & UnderlyingTypeEqualityGeneration.GenerateMethods
            ) == UnderlyingTypeEqualityGeneration.GenerateMethods
        )
        {
            interfaceDefsBuilder.Append($", IEquatable<{config.UnderlyingTypeName}>");
        }

        if (config.ParsableGeneration == ParsableGeneration.Generate)
        {
            interfaceDefsBuilder.Append(
                $", ISpanParsable<{config.TypeName}>, IUtf8SpanParsable<{config.TypeName}>"
            );
        }

        if (config.Comparison != ComparisonGeneration.Omit)
        {
            interfaceDefsBuilder.Append($", IComparable<{config.TypeName}>");
            interfaceDefsBuilder.Append(", IComparable");
        }

        return interfaceDefsBuilder.ToString();
    }
}
