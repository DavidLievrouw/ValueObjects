
        #nullable enable

        namespace Dalion.ValueObjects.Samples {
            public static class CelsiusUnderlyingTypeCreationExtensions
            {
                
                 public static Dalion.ValueObjects.Samples.Celsius Celsius(this System.Decimal value)
                 {
                     return Dalion.ValueObjects.Samples.Celsius.From(value);
                 }
            }
        }
        