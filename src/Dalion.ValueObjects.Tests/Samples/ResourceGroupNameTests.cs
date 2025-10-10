using System.Text.Json;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public partial class ResourceGroupNameTests
{
    public class Construction : ResourceGroupNameTests
    {
        // Does not compile, as intended, when the analyzer works correctly.
        /*[Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new ResourceGroupName();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }*/
    }

    public class From : ResourceGroupNameTests
    {
        [Fact]
        public void From_CreatesResourceGroupNameWithValue()
        {
            var actual = ResourceGroupName.From("theValue");
            Assert.Equal("theValue", actual.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" \t  ")]
        public void CanCreateEmpty(string? invalid)
        {
            var empty = ResourceGroupName.Empty;

            var actual = ResourceGroupName.From(invalid);

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
        public void CannotCreateInvalidResourceGroupName(string invalid)
        {
            Action act = () => ResourceGroupName.From(invalid);

            Assert.Throws<InvalidOperationException>(act);
        }
    }

    public class TryFrom : ResourceGroupNameTests
    {
        [Fact]
        public void TryFrom_CreatesResourceGroupNameWithValue()
        {
            var success = ResourceGroupName.TryFrom("theValue", out var actual);

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
            var success = ResourceGroupName.TryFrom(invalid, out _);

            Assert.False(success);
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
        public void CannotCreateInvalidResourceGroupName(string invalid)
        {
            var success = ResourceGroupName.TryFrom(invalid, out _);

            Assert.False(success);
        }
    }

    public class Value : ResourceGroupNameTests
    {
        [Fact]
        public void ReturnsUnderlyingValue()
        {
            var expected = "theValue";

            var actual = ResourceGroupName.From(expected);

            Assert.Equal(expected, actual.Value);
        }
    }

    public class Comparison : ResourceGroupNameTests
    {
        [Fact]
        public void CompareTo_WorksAsExpected()
        {
            var a = ResourceGroupName.From("aaa");
            var b = ResourceGroupName.From("bbb");
            var a2 = ResourceGroupName.From("aaa");

            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
            Assert.True(a.CompareTo(a2) == 0);
        }

        [Fact]
        public void CompareTo_IsCaseSensitive()
        {
            var a = ResourceGroupName.From("aaa");
            var A = ResourceGroupName.From("AAA");

            Assert.True(a.CompareTo(A) < 0);
            Assert.True(A.CompareTo(a) > 0);
        }
    }

    public class Equality : ResourceGroupNameTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var first = ResourceGroupName.From("abc123");
            var second = ResourceGroupName.From(first.Value);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var first = ResourceGroupName.From("abc123");
            var second = ResourceGroupName.From("xyz123");

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValuesAreDifferentlyCased_AreEqual()
        {
            var first = ResourceGroupName.From("abc123");
            var second = ResourceGroupName.From("aBc123");

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToDefault()
        {
            ResourceGroupName first = default;
            ResourceGroupName second = default;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToEmpty()
        {
            ResourceGroupName first = default;
            ResourceGroupName second = ResourceGroupName.Empty;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            ResourceGroupName first = ResourceGroupName.From("abc123");
            ResourceGroupName second = default;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotResourceGroupName_AreNotEqual()
        {
            var first = ResourceGroupName.From("abc123");
            var second = new object();

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void GivenOtherObjectIsAnotherValueType_AreNotEqual()
        {
            var first = ResourceGroupName.From("abc123");
            var second = OtherResourceGroupName.From(first.Value);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));
        }

        [Fact]
        public void HasEqualityMethodsForUnderlyingType()
        {
            var first = ResourceGroupName.From("abc123");
            var second = "abc123";

            Assert.True(first.Equals(second));

            var third = "xyz789";
            Assert.False(first.Equals(third));
        }

        [Fact]
        public void HasEqualityOperatorsForUnderlyingType()
        {
            var first = ResourceGroupName.From("abc123");
            var second = "abc123";

            Assert.True(first == second);
            Assert.True(second == first);
            Assert.False(first != second);
            Assert.False(second != first);

            var third = "xyz789";
            Assert.False(first == third);
            Assert.False(third == first);
            Assert.True(first != third);
            Assert.True(third != first);
        }
    }

    public class IsInitialized : ResourceGroupNameTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var sut = ResourceGroupName.From("abc123");

            Assert.True(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsDefault_IsFalse()
        {
            ResourceGroupName sut = default;

            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsEmpty_IsFalse()
        {
            ResourceGroupName sut = ResourceGroupName.Empty;

            Assert.False(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : ResourceGroupNameTests
    {
        [Fact]
        public void ReturnsValue()
        {
            var value = Guid.NewGuid().ToString();

            var actual = ResourceGroupName.From(value).ToString();

            Assert.Equal(value, actual);
        }
    }

    public class ConversionOperatorsForPrimitive : ResourceGroupNameTests
    {
        [Fact]
        public void IsImplicitlyConvertibleToPrimitive()
        {
            var value = Guid.NewGuid().ToString();
            var obj = ResourceGroupName.From(value);

            string actual = obj;

            Assert.Equal(value, actual);
        }
        
        [Fact]
        public void IsExplicitlyConvertibleToPrimitive()
        {
            var value = Guid.NewGuid().ToString();
            var obj = ResourceGroupName.From(value);

            var actual = (string)obj;

            Assert.Equal(value, actual);
        }

        [Fact]
        public void IsExplicitlyConvertibleFromPrimitive()
        {
            var value = Guid.NewGuid().ToString();
            var str = value;

            var actual = (ResourceGroupName)str;

            var expected = ResourceGroupName.From(value);
            Assert.Equal(expected, actual);
        }
    }

    public class Serialization : ResourceGroupNameTests
    {
        [Fact]
        public void WhenNonsense_ThrowsJsonException()
        {
            var nonsense = "{}";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ResourceGroupName>(nonsense));
        }

        [Fact]
        public void WhenEmptyString_CreatesEmpty()
        {
            var actual = JsonSerializer.Deserialize<ResourceGroupName>("\"\"");

            Assert.Equal(ResourceGroupName.Empty, actual);
        }

        [Fact]
        public void CanRoundTrip()
        {
            var original = ResourceGroupName.From("test-resource-group-name");

            var serialized = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<ResourceGroupName>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var sut = ResourceGroupName.From("test-resource-group-name");

            var serialized = JsonSerializer.Serialize(sut);

            Assert.Equal("\"test-resource-group-name\"", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            ResourceGroupName original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"\"", serialized);

            var deserialized = JsonSerializer.Deserialize<ResourceGroupName>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripEmpty()
        {
            var original = ResourceGroupName.Empty;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"\"", serialized);

            var deserialized = JsonSerializer.Deserialize<ResourceGroupName>(serialized);

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
                Data = ResourceGroupName.Empty
            };

            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":\"\"}", serialized);
        }

        [Fact]
        public void DeserializesEmptyToEmpty()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":\"\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(ResourceGroupName.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
        }

        [Fact]
        public void DeserializesMissingToEmpty()
        {
            var serialized = "{\"Id\":\"one\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(ResourceGroupName.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
        }

        internal class Container
        {
            public required string Id { get; set; }
            public ResourceGroupName Data { get; set; }
        }
    }

    public class TypeConversion : ResourceGroupNameTests
    {
        [Fact]
        public void CanConvertFromPrimitive()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(ResourceGroupName));
            Assert.True(converter.CanConvertFrom(typeof(string)));

            var actual = converter.ConvertFrom("test-resource-group-name");

            Assert.Equal(ResourceGroupName.From("test-resource-group-name"), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(ResourceGroupName));
            Assert.False(converter.CanConvertFrom(typeof(int)));

            Action act = () => converter.ConvertFrom(5);

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToPrimitive()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(ResourceGroupName));
            Assert.True(converter.CanConvertTo(typeof(string)));

            var sut = ResourceGroupName.From("test-resource-group-name");
            var actual = converter.ConvertTo(sut, typeof(string));

            Assert.Equal("test-resource-group-name", actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(ResourceGroupName));
            Assert.False(converter.CanConvertTo(typeof(int)));

            var sut = ResourceGroupName.From("test-resource-group-name");
            Action act = () => converter.ConvertTo(sut, typeof(int));

            Assert.Throws<NotSupportedException>(act);
        }
    }

    [ValueObject<string>(
        fromPrimitiveCasting: CastOperator.Explicit,
        toPrimitiveCasting: CastOperator.Explicit,
        comparison: ComparisonGeneration.UseUnderlying,
        stringCaseSensitivity: StringCaseSensitivity.CaseInsensitive
    )]
    public readonly partial record struct OtherResourceGroupName;
}