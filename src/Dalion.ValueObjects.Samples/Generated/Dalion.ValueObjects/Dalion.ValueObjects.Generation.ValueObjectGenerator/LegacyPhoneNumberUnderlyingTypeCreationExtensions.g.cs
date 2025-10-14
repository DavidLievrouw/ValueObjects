
        #nullable enable

        namespace Dalion.ValueObjects.Samples {
            public static class LegacyPhoneNumberUnderlyingTypeCreationExtensions
            {
                
                 public static Dalion.ValueObjects.Samples.LegacyPhoneNumber LegacyPhoneNumber(this System.String? value)
                 {
                     return Dalion.ValueObjects.Samples.LegacyPhoneNumber.From(value);
                 }
            }
        }
        