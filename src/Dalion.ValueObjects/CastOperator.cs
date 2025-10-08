namespace Dalion.ValueObjects
{
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
}
