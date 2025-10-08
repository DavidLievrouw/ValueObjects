namespace Dalion.ValueObjects;

/// <summary>
///     The generation of comparison code for a Value Object.
/// </summary>
public enum ComparisonGeneration
{
    /// <summary>
    ///     Omits the IComparable interface and implementation. Useful for opaque types such as tokens or IDs where comparison
    ///     doesn't make sense.
    /// </summary>
    Omit,

    /// <summary>
    ///     Uses the default IComparable from the underlying type.
    /// </summary>
    UseUnderlying,
}
