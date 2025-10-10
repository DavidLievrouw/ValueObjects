using System.Globalization;
using System.Text.Json;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public partial class CelsiusTests
{
    public class Construction : CelsiusTests
    {
        // Does not compile, as intended, when the analyzer works correctly.
        /*[Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new Celsius();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }*/
    }

    public class From : CelsiusTests
    {
        [Fact]
        public void From_CreatesCelsiusWithValue()
        {
            var backingValue = 24.2M;
            
            var actual = Celsius.From(backingValue);
            
            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CanCreateZero()
        {
            var backingValue = 0m;
            
            var actual = Celsius.From(backingValue);
            
            Assert.Equal(backingValue, actual.Value);
            
            var zero = Celsius.Zero;
            Assert.True(actual.Equals(zero));
            Assert.True(actual == zero);
            Assert.False(actual != zero);
            Assert.Equal(actual.GetHashCode(), zero.GetHashCode());
            
            Assert.True(actual.IsInitialized());
        }

        [Theory]
        [InlineData(-273.16)] // lower than absolute zero
        [InlineData(-300)] // lower than absolute zero
        public void CannotCreateInvalidInstance(double invalid)
        {
            var dec = (decimal)invalid;
            Action act = () => Celsius.From(dec);

            Assert.Throws<InvalidOperationException>(act);
        }
    }

    public class TryFrom : CelsiusTests
    {
        [Fact]
        public void TryFrom_CreatesCelsiusWithValue()
        {
            var backingValue = 24.2m;

            var success = Celsius.TryFrom(backingValue, out var actual);

            Assert.True(success);
            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CanCreateZero()
        {
            var success = Celsius.TryFrom(0m, out var actual);

            Assert.True(success);
            
            var zero = Celsius.Zero;
            Assert.True(actual.Equals(zero));
            Assert.True(actual == zero);
            Assert.False(actual != zero);
            Assert.Equal(actual.GetHashCode(), zero.GetHashCode());
            
            Assert.True(actual.IsInitialized());
        }
        
        [Theory]
        [InlineData(-273.16)] // lower than absolute zero
        [InlineData(-300)] // lower than absolute zero
        public void CannotCreateInvalidInstance(double invalid)
        {
            var dec = (decimal)invalid;
            var success = Celsius.TryFrom(dec, out _);

            Assert.False(success);
        }
    }

    public class Value : CelsiusTests
    {
        [Fact]
        public void ReturnsUnderlyingValue()
        {
            var expected = 24.2m;

            var actual = Celsius.From(expected);

            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public void EmptyReturnsExpectedUnderlyingValue()
        {
            var actual = Celsius.Zero;

            Assert.Equal(0m, actual.Value);
        }
    }
    
    public class Comparison : CelsiusTests
    {
        [Fact]
        public void CompareTo_WorksAsExpected()
        {
            var a = Celsius.From(24.2m);
            var b = Celsius.From(25.5m);
            var a2 = Celsius.From(24.2m);

            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
            Assert.True(a.CompareTo(a2) == 0);
        }
    }
    
    public class Equality : CelsiusTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var backingValue = 24.2m;
            var first = Celsius.From(backingValue);
            var second = Celsius.From(first.Value);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var backingValue1 = 24.2m;
            var first = Celsius.From(backingValue1);
            var backingValue2 = -2.54m;
            var second = Celsius.From(backingValue2);

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToDefault()
        {
            Celsius first = default;
            Celsius second = default;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsNotEqualToZero()
        {
            Celsius first = default;
            var second = Celsius.Zero;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            var backingValue = 24.2m;
            var first = Celsius.From(backingValue);
            Celsius second = default;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotCelsius_AreNotEqual()
        {
            var backingValue = 24.2m;
            var first = Celsius.From(backingValue);
            var second = new object();

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void GivenOtherObjectIsAnotherValueType_AreNotEqual()
        {
            var backingValue = 24.2m;
            var first = Celsius.From(backingValue);
            var second = OtherCelsius.From(backingValue);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));
        }

        [Fact]
        public void HasEqualityMethodsForUnderlyingType()
        {
            var backingValue = 24.2m;
            var first = Celsius.From(backingValue);
            var second = backingValue;

            Assert.True(first.Equals(second));

            var third = -2.4m;
            Assert.False(first.Equals(third));
        }
        
        [Fact]
        public void HasEqualityOperatorsForUnderlyingType()
        {
            var first = Celsius.From(123m);
            var second = 123m;

            Assert.True(first == second);
            Assert.True(second == first);
            Assert.False(first != second);
            Assert.False(second != first);

            var third = -21.2m;
            Assert.False(first == third);
            Assert.False(third == first);
            Assert.True(first != third);
            Assert.True(third != first);
        }
    }

    public class IsInitialized : CelsiusTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);

            Assert.True(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsDefault_IsFalse()
        {
            Celsius sut = default;

            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsZero_IsTrue()
        {
            var sut = Celsius.Zero;

            Assert.True(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : CelsiusTests
    {
        [Fact]
        public void ReturnsValue()
        {
            var value = 24.2m;

            var actual = Celsius.From(value).ToString();

            Assert.Equal(value.ToString(CultureInfo.InvariantCulture), actual);
        }
    }

    public class ConversionOperatorsForUnderlyingType : CelsiusTests
    {
        [Fact]
        public void IsExplicitlyConvertibleToUnderlyingType()
        {
            var value = 24.2m;
            var obj = Celsius.From(value);

            var actual = (decimal)obj;

            Assert.Equal(value, actual);
        }

        [Fact]
        public void IsExplicitlyConvertibleFromUnderlyingType()
        {
            var value = 24.2m;
            var str = value;

            var actual = (Celsius)str;

            var expected = Celsius.From(value);
            Assert.Equal(expected, actual);
        }
    }

    public class Serialization : CelsiusTests
    {
        [Fact]
        public void WhenNonsense_ThrowsJsonException()
        {
            var nonsense = "\"nonsense\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Celsius>(nonsense));
        }

        [Fact]
        public void WhenEmptyString_ThrowsJsonException()
        {
            var nonsense = "\"\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Celsius>(nonsense));
        }

        [Fact]
        public void CanRoundTrip()
        {
            var backingValue = 24.2m;
            var original = Celsius.From(backingValue);

            var serialized = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<Celsius>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);

            var serialized = JsonSerializer.Serialize(sut);

            Assert.Equal("24.2", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            Celsius original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<Celsius>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripZero()
        {
            var original = Celsius.Zero;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("0", serialized);

            var deserialized = JsonSerializer.Deserialize<Celsius>(serialized);

            Assert.Equal(original, deserialized);
            Assert.True(deserialized.IsInitialized());
        }

        [Fact]
        public void SerializesUninitializedToNull()
        {
            var container = new Container { Id = "one", Data = default };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal(
                "{\"Id\":\"one\",\"Data\":null}",
                serialized
            );
        }

        [Fact]
        public void SerializesZeroToZero()
        {
            var container = new Container { Id = "one", Data = Celsius.Zero };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal(
                "{\"Id\":\"one\",\"Data\":0}",
                serialized
            );
        }

        [Fact]
        public void DeserializesZeroToZero()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":0}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(Celsius.Zero, deserialized.Data);
            Assert.NotEqual(default, deserialized.Data);
            
            Assert.True(deserialized.Data.IsInitialized());
        }

        [Fact]
        public void DeserializesMissingToUninitialized()
        {
            var serialized = "{\"Id\":\"one\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(default, deserialized.Data);
            
            Assert.False(deserialized.Data.IsInitialized());
        }

        [Fact]
        public void DeserializesNullToUninitialized()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":null}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(default, deserialized.Data);
            
            Assert.False(deserialized.Data.IsInitialized());
        }

        internal class Container
        {
            public required string Id { get; set; }
            public Celsius Data { get; set; }
        }
    }

    public class TypeConversion : CelsiusTests
    {
        [Fact]
        public void CanConvertFromUnderlyingType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.True(converter.CanConvertFrom(typeof(decimal)));

            var backingValue = 24.2m;
            var actual = converter.ConvertFrom(backingValue);

            Assert.Equal(Celsius.From(backingValue), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.False(converter.CanConvertFrom(typeof(int)));

            Action act = () => converter.ConvertFrom(5);

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToUnderlyingType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.True(converter.CanConvertTo(typeof(decimal)));

            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);
            var actual = converter.ConvertTo(sut, typeof(decimal));

            Assert.Equal(backingValue, actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.False(converter.CanConvertTo(typeof(int)));

            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);
            Action act = () => converter.ConvertTo(sut, typeof(int));

            Assert.Throws<NotSupportedException>(act);
        }
    }

    [ValueObject<decimal>(
        fromUnderlyingTypeCasting: CastOperator.Explicit,
        toUnderlyingTypeCasting: CastOperator.Explicit
    )]
    public readonly partial record struct OtherCelsius;
}