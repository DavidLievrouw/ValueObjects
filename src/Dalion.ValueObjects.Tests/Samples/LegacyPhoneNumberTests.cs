using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" \t  ")]
        public void CanCreateEmpty(string? invalid)
        {
            var empty = LegacyPhoneNumber.Empty;

            var actual = LegacyPhoneNumber.From(invalid);

            Assert.True(actual.Equals(empty));
            Assert.True(actual == empty);
            Assert.False(actual != empty);
            Assert.Equal(actual.GetHashCode(), empty.GetHashCode());
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" \t  ")]
        public void CannotCreateNullEmptyOrWhitespace(string? invalid)
        {
            var success = LegacyPhoneNumber.TryFrom(invalid, out _);

            Assert.False(success);
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
        public void WhenValueIsDefault_IsEqualToEmpty()
        {
            LegacyPhoneNumber first = default;
            var second = LegacyPhoneNumber.Empty;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
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
        public void HasEqualityMethodsForUnderlyingType()
        {
            var first = LegacyPhoneNumber.From("+31 488 24 55 33");
            var second = "+31 488 24 55 33";

            Assert.True(first.Equals(second));

            var third = "+44 1.5458.55.44.8";
            Assert.False(first.Equals(third));
        }

        [Fact]
        public void HasEqualityOperatorsForUnderlyingType()
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
        public void WhenValueIsEmpty_IsFalse()
        {
            var sut = LegacyPhoneNumber.Empty;

            Assert.False(sut.IsInitialized());
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

    public class ConversionOperatorsForPrimitive : LegacyPhoneNumberTests
    {
        [Fact]
        public void IsImplicitlyConvertibleToPrimitive()
        {
            var value = Guid.NewGuid().ToString();
            var obj = LegacyPhoneNumber.From(value);

            string actual = obj;

            Assert.Equal(value, actual);
        }

        [Fact]
        public void IsExplicitlyConvertibleToPrimitive()
        {
            var value = Guid.NewGuid().ToString();
            var obj = LegacyPhoneNumber.From(value);

            var actual = (string)obj;

            Assert.Equal(value, actual);
        }

        [Fact]
        public void IsImplicitlyConvertibleFromPrimitive()
        {
            var value = Guid.NewGuid().ToString();
            var str = value;

            LegacyPhoneNumber actual = str;

            var expected = LegacyPhoneNumber.From(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsExplicitlyConvertibleFromPrimitive()
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
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
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

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<LegacyPhoneNumber>(
                serialized,
                JsonOptions
            );

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesUninitializedToEmpty()
        {
            var container = new Container { Id = "one", Data = default };

            var serialized = JsonSerializer.Serialize(container, JsonOptions);

            Assert.Equal("{\"id\":\"one\",\"data\":null}", serialized);
        }

        [Fact]
        public void SerializesEmptyToEmpty()
        {
            var container = new Container { Id = "one", Data = LegacyPhoneNumber.Empty };

            var serialized = JsonSerializer.Serialize(container, JsonOptions);

            Assert.Equal("{\"id\":\"one\",\"data\":null}", serialized);
        }

        [Fact]
        public void DeserializesEmptyToEmpty()
        {
            var serialized = "{\"id\":\"one\",\"data\":null}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized, JsonOptions);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(LegacyPhoneNumber.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
        }

        [Fact]
        public void DeserializesMissingToEmpty()
        {
            var serialized = "{\"id\":\"one\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized, JsonOptions);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(LegacyPhoneNumber.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
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
        public void CanConvertFromPrimitive()
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
        public void CanConvertToPrimitive()
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

    [ValueObject<string>(
        fromPrimitiveCasting: CastOperator.Explicit,
        toPrimitiveCasting: CastOperator.Explicit,
        comparison: ComparisonGeneration.Omit
    )]
    public readonly partial record struct OtherLegacyPhoneNumber;
}
