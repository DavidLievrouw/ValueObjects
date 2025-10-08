namespace Dalion.ValueObjects
{
    /// <summary>
    ///     Specifies whether to generate string comparers for a value object based on a string primitive type
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
}