using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public partial class LegacyPhoneNumberTests
{
    public class Construction : LegacyPhoneNumberTests
    {
        // Does not compile, as intended, when the analyzer works correctly.
        /*[Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new LegacyPhoneNumber();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }*/
    }

    public class From : LegacyPhoneNumberTests
    {
        [Fact]
        public void From_CreatesLegacyPhoneNumberWithValue()
        {
            var actual = LegacyPhoneNumber.From("+31 488 24 55 33");
            Assert.Equal("+31 488 24 55 33", actual.Value);
        }

        [Fact]
        public void CannotCreateUninitializedWithNullValue()
        {
            Action act = () => LegacyPhoneNumber.From(null);

            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void CanCreateEmpty()
        {
            var actual = LegacyPhoneNumber.From(string.Empty);

            var empty = LegacyPhoneNumber.Empty;
            Assert.True(actual.Equals(empty));
            Assert.True(actual == empty);
            Assert.False(actual != empty);
            Assert.Equal(actual.GetHashCode(), empty.GetHashCode());

            Assert.True(actual.IsInitialized());
        }
    }

    public class TryFrom : LegacyPhoneNumberTests
    {
        [Fact]
        public void TryFrom_CreatesLegacyPhoneNumberWithValue()
        {
            var success = LegacyPhoneNumber.TryFrom("+31 488 24 55 33", out var actual);

            Assert.True(success);
            Assert.Equal("+31 488 24 55 33", actual.Value);
        }

        [Fact]
        public void CannotCreateUninitializedWithNullValue()
        {
            var success = LegacyPhoneNumber.TryFrom(null, out var actual);

            Assert.False(success);
            Assert.False(actual.IsInitialized());
        }

        [Fact]
        public void CanCreateEmpty()
        {
            var success = LegacyPhoneNumber.TryFrom(string.Empty, out var actual);

            Assert.True(success);

            var empty = LegacyPhoneNumber.Empty;
            Assert.True(actual.Equals(empty));
            Assert.True(actual == empty);
            Assert.False(actual != empty);
            Assert.Equal(actual.GetHashCode(), empty.GetHashCode());

            Assert.True(actual.IsInitialized());
        }
    }

    public class Value : LegacyPhoneNumberTests
    {
        [Fact]
        public void ReturnsUnderlyingValue()
        {
            var expected = "+31 488 24 55 33";

            var actual = LegacyPhoneNumber.From(expected);

            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public void EmptyReturnsExpectedUnderlyingValue()
        {
            var actual = LegacyPhoneNumber.Empty;

            Assert.Equal(string.Empty, actual.Value);
        }
    }

    public class Equality : LegacyPhoneNumberTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33");
            var second = LegacyPhoneNumber.From(first.Value);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33");
            var second = LegacyPhoneNumber.From("+44 1.5458.55.44.8");

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValuesAreDifferentlyCased_AreNotEqual()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33 A");
            var second = LegacyPhoneNumber.From("+31 488 24 55 33 a");

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToDefault()
        {
            LegacyPhoneNumber first = default;
            LegacyPhoneNumber second = default;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsNotEqualToEmpty()
        {
            LegacyPhoneNumber first = default;
            var second = LegacyPhoneNumber.Empty;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33");
            LegacyPhoneNumber second = default;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotLegacyPhoneNumber_AreNotEqual()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33");
            var second = new object();

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void GivenOtherObjectIsAnotherValueType_AreNotEqual()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33");
            var second = OtherLegacyPhoneNumber.From(first.Value);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));
        }

        [Fact]
        public void DoesNotHaveEqualityOperatorsForUnderlyingType_ButCastsImplicitly()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33");
            var second = "+31 488 24 55 33";

            Assert.True(first == second);
            Assert.True(second == first);
            Assert.False(first != second);
            Assert.False(second != first);

            var third = "+44 1.5458.55.44.8";
            Assert.False(first == third);
            Assert.False(third == first);
            Assert.True(first != third);
            Assert.True(third != first);
        }
    }

    public class IsInitialized : LegacyPhoneNumberTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var sut = LegacyPhoneNumber.From("+31 488 24 55 33");

            Assert.True(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsDefault_IsFalse()
        {
            LegacyPhoneNumber sut = default;

            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void EmptyIsInitialized()
        {
            var sut = LegacyPhoneNumber.Empty;

            Assert.True(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : LegacyPhoneNumberTests
    {
        [Fact]
        public void ReturnsValue()
        {
            var value = Guid.NewGuid().ToString();

            var actual = LegacyPhoneNumber.From(value).ToString();

            Assert.Equal(value, actual);
        }
    }

    public class ConversionOperatorsForUnderlyingType : LegacyPhoneNumberTests
    {
        [Fact]
        public void IsImplicitlyConvertibleToUnderlyingType()
        {
            var value = Guid.NewGuid().ToString();
            var obj = LegacyPhoneNumber.From(value);

            string actual = obj;

            Assert.Equal(value, actual);
        }

        [Fact]
        public void IsExplicitlyConvertibleToUnderlyingType()
        {
            var value = Guid.NewGuid().ToString();
            var obj = LegacyPhoneNumber.From(value);

            var actual = (string)obj;

            Assert.Equal(value, actual);
        }

        [Fact]
        public void IsExplicitlyConvertibleFromUnderlyingType()
        {
            var value = Guid.NewGuid().ToString();
            var str = value;

            var actual = (LegacyPhoneNumber)str;

            var expected = LegacyPhoneNumber.From(value);
            Assert.Equal(expected, actual);
        }
    }

    public class Serialization : LegacyPhoneNumberTests
    {
        private static readonly JsonSerializerOptions JsonOptions;

        static Serialization()
        {
            JsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };
        }

        [Fact]
        public void WhenNonsense_ThrowsJsonException()
        {
            var nonsense = "{}";

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<LegacyPhoneNumber>(nonsense, JsonOptions)
            );
        }

        [Fact]
        public void WhenEmptyString_CreatesEmpty()
        {
            var actual = JsonSerializer.Deserialize<LegacyPhoneNumber>("\"\"", JsonOptions);

            Assert.Equal(LegacyPhoneNumber.Empty, actual);
        }

        [Fact]
        public void CanRoundTrip()
        {
            var original = LegacyPhoneNumber.From("+44 1.5458.55.44.8");

            var serialized = JsonSerializer.Serialize(original, JsonOptions);
            var deserialized = JsonSerializer.Deserialize<LegacyPhoneNumber>(
                serialized,
                JsonOptions
            );

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var sut = LegacyPhoneNumber.From("+44 1.5458.55.44.8");

            var serialized = JsonSerializer.Serialize(sut, JsonOptions);

            Assert.Equal("\"+44 1.5458.55.44.8\"", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            LegacyPhoneNumber original = default;

            var serialized = JsonSerializer.Serialize(original, JsonOptions);

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<LegacyPhoneNumber>(
                serialized,
                JsonOptions
            );

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripEmpty()
        {
            var original = LegacyPhoneNumber.Empty;

            var serialized = JsonSerializer.Serialize(original, JsonOptions);

            Assert.Equal("\"\"", serialized);

            var deserialized = JsonSerializer.Deserialize<LegacyPhoneNumber>(
                serialized,
                JsonOptions
            );

            Assert.Equal(original, deserialized);
            Assert.True(deserialized.IsInitialized());
        }

        [Fact]
        public void SerializesUninitializedToNull()
        {
            var container = new Container { Id = "one", Data = default };

            var serialized = JsonSerializer.Serialize(container, JsonOptions);

            Assert.Equal("{\"id\":\"one\"}", serialized);
        }

        [Fact]
        public void SerializesEmptyToEmpty()
        {
            var container = new Container { Id = "one", Data = LegacyPhoneNumber.Empty };

            var serialized = JsonSerializer.Serialize(container, JsonOptions);

            Assert.Equal("{\"id\":\"one\",\"data\":\"\"}", serialized);
        }

        [Fact]
        public void DeserializesEmptyToEmpty()
        {
            var serialized = "{\"id\":\"one\",\"data\":\"\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized, JsonOptions);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(LegacyPhoneNumber.Empty, deserialized.Data);
            Assert.NotEqual(default, deserialized.Data);

            Assert.True(deserialized.Data.IsInitialized());
        }

        [Fact]
        public void DeserializesMissingToUninitialized()
        {
            var serialized = "{\"id\":\"one\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized, JsonOptions);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.NotEqual(LegacyPhoneNumber.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);

            Assert.False(deserialized.Data.IsInitialized());
        }

        [Fact]
        public void DeserializesNullToUninitialized()
        {
            var serialized = "{\"id\":\"one\",\"data\":null}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized, JsonOptions);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.NotEqual(LegacyPhoneNumber.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);

            Assert.False(deserialized.Data.IsInitialized());
        }

        internal class Container
        {
            public required string Id { get; set; }
            public LegacyPhoneNumber Data { get; set; }
        }
    }

    public class TypeConversion : LegacyPhoneNumberTests
    {
        [Fact]
        public void CanConvertFromUnderlyingType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(LegacyPhoneNumber));
            Assert.True(converter.CanConvertFrom(typeof(string)));

            var actual = converter.ConvertFrom("+44 1.5458.55.44.8");

            Assert.Equal(LegacyPhoneNumber.From("+44 1.5458.55.44.8"), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(LegacyPhoneNumber));
            Assert.False(converter.CanConvertFrom(typeof(int)));

            Action act = () => converter.ConvertFrom(5);

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToUnderlyingType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(LegacyPhoneNumber));
            Assert.True(converter.CanConvertTo(typeof(string)));

            var sut = LegacyPhoneNumber.From("+44 1.5458.55.44.8");
            var actual = converter.ConvertTo(sut, typeof(string));

            Assert.Equal("+44 1.5458.55.44.8", actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(LegacyPhoneNumber));
            Assert.False(converter.CanConvertTo(typeof(int)));

            var sut = LegacyPhoneNumber.From("+44 1.5458.55.44.8");
            Action act = () => converter.ConvertTo(sut, typeof(int));

            Assert.Throws<NotSupportedException>(act);
        }
    }

    public class IsValid : LegacyPhoneNumberTests
    {
        [Fact]
        public void ValidInstanceIsValid()
        {
            var sut = LegacyPhoneNumber.From("+44 1.5458.55.44.8");

            Assert.True(sut.IsValid());
        }

        [Fact]
        public void EmptyIsValid()
        {
            var sut = LegacyPhoneNumber.Empty;

            Assert.True(sut.IsValid());
        }
    }

    public class GetValidationErrorMessage : LegacyPhoneNumberTests
    {
        [Fact]
        public void WhenValidReturnsNull()
        {
            var sut = LegacyPhoneNumber.From("+44 1.5458.55.44.8");

            Assert.Null(sut.GetValidationErrorMessage());
        }
    }

    public class FluentValidationExtensions : LegacyPhoneNumberTests
    {
        public class MustBeInitialized : FluentValidationExtensions
        {
            private readonly Func<ValidationResult> _act;
            private LegacyPhoneNumber _vo;

            public MustBeInitialized()
            {
                _vo = LegacyPhoneNumber.From("+44 1.5458.55.44.8");
                _act = () =>
                    new ContainerValidator().Validate(new Container { Id = "one", Data = _vo });
            }

            [Fact]
            public void WhenValid_ReturnsValid()
            {
                _vo = LegacyPhoneNumber.From("+44 1.5458.55.44.8");

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
                Assert.Equal(
                    "LegacyPhoneNumber must be initialized.",
                    result.Errors[0].ErrorMessage
                );
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
            public LegacyPhoneNumber Data { get; set; }
        }
    }

    public class CreationMethodFromUnderlyingType : LegacyPhoneNumberTests
    {
        [Fact]
        public void CanCreateValid()
        {
            var actual = "+44 1.5458.55.44.8".LegacyPhoneNumber();

            var expected = LegacyPhoneNumber.From("+44 1.5458.55.44.8");
            Assert.Equal(expected, actual);
            Assert.True(actual.IsValid());
        }

        [Fact]
        public void CannotCreateFromNullInput()
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Action act = () => ((string)null).LegacyPhoneNumber();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void CanCreateEmpty()
        {
            var actual = string.Empty.LegacyPhoneNumber();

            Assert.Equal(LegacyPhoneNumber.Empty, actual);
            Assert.True(actual.IsValid());
        }
    }

    [ValueObject<string>(
        fromUnderlyingTypeCasting: CastOperator.Explicit,
        toUnderlyingTypeCasting: CastOperator.Explicit,
        comparison: ComparisonGeneration.Omit
    )]
    public readonly partial record struct OtherLegacyPhoneNumber;
}
