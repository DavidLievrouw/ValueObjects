using System.Text.Json;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public class PasswordTests
{
    public class From : PasswordTests
    {
        [Fact]
        public void From_CreatesPasswordWithValue()
        {
            var actual = Password.From("theValue");
            Assert.Equal("theValue", actual.Value);
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
        [InlineData("a")] // too short
        [InlineData("ab")] // too short
        [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijkl")] // too long
        [InlineData("-abc")] // starts with invalid character
        [InlineData(".abc")] // starts with invalid character
        [InlineData("_abc")] // starts with invalid character
        [InlineData("abc-")] // ends with invalid character
        [InlineData("abc.")] // ends with invalid character
        [InlineData("abc_")] // ends with invalid character
        public void CannotCreateInvalidPassword(string invalid)
        {
            Action act = () => Password.From(invalid);

            Assert.Throws<ArgumentException>(act);
        }
    } 
    
    public class TryFrom : PasswordTests
    {
        [Fact]
        public void TryFrom_CreatesPasswordWithValue()
        {
            var success = Password.TryFrom("theValue", out var actual);
            
            Assert.True(success);
            Assert.Equal("theValue", actual.Value);
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
        [InlineData("a")] // too short
        [InlineData("ab")] // too short
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
            var expected = "theValue";
            
            var actual = Password.From(expected);
            
            Assert.Equal(expected, actual.Value);
        }
    }

    public class Equality : PasswordTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var first = Password.From("abc123");
            var second = Password.From(first);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var first = Password.From("abc123");
            var second = Password.From("xyz123");

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValuesAreDifferentlyCased_AreNotEqual()
        {
            var first = Password.From("abc123");
            var second = Password.From("aBc123");

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
            Password first = Password.From("abc123");
            Password second = default;
            
            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotPassword_AreNotEqual()
        {
            var first = Password.From("abc123");
            var second = new object();

            Assert.False(first.Equals(second));
        }
    }
    
    public class IsInitialized : PasswordTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var sut = Password.From("abc123");
            
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
            var value = Guid.NewGuid().ToString();
            
            var actual = Password.From(value).ToString();
            
            Assert.Equal(value, actual);
        }
    }

    public class ConversionOperatorsForString : PasswordTests
    {
        [Fact]
        public void IsExplicitlyConvertibleToString()
        {
            var value = Guid.NewGuid().ToString();
            var obj = Password.From(value);
            
            var actual = (string)obj;
            
            Assert.Equal(value, actual);
        }
    }

    public class Serialization : PasswordTests
    {
        [Fact]
        public void CanRoundTrip()
        {
            var original = Password.From("test-pwd");

            var serialized = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<Password>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var sut = Password.From("test-pwd");

            var serialized = JsonSerializer.Serialize(sut);

            Assert.Equal("\"test-pwd\"", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            Password original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"\"", serialized);

            var deserialized = JsonSerializer.Deserialize<Password>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripEmpty()
        {
            var original = Password.Empty;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"\"", serialized);

            var deserialized = JsonSerializer.Deserialize<Password>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesUninitializedToEmpty()
        {
            var container = new Container
            {
                Id = "one",
                Data = default
            };
            
            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":\"\"}", serialized);
        }

        [Fact]
        public void SerializesEmptyToEmpty()
        {
            var container = new Container
            {
                Id = "one",
                Data = Password.Empty
            };
            
            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":\"\"}", serialized);
        }

        [Fact]
        public void DeserializesMissingToEmpty()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":\"\"}";

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
}