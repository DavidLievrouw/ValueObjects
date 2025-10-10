using System.Text.Json;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public partial class PasswordTests
{
    public class Construction : PasswordTests
    {
        // Does not compile, as intended, when the analyzer works correctly.
        /*[Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new Password();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }*/
    }

    public class From : PasswordTests
    {
        [Fact]
        public void From_CreatesPasswordWithValue()
        {
            var actual = Password.From("test-Pwd2");
            Assert.Equal("test-Pwd2", actual.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" \t  ")]
        public void CanCreateEmpty(string? invalid)
        {
            var empty = Password.Empty;

            var actual = Password.From(invalid);

            Assert.True(actual.Equals(empty));
            Assert.True(actual == empty);
            Assert.False(actual != empty);
            Assert.Equal(actual.GetHashCode(), empty.GetHashCode());
        }

        [Theory]
        [InlineData("shoRt1!")] // Too short
        [InlineData("alllowercase1!")] // No uppercase
        [InlineData("ALLUPPERCASE1!")] // No lowercase
        [InlineData("NoDigits!")] // No digit
        [InlineData("NoSpecial1")] // No special character
        [InlineData("Password1")] // No special character
        [InlineData("Password!")] // No digit
        public void CannotCreateInvalidPassword(string invalid)
        {
            Action act = () => Password.From(invalid);

            Assert.Throws<InvalidOperationException>(act);
        }
    }

    public class TryFrom : PasswordTests
    {
        [Fact]
        public void TryFrom_CreatesPasswordWithValue()
        {
            var success = Password.TryFrom("test-Pwd2", out var actual);

            Assert.True(success);
            Assert.Equal("test-Pwd2", actual.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" \t  ")]
        public void CannotCreateNullEmptyOrWhitespace(string? invalid)
        {
            var success = Password.TryFrom(invalid, out _);

            Assert.False(success);
        }

        [Theory]
        [InlineData("shoRt1!")] // Too short
        [InlineData("alllowercase1!")] // No uppercase
        [InlineData("ALLUPPERCASE1!")] // No lowercase
        [InlineData("NoDigits!")] // No digit
        [InlineData("NoSpecial1")] // No special character
        [InlineData("Password1")] // No special character
        [InlineData("Password!")] // No digit
        public void CannotCreateInvalidPassword(string invalid)
        {
            var success = Password.TryFrom(invalid, out _);

            Assert.False(success);
        }
    }

    public class Value : PasswordTests
    {
        [Fact]
        public void ReturnsUnderlyingValue()
        {
            var expected = "test-Pwd2";

            var actual = Password.From(expected);

            Assert.Equal(expected, actual.Value);
        }
    }

    public class Equality : PasswordTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var first = Password.From("test-Pwd2");
            var second = Password.From(first.Value);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var first = Password.From("test-Pwd2");
            var second = Password.From("other-Pwd4");

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValuesAreDifferentlyCased_AreNotEqual()
        {
            var first = Password.From("test-Pwd2");
            var second = Password.From("test-PWD2");

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToDefault()
        {
            Password first = default;
            Password second = default;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToEmpty()
        {
            Password first = default;
            Password second = Password.Empty;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            Password first = Password.From("test-Pwd2");
            Password second = default;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotPassword_AreNotEqual()
        {
            var first = Password.From("test-Pwd2");
            var second = new object();

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void GivenOtherObjectIsAnotherValueType_AreNotEqual()
        {
            var first = Password.From("test-Pwd2");
            var second = OtherPassword.From(first.Value);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));
        }
    }

    public class IsInitialized : PasswordTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var sut = Password.From("test-Pwd2");

            Assert.True(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsDefault_IsFalse()
        {
            Password sut = default;

            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsEmpty_IsFalse()
        {
            Password sut = Password.Empty;

            Assert.False(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : PasswordTests
    {
        [Fact]
        public void ReturnsValue()
        {
            var value = "test-Pwd2";

            var actual = Password.From(value).ToString();

            Assert.Equal(value, actual);
        }
    }

    public class ConversionOperatorsForPrimitive : PasswordTests
    {
        [Fact]
        public void IsExplicitlyConvertibleToPrimitive()
        {
            var value = "test-Pwd2";
            var obj = Password.From(value);

            var actual = (string)obj;

            Assert.Equal(value, actual);
        }
    }

    public class Serialization : PasswordTests
    {
        [Fact]
        public void WhenNonsense_ThrowsJsonException()
        {
            var nonsense = "{}";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Password>(nonsense));
        }

        [Fact]
        public void WhenEmptyString_CreatesEmpty()
        {
            var actual = JsonSerializer.Deserialize<Password>("\"\"");
            
            Assert.Equal(Password.Empty, actual);
        }

        [Fact]
        public void CanRoundTrip()
        {
            var original = Password.From("test-Pwd2");

            var serialized = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<Password>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var sut = Password.From("test-Pwd2");

            var serialized = JsonSerializer.Serialize(sut);

            Assert.Equal("\"test-Pwd2\"", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            Password original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<Password>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripEmpty()
        {
            var original = Password.Empty;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<Password>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesUninitializedToEmpty()
        {
            var container = new Container { Id = "one", Data = default };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":null}", serialized);
        }

        [Fact]
        public void SerializesEmptyToEmpty()
        {
            var container = new Container { Id = "one", Data = Password.Empty };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":null}", serialized);
        }

        [Fact]
        public void DeserializesEmptyToEmpty()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":null}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(Password.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
        }

        [Fact]
        public void DeserializesMissingToEmpty()
        {
            var serialized = "{\"Id\":\"one\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(Password.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
        }

        internal class Container
        {
            public required string Id { get; set; }
            public Password Data { get; set; }
        }
    }

    public class TypeConversion : PasswordTests
    {
        [Fact]
        public void CanConvertFromPrimitive()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Password));
            Assert.True(converter.CanConvertFrom(typeof(string)));

            var actual = converter.ConvertFrom("test-Pwd2");

            Assert.Equal(Password.From("test-Pwd2"), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Password));
            Assert.False(converter.CanConvertFrom(typeof(int)));

            Action act = () => converter.ConvertFrom(5);

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToPrimitive()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Password));
            Assert.True(converter.CanConvertTo(typeof(string)));

            var sut = Password.From("test-Pwd2");
            var actual = converter.ConvertTo(sut, typeof(string));

            Assert.Equal("test-Pwd2", actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Password));
            Assert.False(converter.CanConvertTo(typeof(int)));

            var sut = Password.From("test-Pwd2");
            Action act = () => converter.ConvertTo(sut, typeof(int));

            Assert.Throws<NotSupportedException>(act);
        }
    }

    [ValueObject<string>(
        fromPrimitiveCasting: CastOperator.None,
        toPrimitiveCasting: CastOperator.Explicit,
        comparison: ComparisonGeneration.Omit,
        stringCaseSensitivity: StringCaseSensitivity.CaseSensitive,
        primitiveEqualityGeneration: PrimitiveEqualityGeneration.Omit
    )]
    public readonly partial record struct OtherPassword;
}