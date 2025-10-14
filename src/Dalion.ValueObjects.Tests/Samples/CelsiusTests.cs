using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
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
        [InlineData(-459.67)]
        public void CanCreatePreSetInstance(double preSet)
        {
            var dec = (decimal)preSet;
            var actual = Celsius.From(dec);
            
            Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
            Assert.True(actual.IsInitialized());
            Assert.False(actual.IsValid());
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
        [InlineData(-459.67)]
        public void CanCreatePreSetInstance(double preSet)
        {
            var dec = (decimal)preSet;
            var success = Celsius.TryFrom(dec, out var actual);
            
            Assert.True(success);
            Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
            Assert.True(actual.IsInitialized());
            Assert.False(actual.IsValid());
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
        public void ZeroIsInitialized()
        {
            var sut = Celsius.Zero;

            Assert.True(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : CelsiusTests
    {
        [Fact]
        public void ReturnsValueInCurrentLocale()
        {
            var value = 24.2m;

            var actual = Celsius.From(value).ToString();

            Assert.Equal(value.ToString(CultureInfo.CurrentCulture), actual);
        }

        [Fact]
        public void HasIFormattableSupport()
        {
            var value = 24.2m;

            var vo = Celsius.From(value);
            var actual = vo.ToString("000.00", CultureInfo.InvariantCulture);

            var expected = "024.20";
            Assert.Equal(expected, actual);
            Assert.IsAssignableFrom<IFormattable>(vo);
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
        public void WhenInvalid_CannotDeserialize()
        {
            var invalid = "-99999"; // lower than absolute zero

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Celsius>(invalid));
        }

        [Fact]
        public void WhenInvalid_ButPreSet_CanDeserialize()
        {
            var invalid = "-459.67"; // absolute zero in fahrenheit, but not in celsius

            var deserialized = JsonSerializer.Deserialize<Celsius>(invalid);

            Assert.Equal(Celsius.AbsoluteZeroFahrenheit, deserialized);
            Assert.True(deserialized.IsInitialized());
            Assert.False(deserialized.IsValid());
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

            Assert.Equal("{\"Id\":\"one\",\"Data\":null}", serialized);
        }

        [Fact]
        public void SerializesZeroToZero()
        {
            var container = new Container { Id = "one", Data = Celsius.Zero };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":0}", serialized);
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
            var converter = TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.True(converter.CanConvertFrom(typeof(decimal)));

            var backingValue = 24.2m;
            var actual = converter.ConvertFrom(backingValue);

            Assert.Equal(Celsius.From(backingValue), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.False(converter.CanConvertFrom(typeof(int)));

            Action act = () => converter.ConvertFrom(5);

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToUnderlyingType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.True(converter.CanConvertTo(typeof(decimal)));

            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);
            var actual = converter.ConvertTo(sut, typeof(decimal));

            Assert.Equal(backingValue, actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Celsius));
            Assert.False(converter.CanConvertTo(typeof(int)));

            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);
            Action act = () => converter.ConvertTo(sut, typeof(int));

            Assert.Throws<NotSupportedException>(act);
        }
    }

    public class IsValid : CelsiusTests
    {
        [Fact]
        public void ValidInstanceIsValid()
        {
            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);

            Assert.True(sut.IsValid());
        }

        [Fact]
        public void ZeroIsValid()
        {
            var sut = Celsius.Zero;

            Assert.True(sut.IsValid());
        }
    }

    public class GetValidationErrorMessage : CelsiusTests
    {
        [Fact]
        public void WhenValidReturnsNull()
        {
            var backingValue = 24.2m;
            var sut = Celsius.From(backingValue);

            Assert.Null(sut.GetValidationErrorMessage());
        }
    }

    public class Zero : CelsiusTests
    {
        [Fact]
        public void ObjectHasExpectedUnderlyingValue()
        {
            var actual = Celsius.Zero.Value;
            Assert.Equal(0, actual);
        }

        [Fact]
        public void ObjectIsInitialized()
        {
            Assert.True(Celsius.Zero.IsInitialized());
        }

        [Fact]
        public void ObjectIsValid()
        {
            Assert.True(Celsius.Zero.IsValid());
        }
    }

    public class AbsoluteZero : CelsiusTests
    {
        [Fact]
        public void ObjectHasExpectedUnderlyingValue()
        {
            var actual = Celsius.AbsoluteZero.Value;
            Assert.Equal(-273.15m, actual);
        }

        [Fact]
        public void ObjectIsInitialized()
        {
            Assert.True(Celsius.AbsoluteZero.IsInitialized());
        }

        [Fact]
        public void ObjectIsValid()
        {
            Assert.True(Celsius.AbsoluteZero.IsValid());
        }
    }

    public class FluentValidationExtensions : CelsiusTests
    {
        public class MustBeInitializedAndValid : FluentValidationExtensions
        {
            private readonly Func<ValidationResult> _act;
            private Celsius _vo;

            public MustBeInitializedAndValid()
            {
                _vo = Celsius.From(24.2m);
                _act = () =>
                    new ContainerValidator().Validate(new Container { Id = "one", Data = _vo });
            }

            [Fact]
            public void WhenValid_ReturnsValid()
            {
                _vo = Celsius.From(24.2m);

                var result = _act();

                Assert.True(result.IsValid);
            }

            [Fact]
            public void WhenInvalid_ReturnsInvalid()
            {
                Celsius.TryFrom(-300, out _vo); // lower than absolute zero

                var result = _act();

                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
            }

            [Fact]
            public void WhenUninitialized_ReturnsInvalid()
            {
                _vo = default;

                var result = _act();

                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
            }

            [Fact]
            public void HasValidationErrorMessage()
            {
                Celsius.TryFrom(-300, out _vo); // lower than absolute zero

                var result = _act();

                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
                Assert.Equal(
                    "Temperature cannot be below absolute zero (-273.15°C).",
                    result.Errors[0].ErrorMessage
                );
            }

            internal class ContainerValidator : AbstractValidator<Container>
            {
                public ContainerValidator()
                {
                    RuleFor(c => c.Data).MustBeInitializedAndValid();
                }
            }
        }

        public class MustBeInitialized : FluentValidationExtensions
        {
            private readonly Func<ValidationResult> _act;
            private Celsius _vo;

            public MustBeInitialized()
            {
                _vo = Celsius.From(24.2m);
                _act = () =>
                    new ContainerValidator().Validate(new Container { Id = "one", Data = _vo });
            }

            [Fact]
            public void WhenValid_ReturnsValid()
            {
                _vo = Celsius.From(24.2m);

                var result = _act();

                Assert.True(result.IsValid);
            }

            [Fact]
            public void WhenInvalid_ReturnsValid()
            {
                Celsius.TryFrom(-300, out _vo); // lower than absolute zero

                var result = _act();

                Assert.True(result.IsValid);
            }

            [Fact]
            public void WhenUninitialized_ReturnsInvalid()
            {
                _vo = default;

                var result = _act();

                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
            }

            [Fact]
            public void HasValidationErrorMessage()
            {
                _vo = default;

                var result = _act();

                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
                Assert.Equal("Celsius must be initialized.", result.Errors[0].ErrorMessage);
            }

            internal class ContainerValidator : AbstractValidator<Container>
            {
                public ContainerValidator()
                {
                    RuleFor(c => c.Data).MustBeInitialized();
                }
            }
        }

        internal class Container
        {
            public required string Id { get; set; }
            public Celsius Data { get; set; }
        }
    }

    public class Parsable : CelsiusTests
    {
        public class FromString : Parsable
        {
            [Fact]
            public void CanParseFromString()
            {
                var str = "24.2";

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.From(24.2m), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotParseInvalidValue()
            {
                var str = "-300"; // lower than absolute zero

                Action act = () => Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Throws<InvalidOperationException>(act);
            }

            [Fact]
            public void CanParseInvalidWhenPreSetValue()
            {
                var str = "-459.67"; // invalid, but pre-set value

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanParsePreSetValue()
            {
                var str = "-273.15";

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.AbsoluteZero, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CannotParseNonsenseValue()
            {
                var str = "nonsense";

                Action act = () => Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Throws<FormatException>(act);
            }

            [Fact]
            public void CanParseZero()
            {
                var str = "0";

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.Zero, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanTryParseFromString()
            {
                var str = "24.2";

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.From(24.2m), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotTryParseNonsenseValue()
            {
                var str = "nonsense";

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CannotTryParseInvalidValue()
            {
                var str = "-300"; // lower than absolute zero

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CanTryParseInvalidWhenPreSetValue()
            {
                var str = "-459.67"; // invalid, but pre-set value

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanTryParsPreSetValue()
            {
                var str = "-273.15";

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.AbsoluteZero, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CanTryParseZero()
            {
                var str = "0";

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.Zero, actual);
                Assert.True(actual.IsInitialized());
            }
        }

        public class FromReadOnlySpanOfChar : Parsable
        {
            private static ReadOnlySpan<char> ToReadOnlySpan(string? s)
            {
                return s is null ? ReadOnlySpan<char>.Empty : s.AsSpan();
            }

            [Fact]
            public void CanParseFromString()
            {
                var str = ToReadOnlySpan("24.2");

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.From(24.2m), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotParseInvalidValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("-300"); // lower than absolute zero
                    Celsius.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<InvalidOperationException>(act);
            }

            [Fact]
            public void CanParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("-459.67"); // invalid, but pre-set value

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanParsePreSetValue()
            {
                var str = ToReadOnlySpan("-273.15");

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.AbsoluteZero, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CannotParseNonsenseValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("nonsense");
                    Celsius.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<FormatException>(act);
            }

            [Fact]
            public void CanParseZero()
            {
                var str = ToReadOnlySpan("0");

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.Zero, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanTryParseFromString()
            {
                var str = ToReadOnlySpan("24.2");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.From(24.2m), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotTryParseNonsenseValue()
            {
                var str = ToReadOnlySpan("nonsense");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.False(success);
            }

            [Fact]
            public void CannotTryParseInvalidValue()
            {
                var str = ToReadOnlySpan("-300"); // lower than absolute zero

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.False(success);
            }

            [Fact]
            public void CanTryParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("-459.67"); // invalid, but pre-set value

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanTryParsePreSetValue()
            {
                var str = ToReadOnlySpan("-273.15");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.AbsoluteZero, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CanTryParseZero()
            {
                var str = ToReadOnlySpan("0");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.Zero, actual);
                Assert.True(actual.IsInitialized());
            }
        }

        public class FromReadOnlySpanOfByte : Parsable
        {
            private static ReadOnlySpan<byte> ToReadOnlySpan(string? s)
            {
                return s == null ? ReadOnlySpan<byte>.Empty : Encoding.UTF8.GetBytes(s);
            }

            [Fact]
            public void CanParseFromString()
            {
                var str = ToReadOnlySpan("24.2");

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.From(24.2m), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotParseInvalidValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("-300"); // lower than absolute zero
                    Celsius.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<InvalidOperationException>(act);
            }

            [Fact]
            public void CanParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("-459.67"); // invalid, but pre-set value

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanParsePreSetValue()
            {
                var str = ToReadOnlySpan("-273.15");

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.AbsoluteZero, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CannotParseNonsenseValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("nonsense");
                    Celsius.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<FormatException>(act);
            }

            [Fact]
            public void CanParseZero()
            {
                var str = ToReadOnlySpan("0");

                var actual = Celsius.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Celsius.Zero, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanTryParseFromString()
            {
                var str = ToReadOnlySpan("24.2");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.From(24.2m), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotTryParseNonsenseValue()
            {
                var str = ToReadOnlySpan("nonsense");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.False(success);
            }

            [Fact]
            public void CannotTryParseInvalidValue()
            {
                var str = ToReadOnlySpan("-300"); // lower than absolute zero

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CanTryParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("-459.67"); // invalid, but pre-set value

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.AbsoluteZeroFahrenheit, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanTryParsePreSetValue()
            {
                var str = ToReadOnlySpan("-273.15");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.AbsoluteZero, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CanTryParseZero()
            {
                var str = ToReadOnlySpan("0");

                var success = Celsius.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Celsius.Zero, actual);
                Assert.True(actual.IsInitialized());
            }
        }
    }

    [ValueObject<decimal>(
        fromUnderlyingTypeCasting: CastOperator.Explicit,
        toUnderlyingTypeCasting: CastOperator.Explicit
    )]
    public readonly partial record struct OtherCelsius;
}
