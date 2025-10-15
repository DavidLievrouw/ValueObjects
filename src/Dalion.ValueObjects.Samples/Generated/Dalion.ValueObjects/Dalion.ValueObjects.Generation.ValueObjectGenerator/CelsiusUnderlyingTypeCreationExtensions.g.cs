#nullable enable

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods to create <see cref="Celsius"/> value objects.
    /// </summary>
    public static class CelsiusUnderlyingTypeCreationExtensions
    {
        /// <summary>
        ///     Creates a new <see cref="Celsius"/> from the given <see cref="System.Decimal"/>.
        /// </summary>
        /// <param name="value">The value to create the value object from.</param>
        /// <returns>A new <see cref="Celsius"/>.</returns>
        public static Dalion.ValueObjects.Samples.Celsius Celsius(this System.Decimal value)
        {
            return Dalion.ValueObjects.Samples.Celsius.From(value);
        }
    }
}