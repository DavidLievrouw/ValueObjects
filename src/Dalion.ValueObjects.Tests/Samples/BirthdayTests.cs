using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public partial class BirthdayTests
{
    public class Construction : BirthdayTests
    {
        // Does not compile, as intended, when the analyzer works correctly.
        /*[Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new Birthday();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }*/
    }

    public class From : BirthdayTests
    {
        [Fact]
        public void From_CreatesBirthdayWithValue()
        {
            var backingValue = new DateOnly(2020, 3, 12);

            var actual = Birthday.From(backingValue);

            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CanCreateNone()
        {
            var backingValue = DateOnly.MinValue;

            var actual = Birthday.From(backingValue);

            Assert.Equal(backingValue, actual.Value);

            var none = Birthday.None;
            Assert.True(actual.Equals(none));
            Assert.True(actual == none);
            Assert.False(actual != none);
            Assert.Equal(actual.GetHashCode(), none.GetHashCode());

            Assert.True(actual.IsInitialized());
        }

        [Fact]
        public void CanCreatePreSetInstance()
        {
            var actual = Birthday.From(new DateOnly(1976, 9, 13));

            Assert.Equal(Birthday.Patrick, actual);
            Assert.True(actual.IsInitialized());
            Assert.True(actual.IsValid());
        }

        [Fact]
        public void CannotCreateInvalidInstance()
        {
            Action act = () => Birthday.From(new DateOnly(4000, 1, 1));

            Assert.Throws<ArgumentException>(act);
        }
    }

    public class TryFrom : BirthdayTests
    {
        [Fact]
        public void TryFrom_CreatesBirthdayWithValue()
        {
            var backingValue = new DateOnly(2020, 3, 12);

            var success = Birthday.TryFrom(backingValue, out var actual);

            Assert.True(success);
            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CanCreateNone()
        {
            var success = Birthday.TryFrom(DateOnly.MinValue, out var actual);

            Assert.True(success);

            var zero = Birthday.None;
            Assert.True(actual.Equals(zero));
            Assert.True(actual == zero);
            Assert.False(actual != zero);
            Assert.Equal(actual.GetHashCode(), zero.GetHashCode());

            Assert.True(actual.IsInitialized());
        }

        [Fact]
        public void CanCreatePreSetInstance()
        {
            var success = Birthday.TryFrom(new DateOnly(1976, 9, 13), out var actual);

            Assert.True(success);
            Assert.Equal(Birthday.Patrick, actual);
            Assert.True(actual.IsInitialized());
            Assert.True(actual.IsValid());
        }

        [Fact]
        public void CannotCreateInvalidInstance()
        {
            var success = Birthday.TryFrom(new DateOnly(4000, 1, 1), out _);

            Assert.False(success);
        }
    }

    public class Value : BirthdayTests
    {
        [Fact]
        public void ReturnsUnderlyingValue()
        {
            var expected = new DateOnly(2020, 3, 12);

            var actual = Birthday.From(expected);

            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public void NoneReturnsExpectedUnderlyingValue()
        {
            var actual = Birthday.None;

            Assert.Equal(DateOnly.MinValue, actual.Value);
        }
    }

    public class Comparison : BirthdayTests
    {
        [Fact]
        public void CompareTo_WorksAsExpected()
        {
            var a = Birthday.From(new DateOnly(1976, 9, 13));
            var b = Birthday.From(new DateOnly(2020, 3, 12));
            var a2 = Birthday.From(new DateOnly(1976, 9, 13));

            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
            Assert.True(a.CompareTo(a2) == 0);
        }
    }

    public class Equality : BirthdayTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var first = Birthday.From(backingValue);
            var second = Birthday.From(first.Value);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var backingValue1 = new DateOnly(2020, 3, 12);
            var first = Birthday.From(backingValue1);
            var backingValue2 = new DateOnly(1976, 9, 13);
            var second = Birthday.From(backingValue2);

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToDefault()
        {
            Birthday first = default;
            Birthday second = default;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsNotEqualToNone()
        {
            Birthday first = default;
            var second = Birthday.None;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var first = Birthday.From(backingValue);
            Birthday second = default;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotBirthday_AreNotEqual()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var first = Birthday.From(backingValue);
            var second = new object();

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void GivenOtherObjectIsAnotherValueType_AreNotEqual()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var first = Birthday.From(backingValue);
            var second = OtherBirthday.From(backingValue);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));
        }

        [Fact]
        public void HasEqualityMethodsForUnderlyingType()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var first = Birthday.From(backingValue);
            var second = backingValue;

            Assert.True(first.Equals(second));

            var third = new DateOnly(1976, 9, 13);
            Assert.False(first.Equals(third));
        }

        [Fact]
        public void HasEqualityOperatorsForUnderlyingType()
        {
            var first = Birthday.From(new DateOnly(2020, 3, 12));
            var second = new DateOnly(2020, 3, 12);

            Assert.True(first == second);
            Assert.True(second == first);
            Assert.False(first != second);
            Assert.False(second != first);

            var third = new DateOnly(1976, 9, 13);
            Assert.False(first == third);
            Assert.False(third == first);
            Assert.True(first != third);
            Assert.True(third != first);
        }
    }

    public class IsInitialized : BirthdayTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var sut = Birthday.From(backingValue);

            Assert.True(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsDefault_IsFalse()
        {
            Birthday sut = default;

            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void NoneIsInitialized()
        {
            var sut = Birthday.None;

            Assert.True(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : BirthdayTests
    {
        [Fact]
        public void ReturnsValueInCurrentLocale()
        {
            var value = new DateOnly(2020, 3, 12);

            var actual = Birthday.From(value).ToString();

            var expected = value.ToString(CultureInfo.CurrentCulture);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HasIFormattableSupport()
        {
            var value = new DateOnly(2020, 3, 12);

            var vo = Birthday.From(value);
            var actual = vo.ToString("MM-dd", CultureInfo.InvariantCulture);

            var expected = "03-12";
            Assert.Equal(expected, actual);
            Assert.IsAssignableFrom<IFormattable>(vo);
        }
    }

    public class ConversionOperatorsForUnderlyingType : BirthdayTests
    {
        [Fact]
        public void IsExplicitlyConvertibleToUnderlyingType()
        {
            var value = new DateOnly(2020, 3, 12);
            var obj = Birthday.From(value);

            var actual = (DateOnly)obj;

            Assert.Equal(value, actual);
        }

        [Fact]
        public void IsExplicitlyConvertibleFromUnderlyingType()
        {
            var value = new DateOnly(2020, 3, 12);
            var str = value;

            var actual = (Birthday)str;

            var expected = Birthday.From(value);
            Assert.Equal(expected, actual);
        }
    }

    public class Serialization : BirthdayTests
    {
        [Fact]
        public void WhenNonsense_ThrowsJsonException()
        {
            var nonsense = "\"nonsense\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Birthday>(nonsense));
        }

        [Fact]
        public void WhenInvalid_CannotDeserialize()
        {
            var invalid = "0001-01-01";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Birthday>(invalid));
        }

        [Fact]
        public void WhenEmptyString_ThrowsJsonException()
        {
            var nonsense = "\"\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Birthday>(nonsense));
        }

        [Fact]
        public void CanRoundTrip()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var original = Birthday.From(backingValue);

            var serialized = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<Birthday>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var sut = Birthday.From(backingValue);

            var serialized = JsonSerializer.Serialize(sut);

            Assert.Equal("\"2020-03-12\"", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            Birthday original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<Birthday>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripNone()
        {
            var original = Birthday.None;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"0001-01-01\"", serialized);

            var deserialized = JsonSerializer.Deserialize<Birthday>(serialized);

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
        public void SerializesNoneToNone()
        {
            var container = new Container { Id = "one", Data = Birthday.None };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":\"0001-01-01\"}", serialized);
        }

        [Fact]
        public void DeserializesNoneToNone()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":\"0001-01-01\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(Birthday.None, deserialized.Data);
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
            public Birthday Data { get; set; }
        }
    }

    public class TypeConversion : BirthdayTests
    {
        [Fact]
        public void CanConvertFromUnderlyingType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.True(converter.CanConvertFrom(typeof(DateOnly)));

            var backingValue = new DateOnly(2020, 3, 12);
            var actual = converter.ConvertFrom(backingValue);

            Assert.Equal(Birthday.From(backingValue), actual);
        }
        
        [Fact]
        public void CanConvertFromValueObject()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.True(converter.CanConvertFrom(typeof(Birthday)));

            var backingValue = new DateOnly(2020, 3, 12);
            var actual = converter.ConvertFrom(Birthday.From(backingValue));

            Assert.Equal(Birthday.From(backingValue), actual);
        }
        
        [Fact]
        public void CanConvertFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.True(converter.CanConvertFrom(typeof(string)));

            var backingValue = "2020-03-12";
            var actual = converter.ConvertFrom(backingValue);

            Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.False(converter.CanConvertFrom(typeof(int)));

            Action act = () => converter.ConvertFrom(5);

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToUnderlyingType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.True(converter.CanConvertTo(typeof(DateOnly)));

            var backingValue = new DateOnly(2020, 3, 12);
            var sut = Birthday.From(backingValue);
            var actual = converter.ConvertTo(sut, typeof(DateOnly));

            Assert.Equal(backingValue, actual);
        }

        [Fact]
        public void CanConvertToValueObjectFromBackingValue()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.True(converter.CanConvertTo(typeof(Birthday)));

            var backingValue = new DateOnly(2020, 3, 12);
            var actual = converter.ConvertTo(backingValue, typeof(Birthday));

            Assert.Equal(Birthday.From(backingValue), actual);
        }

        [Fact]
        public void CanConvertToValueObjectFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.True(converter.CanConvertTo(typeof(Birthday)));

            var str = "2020-03-12";
            var actual = converter.ConvertTo(str, typeof(Birthday));

            Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
        }

        [Fact]
        public void CanConvertToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.True(converter.CanConvertTo(typeof(string)));

            var backingValue = new DateOnly(2020, 3, 12);
            var sut = Birthday.From(backingValue);
            var actual = converter.ConvertTo(sut, typeof(string));

            Assert.Equal("2020-03-12", actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Birthday));
            Assert.False(converter.CanConvertTo(typeof(int)));

            var backingValue = new DateOnly(2020, 3, 12);
            var sut = Birthday.From(backingValue);
            Action act = () => converter.ConvertTo(sut, typeof(int));

            Assert.Throws<NotSupportedException>(act);
        }
    }

    public class IsValid : BirthdayTests
    {
        [Fact]
        public void ValidInstanceIsValid()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var sut = Birthday.From(backingValue);

            Assert.True(sut.IsValid());
        }

        [Fact]
        public void NoneIsInvalid()
        {
            var sut = Birthday.None;

            Assert.False(sut.IsValid());
        }
    }

    public class GetValidationErrorMessage : BirthdayTests
    {
        [Fact]
        public void WhenValidReturnsNull()
        {
            var backingValue = new DateOnly(2020, 3, 12);
            var sut = Birthday.From(backingValue);

            Assert.Null(sut.GetValidationErrorMessage());
        }
    }

    public class None : BirthdayTests
    {
        [Fact]
        public void ObjectHasExpectedUnderlyingValue()
        {
            var actual = Birthday.None.Value;
            Assert.Equal(DateOnly.MinValue, actual);
        }

        [Fact]
        public void ObjectIsInitialized()
        {
            Assert.True(Birthday.None.IsInitialized());
        }

        [Fact]
        public void ObjectIsInvalid()
        {
            Assert.False(Birthday.None.IsValid());
        }
    }

    public class FluentValidationExtensions : BirthdayTests
    {
        public class MustBeInitializedAndValid : FluentValidationExtensions
        {
            private readonly Func<ValidationResult> _act;
            private Birthday _vo;

            public MustBeInitializedAndValid()
            {
                _vo = Birthday.From(new DateOnly(2020, 3, 12));
                _act = () =>
                    new ContainerValidator().Validate(new Container { Id = "one", Data = _vo });
            }

            [Fact]
            public void WhenValid_ReturnsValid()
            {
                _vo = Birthday.From(new DateOnly(2020, 3, 12));

                var result = _act();

                Assert.True(result.IsValid);
            }

            [Fact]
            public void WhenInvalid_ReturnsInvalid()
            {
                Birthday.TryFrom(DateOnly.MinValue, out _vo);

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
                Birthday.TryFrom(DateOnly.MinValue, out _vo);

                var result = _act();

                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
                Assert.Equal(
                    "Birthday must be initialized (greater than 0001-01-01).",
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
            private Birthday _vo;

            public MustBeInitialized()
            {
                _vo = Birthday.From(new DateOnly(2020, 3, 12));
                _act = () =>
                    new ContainerValidator().Validate(new Container { Id = "one", Data = _vo });
            }

            [Fact]
            public void WhenValid_ReturnsValid()
            {
                _vo = Birthday.From(new DateOnly(2020, 3, 12));

                var result = _act();

                Assert.True(result.IsValid);
            }

            [Fact]
            public void WhenInvalid_ReturnsValid()
            {
                Birthday.TryFrom(new DateOnly(2020, 3, 12), out _vo);

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
                Assert.Equal("Birthday must be initialized.", result.Errors[0].ErrorMessage);
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
            public Birthday Data { get; set; }
        }
    }

    public class Parsable : BirthdayTests
    {
        public class FromString : Parsable
        {
            [Fact]
            public void CanParseFromString()
            {
                var str = "2020-03-12";

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanParsePreSetValue()
            {
                var str = "1976-09-13";

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.Patrick, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CanParseInvalidWhenPreSetValue()
            {
                var str = "3000-01-01"; // invalid, but pre-set value

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.InvalidFuture, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CannotParseNonsenseValue()
            {
                var str = "nonsense";

                Action act = () => Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Throws<FormatException>(act);
            }

            [Fact]
            public void CannotParseInvalidValue()
            {
                var str = "4000-01-01";

                Action act = () => Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Throws<ArgumentException>(act);
            }

            [Fact]
            public void CanParseNone()
            {
                var str = "0001-01-01";

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.None, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanTryParseFromString()
            {
                var str = "2020-03-12";

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotTryParseNonsenseValue()
            {
                var str = "nonsense";

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CannotTryParseInvalidValue()
            {
                var str = "4000-01-01";

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CanTryParseInvalidWhenPreSetValue()
            {
                var str = "3000-01-01"; // invalid, but pre-set value

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.InvalidFuture, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanTryParsePreSetValue()
            {
                var str = "1976-09-13";

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.Patrick, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanTryParseNone()
            {
                var str = "0001-01-01";

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.None, actual);
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
                var str = ToReadOnlySpan("2020-03-12");

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotParseInvalidValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("4000-01-01");
                    Birthday.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<ArgumentException>(act);
            }

            [Fact]
            public void CanParsePreSetValue()
            {
                var str = ToReadOnlySpan("1976-09-13");

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.Patrick, actual);
                Assert.True(actual.IsInitialized());
                Assert.True(actual.IsValid());
            }

            [Fact]
            public void CanParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("3000-01-01");

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.InvalidFuture, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CannotParseNonsenseValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("nonsense");
                    Birthday.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<FormatException>(act);
            }

            [Fact]
            public void CanParseNone()
            {
                var str = ToReadOnlySpan("0001-01-01");

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.None, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanTryParseFromString()
            {
                var str = ToReadOnlySpan("2020-03-12");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotTryParseNonsenseValue()
            {
                var str = ToReadOnlySpan("nonsense");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CannotTryParseInvalidValue()
            {
                var str = ToReadOnlySpan("4000-01-01");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CanTryParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("3000-01-01");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.InvalidFuture, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotTryParseInvalid()
            {
                var str = ToReadOnlySpan("4000-01-01");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.False(success);
            }

            [Fact]
            public void CanTryParseNone()
            {
                var str = ToReadOnlySpan("0001-01-01");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.None, actual);
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
                var str = ToReadOnlySpan("2020-03-12");

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotParseInvalidValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("4000-01-01");
                    Birthday.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<ArgumentException>(act);
            }

            [Fact]
            public void CanParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("3000-01-01"); // invalid, but pre-set value

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.InvalidFuture, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CannotParseNonsenseValue()
            {
                var act = () =>
                {
                    var str = ToReadOnlySpan("nonsense");
                    Birthday.Parse(str, CultureInfo.InvariantCulture);
                };

                Assert.Throws<FormatException>(act);
            }

            [Fact]
            public void CanParseNone()
            {
                var str = ToReadOnlySpan("0001-01-01");

                var actual = Birthday.Parse(str, CultureInfo.InvariantCulture);

                Assert.Equal(Birthday.None, actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CanTryParseFromString()
            {
                var str = ToReadOnlySpan("2020-03-12");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.From(new DateOnly(2020, 3, 12)), actual);
                Assert.True(actual.IsInitialized());
            }

            [Fact]
            public void CannotTryParseNonsenseValue()
            {
                var str = ToReadOnlySpan("nonsense");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.False(success);
            }

            [Fact]
            public void CannotTryParseInvalidValue()
            {
                var str = ToReadOnlySpan("4000-01-01");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out _);

                Assert.False(success);
            }

            [Fact]
            public void CanTryParseInvalidWhenPreSetValue()
            {
                var str = ToReadOnlySpan("3000-01-01"); // invalid, but pre-set value

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.InvalidFuture, actual);
                Assert.True(actual.IsInitialized());
                Assert.False(actual.IsValid());
            }

            [Fact]
            public void CanTryParseNone()
            {
                var str = ToReadOnlySpan("0001-01-01");

                var success = Birthday.TryParse(str, CultureInfo.InvariantCulture, out var actual);

                Assert.True(success);
                Assert.Equal(Birthday.None, actual);
                Assert.True(actual.IsInitialized());
            }
        }
    }

    [ValueObject<DateOnly>(
        fromUnderlyingTypeCasting: CastOperator.Explicit,
        toUnderlyingTypeCasting: CastOperator.Explicit
    )]
    public readonly partial record struct OtherBirthday;
}