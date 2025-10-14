using System;

namespace Dalion.ValueObjects.Samples
{
    // ReSharper disable once RedundantNullableDirective
#nullable enable
    /// <inheritdoc/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ValueObjectAttribute<T> : ValueObjectAttribute
    {
        /// <summary>
        ///     Configures aspects of this individual value object.
        /// </summary>
        /// <param name = "comparison">
        ///     Species which comparison code is generated—defaults to
        ///     <see cref = "ComparisonGeneration.UseUnderlying"/> which hoists any IComparable implementations from the underlying
        ///     type.
        /// </param>
        /// <param name = "toUnderlyingTypeCasting">
        ///     Controls how cast operators are generated for casting from the Value Object to the underlying type.
        ///     Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't
        ///     recommended.
        /// </param>
        /// <param name = "fromUnderlyingTypeCasting">
        ///     Controls how cast operators are generated for casting from the underlying type to the Value Object.
        ///     Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't
        ///     recommended.
        /// </param>
        /// <param name = "stringCaseSensitivity">
        ///     When using a <see cref = "string "/> as a backing value, controls whether comparisons are case-sensitive.
        ///     Defaults to <see cref = "StringCaseSensitivity.CaseSensitive"/>.
        /// </param>
        /// <param name = "underlyingTypeEqualityGeneration">
        ///     Specifies whether to generate underlying type comparison operators, allowing this type to be compared for equality
        ///     to the
        ///     underlying type.
        ///     Defaults to <see cref = "UnderlyingTypeEqualityGeneration.Omit"/>
        /// </param>
        /// <param name = "fluentValidationExtensionsGeneration">
        ///     Specifies whether to generate FluentValidation extension methods for this value object.
        ///     Defaults to <see cref = "FluentValidationExtensionsGeneration.Omit"/>.
        /// </param>
        /// <param name = "parsableGeneration">
        ///     Defines if IParsable implementation and related methods are generated. Defaults to
        ///     <see cref = "ParsableGeneration.Generate"/>.
        /// </param>
        /// <param name = "underlyingTypeCreationMethodGeneration">
        ///     Defines if an extension method for value object creation is generated for the underlying type.
        ///     Defaults to <see cref = "UnderlyingTypeCreationMethodGeneration.Omit"/>.
        /// </param>
        /// <param name = "emptyValueName">
        ///     The name of the static property representing an empty value object, if applicable.
        ///     Defaults to "Empty".
        /// </param>
        public ValueObjectAttribute(ComparisonGeneration comparison = DefaultComparison, CastOperator toUnderlyingTypeCasting = DefaultToUnderlyingTypeCasting, CastOperator fromUnderlyingTypeCasting = DefaultFromUnderlyingTypeCasting, StringCaseSensitivity stringCaseSensitivity = DefaultStringCaseSensitivity, UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration = DefaultUnderlyingTypeEqualityGeneration, FluentValidationExtensionsGeneration fluentValidationExtensionsGeneration = DefaultFluentValidationExtensionsGeneration, ParsableGeneration parsableGeneration = DefaultParsableGeneration, UnderlyingTypeCreationMethodGeneration underlyingTypeCreationMethodGeneration = DefaultUnderlyingTypeCreationMethodGeneration, string emptyValueName = DefaultEmptyValueName) : base(typeof(T), comparison, toUnderlyingTypeCasting, fromUnderlyingTypeCasting, stringCaseSensitivity, underlyingTypeEqualityGeneration, fluentValidationExtensionsGeneration, parsableGeneration, underlyingTypeCreationMethodGeneration, emptyValueName)
        {
        }
    }

