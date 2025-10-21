using Microsoft.CodeAnalysis;

namespace Dalion.ValueObjects.Generation.Fragments;

internal class UnderlyingTypeCreationExtensionsProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        if (
            config.UnderlyingTypeCreationMethodGeneration
            == UnderlyingTypeCreationMethodGeneration.Omit
        )
        {
            return null;
        }

        var containingTypeNames = target.SymbolInformation.GetContainingTypeNames();
        var containingTypes =
            containingTypeNames == string.Empty ? string.Empty : containingTypeNames + ".";

        var creationMethod = config.UnderlyingType.SpecialType == SpecialType.System_String
            ? CreateForString(config, containingTypes).Trim()
            : CreateForValueType(config, containingTypes).Trim();

        return $@"
#nullable enable

using System;

namespace {config.Namespace} {{
    /// <summary>
    ///     Extension methods to create <see cref=""{containingTypes}{config.TypeName}""/> value objects.
    /// </summary>
    public static class {config.TypeName}UnderlyingTypeCreationExtensions
    {{
        {creationMethod}
    }}
}}
        ".Trim();
    }

    private static string CreateForString(AttributeConfiguration config, string containingTypes)
    {
        return $@"
        /// <summary>
        ///     Creates a new <see cref=""{containingTypes}{config.TypeName}""/> from the given <see cref=""{config.UnderlyingTypeName}""/>.
        /// </summary>
        /// <param name=""value"">The value to create the value object from.</param>
        /// <returns>A new <see cref=""{containingTypes}{config.TypeName}""/>.</returns>
        public static {config.Namespace}.{containingTypes}{config.TypeName} {config.TypeName}(this {config.UnderlyingTypeName}? value)
        {{
            return {config.Namespace}.{containingTypes}{config.TypeName}.From(value);
        }}";
    }

    private static string CreateForValueType(AttributeConfiguration config, string containingTypes)
    {
        return $@"
        /// <summary>
        ///     Creates a new <see cref=""{containingTypes}{config.TypeName}""/> from the given <see cref=""{config.UnderlyingTypeName}""/>.
        /// </summary>
        /// <param name=""value"">The value to create the value object from.</param>
        /// <returns>A new <see cref=""{containingTypes}{config.TypeName}""/>.</returns>
        public static {config.Namespace}.{containingTypes}{config.TypeName} {config.TypeName}(this {config.UnderlyingTypeName} value)
        {{
            return {config.Namespace}.{containingTypes}{config.TypeName}.From(value);
        }}";
    }
}