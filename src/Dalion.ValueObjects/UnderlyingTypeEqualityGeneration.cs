using System;

namespace Dalion.ValueObjects;

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
