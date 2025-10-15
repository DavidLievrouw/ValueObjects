#nullable enable

namespace Dalion.ValueObjects.Samples {
    /// <summary>
    ///     Extension methods to create <see cref="LegacyPhoneNumber"/> value objects.
    /// </summary>
    public static class LegacyPhoneNumberUnderlyingTypeCreationExtensions
    {
        /// <summary>
        ///     Creates a new <see cref="LegacyPhoneNumber"/> from the given <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value to create the value object from.</param>
        /// <returns>A new <see cref="LegacyPhoneNumber"/>.</returns>
        public static Dalion.ValueObjects.Samples.LegacyPhoneNumber LegacyPhoneNumber(this System.String? value)
        {
            return Dalion.ValueObjects.Samples.LegacyPhoneNumber.From(value);
        }
    }
}