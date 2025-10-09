
#nullable disable

namespace Dalion.ValueObjects.Samples {
    public enum ComparisonGeneration {
        Omit = 0,
        UseUnderlying = 1
    }

    public enum CastOperator {
        None = 0,
        Explicit = 1,
        Implicit = 2
    }

    public enum StringCaseSensitivity {
        CaseSensitive = 0,
        CaseInsensitive = 1
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class ValueObjectAttribute<T> : ValueObjectAttribute {
        public ValueObjectAttribute(
            ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
            CastOperator toPrimitiveCasting = CastOperator.None,
            CastOperator fromPrimitiveCasting = CastOperator.None,
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive
        )
            : base(
                typeof(T),
                comparison,
                toPrimitiveCasting,
                fromPrimitiveCasting,
                stringCaseSensitivity
            ) { }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class ValueObjectAttribute : System.Attribute {
        public ValueObjectAttribute(
            Type underlyingType = null,
            ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
            CastOperator toPrimitiveCasting = CastOperator.None,
            CastOperator fromPrimitiveCasting = CastOperator.None,
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive
        ) { }
    }
}
