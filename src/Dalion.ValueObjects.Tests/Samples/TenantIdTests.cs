using System.Text.Json;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public class TenantIdTests
{
    public class Construction : PasswordTests
    {
        [Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new TenantId();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }
    }
    
    public class From : TenantIdTests
    {
        [Fact]
        public void From_CreatesTenantIdWithValue()
        {
            var backingValue = Guid.NewGuid();
            var actual = TenantId.From(backingValue);
            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CanCreateEmpty()
        {
            var empty = TenantId.Empty;
            
            var actual = TenantId.From(Guid.Empty);

            Assert.True(actual.Equals(empty));
            Assert.True(actual == empty);
            Assert.False(actual != empty);
            Assert.Equal(actual.GetHashCode(), empty.GetHashCode());
        }
    } 
    
    public class TryFrom : TenantIdTests
    {
        [Fact]
        public void TryFrom_CreatesTenantIdWithValue()
        {
            var backingValue = Guid.NewGuid();
            
            var success = TenantId.TryFrom(backingValue, out var actual);
            
            Assert.True(success);
            Assert.Equal(backingValue, actual.Value);
        }

        [Fact]
        public void CannotCreateEmpty()
        {
            var success = TenantId.TryFrom(Guid.Empty, out _);
            
            Assert.False(success);
        }
    }

    public class Value : TenantIdTests
    {
        [Fact]
        public void ReturnsUnderlyingValue()
        {
            var expected = Guid.NewGuid();
            
            var actual = TenantId.From(expected);
            
            Assert.Equal(expected, actual.Value);
        }
    }

    public class Equality : TenantIdTests
    {
        [Fact]
        public void WhenValuesAreEqual_AreEqual()
        {
            var backingValue = Guid.NewGuid();
            var first = TenantId.From(backingValue);
            var second = TenantId.From(first.Value);

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValuesAreDifferent_AreNotEqual()
        {
            var backingValue1 = Guid.NewGuid();
            var first = TenantId.From(backingValue1);
            var backingValue2 = Guid.NewGuid();
            var second = TenantId.From(backingValue2);

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToDefault()
        {
            TenantId first = default;
            TenantId second = default;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsDefault_IsEqualToEmpty()
        {
            TenantId first = default;
            TenantId second = TenantId.Empty;

            Assert.True(first.Equals(second));
            Assert.True(first == second);
            Assert.False(first != second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            var backingValue = Guid.NewGuid();
            TenantId first = TenantId.From(backingValue);
            TenantId second = default;
            
            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void GivenOtherObjectIsNotTenantId_AreNotEqual()
        {
            var backingValue = Guid.NewGuid();
            var first = TenantId.From(backingValue);
            var second = new object();

            Assert.False(first.Equals(second));
        }
    }
    
    public class IsInitialized : TenantIdTests
    {
        [Fact]
        public void WhenValueIsNotDefault_IsTrue()
        {
            var backingValue = Guid.NewGuid();
            var sut = TenantId.From(backingValue);
            
            Assert.True(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsDefault_IsFalse()
        {
            TenantId sut = default;
            
            Assert.False(sut.IsInitialized());
        }

        [Fact]
        public void WhenValueIsEmpty_IsFalse()
        {
            TenantId sut = TenantId.Empty;
            
            Assert.False(sut.IsInitialized());
        }
    }

    public class ToStringRepresentation : TenantIdTests
    {
        [Fact]
        public void ReturnsValue()
        {
            var value = Guid.NewGuid();
            
            var actual = TenantId.From(value).ToString();
            
            Assert.Equal(value.ToString(), actual);
        }
    }

    public class ConversionOperatorsForGuid : TenantIdTests
    {
        [Fact]
        public void IsExplicitlyConvertibleFromString()
        {
            var value = Guid.NewGuid();
            
            var actual = (TenantId)value;
            
            var expected = TenantId.From(value);
            Assert.Equal(expected, actual);
        }
    }

    public class Serialization : TenantIdTests
    {
        [Fact]
        public void CanRoundTrip()
        {
            var backingValue = Guid.NewGuid();
            var original = TenantId.From(backingValue);

            var serialized = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<TenantId>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void SerializesToCorrectJson()
        {
            var backingValue = Guid.NewGuid();
            var sut = TenantId.From(backingValue);

            var serialized = JsonSerializer.Serialize(sut);

            Assert.Equal("\"test-resource-group-name\"", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            TenantId original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"\"", serialized);

            var deserialized = JsonSerializer.Deserialize<TenantId>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripEmpty()
        {
            var original = TenantId.Empty;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"\"", serialized);

            var deserialized = JsonSerializer.Deserialize<TenantId>(serialized);

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
                Data = TenantId.Empty
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
            Assert.Equal(TenantId.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
        }

        [Fact]
        public void DeserializesMissingToEmpty()
        {
            var serialized = "{\"Id\":\"one\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(TenantId.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
        }
        
        internal class Container
        {
            public required string Id { get; set; }
            public TenantId Data { get; set; }
        }
    }
}