    /// <summary>
    ///     Marks a type as a Value Object. The type that this is applied to should be partial so that the
    ///     source generator can augment it with equality, creation barriers, and any conversions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ValueObjectAttribute : Attribute
    {
        internal const ComparisonGeneration DefaultComparison = ComparisonGeneration.UseUnderlying;
        internal const CastOperator DefaultToUnderlyingTypeCasting = CastOperator.None;
        internal const CastOperator DefaultFromUnderlyingTypeCasting = CastOperator.None;
        internal const StringCaseSensitivity DefaultStringCaseSensitivity = StringCaseSensitivity.CaseSensitive;
        internal const UnderlyingTypeEqualityGeneration DefaultUnderlyingTypeEqualityGeneration = UnderlyingTypeEqualityGeneration.Omit;
        internal const FluentValidationExtensionsGeneration DefaultFluentValidationExtensionsGeneration = FluentValidationExtensionsGeneration.Omit;
        internal const ParsableGeneration DefaultParsableGeneration = ParsableGeneration.Generate;
        internal const UnderlyingTypeCreationMethodGeneration DefaultUnderlyingTypeCreationMethodGeneration = UnderlyingTypeCreationMethodGeneration.Omit;
        internal const string DefaultEmptyValueName = "Empty";
        /// <summary>
        ///     Configures aspects of this individual value object.
        /// </summary>
        /// <param name = "underlyingType">The type of the underlying value that is being wrapped.</param>
        /// <param name = "comparison">
        ///     Species which comparison code is generated—defaults to
        ///     <see cref = "ComparisonGeneration.UseUnderlying"/> which hoists any IComparable implementations from the underlying
        ///     type.
        /// </param>
        /// <param name = "toUnderlyingTypeCasting">
        ///     Specifies the type of casting from wrapper to the underlying type - defaults to
        ///     <see cref = "CastOperator.Explicit"/>.
        /// </param>
        /// <param name = "fromUnderlyingTypeCasting">
        ///     Specifies the type of casting from the underlying type to wrapper - default to
        ///     <see cref = "CastOperator.Explicit"/>.
        /// </param>
        /// <param name = "stringCaseSensitivity">
        ///     When using a <see cref = "string "/> as a backing value, controls whether comparisons are case-sensitive.
        ///     Defaults to <see cref = "StringCaseSensitivity.CaseSensitive"/>.
        /// </param>
        /// <param name = "underlyingTypeEqualityGeneration">
        ///     Specifies whether to generate underlying value comparison operators, allowing this type to be compared for equality
        ///     to the
        ///     underlying type.
        ///     Defaults to <see cref = "UnderlyingTypeEqualityGeneration.Omit"/>
        /// </param>
        /// <param name = "fluentValidationExtensionsGeneration">
        ///     Specifies whether to generate FluentValidation extension methods for this value object.
        ///     Defaults to <see cref = "FluentValidationExtensionsGeneration.Omit"/>.
        /// </param>
        /// <param name = "parsableGeneration">
        ///     Defines if IParsable implementation and related methods are generated. Defaults to
        ///     <see cref = "ParsableGeneration.Generate"/>.
        /// </param>
        /// <param name = "underlyingTypeCreationMethodGeneration">
        ///     Defines if an extension method for value object creation is generated for the underlying type.
        ///     Defaults to <see cref = "UnderlyingTypeCreationMethodGeneration.Omit"/>.
        /// </param>
        /// <param name = "emptyValueName">
        ///     The name of the static property representing an empty value object, if applicable.
        ///     Defaults to "Empty".
        /// </param>
        public ValueObjectAttribute(Type? underlyingType = null !, ComparisonGeneration comparison = DefaultComparison, CastOperator toUnderlyingTypeCasting = DefaultToUnderlyingTypeCasting, CastOperator fromUnderlyingTypeCasting = DefaultFromUnderlyingTypeCasting, StringCaseSensitivity stringCaseSensitivity = DefaultStringCaseSensitivity, UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration = DefaultUnderlyingTypeEqualityGeneration, FluentValidationExtensionsGeneration fluentValidationExtensionsGeneration = DefaultFluentValidationExtensionsGeneration, ParsableGeneration parsableGeneration = DefaultParsableGeneration, UnderlyingTypeCreationMethodGeneration underlyingTypeCreationMethodGeneration = DefaultUnderlyingTypeCreationMethodGeneration, string emptyValueName = DefaultEmptyValueName)
        {
        }
    }

    /// <summary>
    ///     The type of cast operator to generate.
    /// </summary>
    public enum CastOperator
    {
        /// <summary>
        ///     No cast operators are generated.
        /// </summary>
        None = 0,
        /// <summary>
        ///     Explicit cast operators are generated.
        /// </summary>
        Explicit = 1,
        /// <summary>
        ///     Implicit cast operators are generated.
        /// </summary>
        Implicit = 2,
    }

    /// <summary>
    ///     The generation of comparison code for a Value Object.
    /// </summary>
    public enum ComparisonGeneration
    {
        /// <summary>
        ///     Omits the IComparable interface and implementation. Useful for opaque types such as tokens or IDs where comparison
        ///     doesn't make sense.
        /// </summary>
        Omit = 0,
        /// <summary>
        ///     Uses the default IComparable from the underlying type.
        /// </summary>
        UseUnderlying = 1,
    }

    /// <summary>
    ///     Specifies whether to generate string comparers for a value object based on a string primitive type.
    /// </summary>
    public enum StringCaseSensitivity
    {
        /// <summary>
        ///     The backing string values are case-sensitive.
        /// </summary>
        CaseSensitive = 0,
        /// <summary>
        ///     The backing string values are case-sensitive.
        /// </summary>
        CaseInsensitive = 1,
    }

    /// <summary>
    ///     Defines if equality operators to the underlying values are generated.
    /// </summary>
    [Flags]
    public enum UnderlyingTypeEqualityGeneration
    {
        /// <summary>
        ///     Do not generate.
        /// </summary>
        Omit = 0,
        /// <summary>
        ///     Generate equals operators for the underlying type.
        /// </summary>
        GenerateOperators = 1 << 0,
        /// <summary>
        ///     Generate equals methods for the underlying type.
        /// </summary>
        GenerateMethods = 1 << 1,
        /// <summary>
        ///     Generate both operators and methods.
        /// </summary>
        GenerateOperatorsAndMethods = GenerateOperators | GenerateMethods,
    }

    /// <summary>
    ///     Defines if FluentValidation extension methods are generated.
    /// </summary>
    [Flags]
    public enum FluentValidationExtensionsGeneration
    {
        /// <summary>
        ///     Do not generate.
        /// </summary>
        Omit = 0,
        /// <summary>
        ///     Generate MustBeInitialized extension method.
        /// </summary>
        GenerateMustBeInitialized = 1 << 0,
        /// <summary>
        ///     Generate MustBeInitializedAndValid extension method.
        /// </summary>
        GenerateMustBeInitializedAndValid = 1 << 1,
        /// <summary>
        ///     Generate all methods.
        /// </summary>
        GenerateAll = GenerateMustBeInitialized | GenerateMustBeInitializedAndValid,
    }

    /// <summary>
    ///     Defines if IParsable implementation and related methods are generated.
    /// </summary>
    public enum ParsableGeneration
    {
        /// <summary>
        ///     Do not generate.
        /// </summary>
        Omit = 0,
        /// <summary>
        ///     Generate IParsable implementation and related methods.
        /// </summary>
        Generate = 1,
    }

    /// <summary>
    ///     Defines if an extension method for value object creation is generated for the underlying type.
    /// </summary>
    public enum UnderlyingTypeCreationMethodGeneration
    {
        /// <summary>
        ///     Do not generate.
        /// </summary>
        Omit = 0,
        /// <summary>
        ///     Generate Create method.
        /// </summary>
        Generate = 1,
    }
}