using System.Text;
using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class EqualityOperatorsUnderlyingTypeProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        var builder = new StringBuilder();

        if (!target.SymbolInformation.IsRecord) {
            builder.AppendLine(GetForValueObjectType(config));
        }

        if (
            (
                config.UnderlyingTypeEqualityGeneration
                & UnderlyingTypeEqualityGeneration.GenerateOperators
            ) != UnderlyingTypeEqualityGeneration.GenerateOperators
        )
        {
            return null;
        }

        builder.AppendLine(
            config.UnderlyingType.SpecialType == SpecialType.System_String
                ? GetForString(config)
                : GetForValueType(config)
        );

        return builder.ToString().Trim();
    }

    private static string GetForValueObjectType(AttributeConfiguration config)
    {
        return $@"
        /// <summary>
        ///     The equality operator for <see cref=""{config.TypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.TypeName} left, {config.TypeName} right) => left.Equals(right);

        /// <summary>
        ///     The inequality operator for <see cref=""{config.TypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.TypeName} left, {config.TypeName} right) => !(left.Equals(right));
";
    }

    private static string GetForValueType(AttributeConfiguration config)
    {
        return $@"
        /// <summary>
        ///     The equality operator for <see cref=""{config.TypeName}"" /> and <see cref=""{config.UnderlyingTypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.TypeName} left, {config.UnderlyingTypeName} right) => left.Value.Equals(right);

        /// <summary>
        ///     The equality operator for <see cref=""{config.UnderlyingTypeName}"" /> and <see cref=""{config.TypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.UnderlyingTypeName} left, {config.TypeName} right) => right.Value.Equals(left);

        /// <summary>
        ///     The inequality operator for <see cref=""{config.TypeName}"" /> and <see cref=""{config.UnderlyingTypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.TypeName} left, {config.UnderlyingTypeName} right) => !(left == right);

        /// <summary>
        ///     The inequality operator for <see cref=""{config.UnderlyingTypeName}"" /> and <see cref=""{config.TypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.UnderlyingTypeName} left, {config.TypeName} right) => !(left == right);";
    }

    private static string GetForString(AttributeConfiguration config)
    {
        return $@"
        /// <summary>
        ///     The equality operator for <see cref=""{config.TypeName}"" /> and <see cref=""{config.UnderlyingTypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.TypeName} left, {config.UnderlyingTypeName}? right) => left.Value.Equals(right);

        /// <summary>
        ///     The equality operator for <see cref=""{config.UnderlyingTypeName}"" /> and <see cref=""{config.TypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.UnderlyingTypeName}? left, {config.TypeName} right) => right.Value.Equals(left);

        /// <summary>
        ///     The inequality operator for <see cref=""{config.TypeName}"" /> and <see cref=""{config.UnderlyingTypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.TypeName} left, {config.UnderlyingTypeName}? right) => !(left == right);

        /// <summary>
        ///     The inequality operator for <see cref=""{config.UnderlyingTypeName}"" /> and <see cref=""{config.TypeName}"" />.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.UnderlyingTypeName}? left, {config.TypeName} right) => !(left == right);";
    }
}
