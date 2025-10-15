using System.Text;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class ConversionProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        var builder = new StringBuilder();

        if (config.ToUnderlyingTypeCasting != CastOperator.None)
        {
            builder.AppendLine(
                $@"
        /// <summary>
        ///     An implicit conversion from <see cref=""{config.TypeName}"" /> to <see cref=""{config.UnderlyingTypeName}"" />.
        /// </summary>
        /// <param name=""id"">The value to convert.</param>
        /// <returns>The {config.UnderlyingTypeName} representation of the value object.</returns>
        public static {config.ToUnderlyingTypeCasting.ToString().ToLower()} operator {config.UnderlyingTypeName}({config.TypeName} id)
        {{
            return id.Value;
        }}"
            );
        }

        if (config.FromUnderlyingTypeCasting != CastOperator.None)
        {
            builder.AppendLine(
                $@"
        /// <summary>
        ///     An explicit conversion from <see cref=""{config.UnderlyingTypeName}"" /> to <see cref=""{config.TypeName}"" />.
        /// </summary>
        /// <param name=""value"">The value to convert.</param>
        /// <returns>The <see cref=""{config.TypeName}"" /> instance created from the input value.</returns>
        public static {config.FromUnderlyingTypeCasting.ToString().ToLower()} operator {config.TypeName}({config.UnderlyingTypeName} value)
        {{
            return {config.TypeName}.From(value);
        }}"
            );
        }

        var code = builder.ToString().Trim();
        return string.IsNullOrEmpty(code) ? null : code;
    }
}
