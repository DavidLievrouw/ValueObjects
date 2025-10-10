using System.Text.Json;
using Xunit;

namespace Dalion.ValueObjects.Samples;

public partial class TenantIdTests
{
    public class Construction : TenantIdTests
    {
        // Does not compile, as intended, when the analyzer works correctly.
        /*[Fact]
        public void NotAllowedToNewUp()
        {
            var actual = new TenantId();
            Assert.Fail("Should not be allowed to new up, but got: " + actual);
        }*/
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
            var actual = TenantId.From(Guid.Empty);

            var empty = TenantId.Empty;
            Assert.True(actual.Equals(empty));
            Assert.True(actual == empty);
            Assert.False(actual != empty);
            Assert.Equal(actual.GetHashCode(), empty.GetHashCode());
            
            Assert.True(actual.IsInitialized());
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
        public void CanCreateEmpty()
        {
            var success = TenantId.TryFrom(Guid.Empty, out var actual);
            
            Assert.True(success);
            
            var empty = TenantId.Empty;
            Assert.True(actual.Equals(empty));
            Assert.True(actual == empty);
            Assert.False(actual != empty);
            Assert.Equal(actual.GetHashCode(), empty.GetHashCode());
            
            Assert.True(actual.IsInitialized());
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

        [Fact]
        public void EmptyReturnsExpectedUnderlyingValue()
        {
            var actual = TenantId.Empty;

            Assert.Equal(Guid.Empty, actual.Value);
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
        public void WhenValueIsDefault_IsNotEqualToEmpty()
        {
            TenantId first = default;
            var second = TenantId.Empty;

            Assert.False(first.Equals(second));
            Assert.False(first == second);
            Assert.True(first != second);
        }

        [Fact]
        public void WhenValueIsNotDefault_IsNotEqualToDefault()
        {
            var backingValue = Guid.NewGuid();
            var first = TenantId.From(backingValue);
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

        [Fact]
        public void GivenOtherObjectIsAnotherValueType_AreNotEqual()
        {
            var backingValue = Guid.NewGuid();
            var first = TenantId.From(backingValue);
            var second = OtherTenantId.From(backingValue);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(first.Equals(second));
        }

        [Fact]
        public void HasEqualityMethodsForUnderlyingType()
        {
            var backingValue = Guid.NewGuid();
            var first = TenantId.From(backingValue);
            var second = backingValue;

            Assert.True(first.Equals(second));
            
            var third = Guid.NewGuid();
            Assert.False(first.Equals(third));
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
        public void WhenValueIsEmpty_IsTrue()
        {
            var sut = TenantId.Empty;
            
            Assert.True(sut.IsInitialized());
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

    public class ConversionOperatorsForUnderlyingType : TenantIdTests
    {
        [Fact]
        public void IsExplicitlyConvertibleFromUnderlyingType()
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
        public void WhenNonsense_ThrowsJsonException()
        {
            var nonsense = "\"nonsense\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TenantId>(nonsense));
        }
        
        [Fact]
        public void WhenEmptyString_ThrowsJsonException()
        {
            var nonsense = "\"\"";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TenantId>(nonsense));
        }

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

            Assert.Equal($"\"{backingValue.ToString()}\"", serialized);
        }

        [Fact]
        public void CanRoundTripDefault()
        {
            TenantId original = default;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("null", serialized);

            var deserialized = JsonSerializer.Deserialize<TenantId>(serialized);

            Assert.Equal(original, deserialized);
        }

        [Fact]
        public void CanRoundTripEmpty()
        {
            var original = TenantId.Empty;

            var serialized = JsonSerializer.Serialize(original);

            Assert.Equal("\"00000000-0000-0000-0000-000000000000\"", serialized);

            var deserialized = JsonSerializer.Deserialize<TenantId>(serialized);

            Assert.Equal(original, deserialized);
            Assert.True(deserialized.IsInitialized());
        }

        [Fact]
        public void SerializesUninitializedToNull()
        {
            var container = new Container
            {
                Id = "one",
                Data = default
            };
            
            var serialized = JsonSerializer.Serialize(container);

            Assert.Equal("{\"Id\":\"one\",\"Data\":null}", serialized);
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

            Assert.Equal("{\"Id\":\"one\",\"Data\":\"00000000-0000-0000-0000-000000000000\"}", serialized);
        }

        [Fact]
        public void DeserializesEmptyToEmpty()
        {
            var serialized = "{\"Id\":\"one\",\"Data\":\"00000000-0000-0000-0000-000000000000\"}";

            var deserialized = JsonSerializer.Deserialize<Container>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal("one", deserialized.Id);
            Assert.Equal(TenantId.Empty, deserialized.Data);
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
            Assert.NotEqual(TenantId.Empty, deserialized.Data);
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
            Assert.NotEqual(TenantId.Empty, deserialized.Data);
            Assert.Equal(default, deserialized.Data);
            
            Assert.False(deserialized.Data.IsInitialized());
        }
        
        internal class Container
        {
            public required string Id { get; set; }
            public TenantId Data { get; set; }
        }
    }
    
    public class TypeConversion : TenantIdTests
    {
        [Fact]
        public void CanConvertFromUnderlyingType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TenantId));
            Assert.True(converter.CanConvertFrom(typeof(Guid)));

            var backingValue = Guid.NewGuid();
            var actual = converter.ConvertFrom(backingValue);

            Assert.Equal(TenantId.From(backingValue), actual);
        }

        [Fact]
        public void CannotConvertFromUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TenantId));
            Assert.False(converter.CanConvertFrom(typeof(int)));

            Action act = () => converter.ConvertFrom(5);

            Assert.Throws<NotSupportedException>(act);
        }

        [Fact]
        public void CanConvertToUnderlyingType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TenantId));
            Assert.True(converter.CanConvertTo(typeof(Guid)));

            var backingValue = Guid.NewGuid();
            var sut = TenantId.From(backingValue);
            var actual = converter.ConvertTo(sut, typeof(Guid));

            Assert.Equal(backingValue, actual);
        }

        [Fact]
        public void CannotConvertToUnsupportedType()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TenantId));
            Assert.False(converter.CanConvertTo(typeof(int)));

            var backingValue = Guid.NewGuid();
            var sut = TenantId.From(backingValue);
            Action act = () => converter.ConvertTo(sut, typeof(int));

            Assert.Throws<NotSupportedException>(act);
        }
    }
    
    public class IsValid : TenantIdTests
    {
        [Fact]
        public void ValidInstanceIsValid()
        {
            var backingValue = Guid.NewGuid();
            var sut = TenantId.From(backingValue);

            Assert.True(sut.IsValid());
        }

        [Fact]
        public void EmptyIsValid()
        {
            var sut = TenantId.Empty;

            Assert.True(sut.IsValid());
        }
    }

    public class GetValidationErrorMessage : TenantIdTests
    {
        [Fact]
        public void WhenValidReturnsNull()
        {
            var backingValue = Guid.NewGuid();
            var sut = TenantId.From(backingValue);

            Assert.Null(sut.GetValidationErrorMessage());
        }
    }
    
    [ValueObject<Guid>(
        fromUnderlyingTypeCasting: CastOperator.Explicit,
        toUnderlyingTypeCasting: CastOperator.None,
        comparison: ComparisonGeneration.Omit,
        underlyingTypeEqualityGeneration: UnderlyingTypeEqualityGeneration.GenerateMethods
    )]
    public readonly partial record struct OtherTenantId;
}