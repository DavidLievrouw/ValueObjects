using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class EqualityOperatorsUnderlyingTypeProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (
            (
                config.UnderlyingTypeEqualityGeneration
                & UnderlyingTypeEqualityGeneration.GenerateOperators
            ) != UnderlyingTypeEqualityGeneration.GenerateOperators
        )
        {
            return null;
        }

        return config.UnderlyingType.SpecialType == SpecialType.System_String
            ? GetForString(config).Trim()
            : GetForValueType(config).Trim();
    }

    private static string GetForValueType(AttributeConfiguration config)
    {
        return $@"
        /// <summary>
        ///     The equality operator for this type and the underlying type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.TypeName} left, {config.UnderlyingTypeName} right) => left.Value.Equals(right);

        /// <summary>
        ///     The equality operator for the underlying type and this type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.UnderlyingTypeName} left, {config.TypeName} right) => right.Value.Equals(left);

        /// <summary>
        ///     The inequality operator for this type and the underlying type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.TypeName} left, {config.UnderlyingTypeName} right) => !(left == right);

        /// <summary>
        ///     The inequality operator for the underlying type and this type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.UnderlyingTypeName} left, {config.TypeName} right) => !(left == right);";
    }

    private static string GetForString(AttributeConfiguration config)
    {
        return $@"
        /// <summary>
        ///     The equality operator for this type and the underlying type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.TypeName} left, {config.UnderlyingTypeName}? right) => left.Value.Equals(right);

        /// <summary>
        ///     The equality operator for the underlying type and this type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator ==({config.UnderlyingTypeName}? left, {config.TypeName} right) => right.Value.Equals(left);

        /// <summary>
        ///     The inequality operator for this type and the underlying type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.TypeName} left, {config.UnderlyingTypeName}? right) => !(left == right);

        /// <summary>
        ///     The inequality operator for the underlying type and this type.
        /// </summary>
        /// <returns><see langword=""true"" /> if the specified items are considered not to be equal; otherwise, <see langword=""false"" />.</returns>
        public static bool operator !=({config.UnderlyingTypeName}? left, {config.TypeName} right) => !(left == right);";
    }
}
