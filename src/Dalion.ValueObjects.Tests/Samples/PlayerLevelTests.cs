using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public partial class PlayerLevelTests
{
    public class Construction : PlayerLevelTests
    {
        // Does not compile, as intended, when the analyzer works correctly.
        /*[Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new PlayerLevel();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }*/
    }

    public class From : PlayerLevelTests
    {
        [Fact]
        public void From_CreatesPlayerLevelWithValue()
        {
            var backingValue = 3;

            var actual = PlayerLevel.From(backingValue);

            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CannotCreateUnspecified()
        {
            var backingValue = 0;

            Action act = () => PlayerLevel.From(backingValue);

            Assert.Throws<InvalidOperationException>(act);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-300)]
        public void CannotCreateInvalidInstance(int invalid)
        {
            Action act = () => PlayerLevel.From(invalid);

            Assert.Throws<InvalidOperationException>(act);
        }
    }

    public class TryFrom : PlayerLevelTests
    {
        [Fact]
        public void TryFrom_CreatesPlayerLevelWithValue()
        {
            var backingValue = 3;

            var success = PlayerLevel.TryFrom(backingValue, out var actual);

            Assert.True(success);
            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CannotCreateUnspecified()
        {
            var success = PlayerLevel.TryFrom(0, out _);

            Assert.False(success);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-300)]
        public void CannotCreateInvalidInstance(int invalid)
        {
            var success = PlayerLevel.TryFrom(invalid, out _);

            Assert.False(success);
        }
    }

    public class Value : PlayerLevelTests
    {
        [Fact]
        public void ReturnsUnderlyingValue()
        {
            var expected = 3;

            var actual = PlayerLevel.From(expected);

            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public void UnspecifiedReturnsExpectedUnderlyingValue()
        {
            var actual = PlayerLevel.Unspecified;

            Assert.Equal(0, actual.Value);
        }
    }

    public class Equality : PlayerLevelTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var backingValue = 3;
            var first = PlayerLevel.From(backingValue);
            var second = PlayerLevel.From(first.Value);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var backingValue1 = 3;
            var first = PlayerLevel.From(backingValue1);
            var backingValue2 = 4;
            var second = PlayerLevel.From(backingValue2);

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToDefault()
        {
            PlayerLevel first = default;
            PlayerLevel second = default;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsNotEqualToUnspecified()
        {
            PlayerLevel first = default;
            var second = PlayerLevel.Unspecified;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            var backingValue = 3;
            var first = PlayerLevel.From(backingValue);
            PlayerLevel second = default;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotPlayerLevel_AreNotEqual()
        {
            var backingValue = 3;
            var first = PlayerLevel.From(backingValue);
            var second = new object();

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void GivenOtherObjectIsAnotherValueType_AreNotEqual()
        {
            var backingValue = 3;
            var first = PlayerLevel.From(backingValue);
            var second = OtherPlayerLevel.From(backingValue);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));
        }

        [Fact]
        public void DoesNotHaveEqualityMethodsForUnderlyingType()
        {
            var backingValue = 3;
            var first = PlayerLevel.From(backingValue);
            var second = backingValue;

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));

            var third = 4;
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(third));
        }
    }

    public class IsInitialized : PlayerLevelTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var backingValue = 3;
            var sut = PlayerLevel.From(backingValue);

            Assert.True(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsDefault_IsFalse()
        {
            PlayerLevel sut = default;

            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void UnspecifiedIsInitialized()
        {
            var sut = PlayerLevel.Unspecified;

            Assert.True(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : PlayerLevelTests
    {
        [Fact]
        public void ReturnsValue()
        {
            var value = 3;

            var actual = PlayerLevel.From(value).ToString();

            Assert.Equal(value.ToString(CultureInfo.InvariantCulture), actual);
        }
    }

    public class Serialization : PlayerLevelTests
    {
        [Fact]
        public void WhenNonsense_ThrowsJsonException()
        {
            var nonsense = "\"nonsense\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PlayerLevel>(nonsense));
        }

        [Fact]
        public void WhenInvalid_CannotDeserialize()
        {
            var invalid = "-99999"; // lower than absolute zero

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PlayerLevel>(invalid));
        }

        [Fact]
        public void WhenEmptyString_ThrowsJsonException()
        {
            var nonsense = "\"\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PlayerLevel>(nonsense));
        }

        [Fact]
        public void CanRoundTrip()
        {
            var backingValue = 3;
            var original = PlayerLevel.From(backingValue);

            var serialized = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<PlayerLevel>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var backingValue = 3;
            var sut = PlayerLevel.From(backingValue);

            var serialized = JsonSerializer.Serialize(sut);

            Assert.Equal("3", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            PlayerLevel original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<PlayerLevel>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripUnspecified()
        {
            var original = PlayerLevel.Unspecified;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("0", serialized);

            var deserialized = JsonSerializer.Deserialize<PlayerLevel>(serialized);

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
        public void SerializesUnspecifiedToUnspecified()
        {
            var container = new Container { Id = "one", Data = PlayerLevel.Unspecified };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":0}", serialized);
        }

        [Fact]
        public void DeserializesUnspecifiedToUnspecified()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":0}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(PlayerLevel.Unspecified, deserialized.Data);
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
            Assert.NotEqual(PlayerLevel.Unspecified, deserialized.Data);

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
            Assert.NotEqual(PlayerLevel.Unspecified, deserialized.Data);

            Assert.False(deserialized.Data.IsInitialized());
        }

        internal class Container
        {
            public required string Id { get; set; }
            public PlayerLevel Data { get; set; }
        }
    }

    public class TypeConversion : PlayerLevelTests
    {
        [Fact]
        public void CanConvertFromUnderlyingType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(PlayerLevel));
            Assert.True(converter.CanConvertFrom(typeof(int)));

            var backingValue = 3;
            var actual = converter.ConvertFrom(backingValue);

            Assert.Equal(PlayerLevel.From(backingValue), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(PlayerLevel));
            Assert.False(converter.CanConvertFrom(typeof(string)));

            Action act = () => converter.ConvertFrom("abc");

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToUnderlyingType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(PlayerLevel));
            Assert.True(converter.CanConvertTo(typeof(int)));

            var backingValue = 3;
            var sut = PlayerLevel.From(backingValue);
            var actual = converter.ConvertTo(sut, typeof(int));

            Assert.Equal(backingValue, actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = TypeDescriptor.GetConverter(typeof(PlayerLevel));
            Assert.False(converter.CanConvertTo(typeof(string)));

            var backingValue = 3;
            var sut = PlayerLevel.From(backingValue);
            Action act = () => converter.ConvertTo(sut, typeof(string));

            Assert.Throws<NotSupportedException>(act);
        }
    }

    public class IsValid : PlayerLevelTests
    {
        [Fact]
        public void ValidInstanceIsValid()
        {
            var backingValue = 3;
            var sut = PlayerLevel.From(backingValue);

            Assert.True(sut.IsValid());
        }

        [Fact]
        public void UnspecifiedIsInvalid()
        {
            var sut = PlayerLevel.Unspecified;

            Assert.False(sut.IsValid());
        }
    }

    public class GetValidationErrorMessage : PlayerLevelTests
    {
        [Fact]
        public void WhenValidReturnsNull()
        {
            var backingValue = 3;
            var sut = PlayerLevel.From(backingValue);

            Assert.Null(sut.GetValidationErrorMessage());
        }

        [Fact]
        public void WhenInvalidReturnsMessage()
        {
            var sut = PlayerLevel.Unspecified;

            Assert.Equal("Player level must be specified.", sut.GetValidationErrorMessage());
        }
    }

    public class FluentValidationExtensions : PlayerLevelTests
    {
        public class MustBeInitializedAndValid : FluentValidationExtensions
        {
            private readonly Func<ValidationResult> _act;
            private PlayerLevel _vo;

            public MustBeInitializedAndValid()
            {
                _vo = PlayerLevel.From(1);
                _act = () =>
                    new ContainerValidator().Validate(new Container { Id = "one", Data = _vo });
            }

            [Fact]
            public void WhenValid_ReturnsValid()
            {
                _vo = PlayerLevel.From(3);

                var result = _act();

                Assert.True(result.IsValid);
            }

            [Fact]
            public void WhenInvalid_ReturnsInvalid()
            {
                _vo = PlayerLevel.Unspecified;

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
                _vo = PlayerLevel.Unspecified;

                var result = _act();

                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
                Assert.Equal("Player level must be specified.", result.Errors[0].ErrorMessage);
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
            private PlayerLevel _vo;

            public MustBeInitialized()
            {
                _vo = PlayerLevel.From(1);
                _act = () =>
                    new ContainerValidator().Validate(new Container { Id = "one", Data = _vo });
            }

            [Fact]
            public void WhenValid_ReturnsValid()
            {
                _vo = PlayerLevel.From(3);

                var result = _act();

                Assert.True(result.IsValid);
            }

            [Fact]
            public void WhenInvalid_ReturnsValid()
            {
                _vo = PlayerLevel.Unspecified;

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
                Assert.Equal("PlayerLevel must be initialized.", result.Errors[0].ErrorMessage);
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
            public PlayerLevel Data { get; set; }
        }
    }

    [ValueObject<decimal>(ComparisonGeneration.Omit, emptyValueName: "Unspecified")]
    public readonly partial record struct OtherPlayerLevel;
}
