
#nullable disable

namespace Dalion.ValueObjects.Samples {
    [System.Flags]
    public enum UnderlyingTypeEqualityGeneration {
        Omit = 0,
        GenerateOperators = 1 << 0,
        GenerateMethods = 1 << 1,
        GenerateOperatorsAndMethods = GenerateOperators | GenerateMethods
    }

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
            CastOperator toUnderlyingTypeCasting = CastOperator.None,
            CastOperator fromUnderlyingTypeCasting = CastOperator.None,
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
            UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration = UnderlyingTypeEqualityGeneration.GenerateOperators,
            string emptyValueName = "Empty"
        )
            : base(
                typeof(T),
                comparison,
                toUnderlyingTypeCasting,
                fromUnderlyingTypeCasting,
                stringCaseSensitivity,
                underlyingTypeEqualityGeneration,
                emptyValueName
            ) { }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class ValueObjectAttribute : System.Attribute {
        public ValueObjectAttribute(
            Type underlyingType = null,
            ComparisonGeneration comparison = ComparisonGeneration.UseUnderlying,
            CastOperator toUnderlyingTypeCasting = CastOperator.None,
            CastOperator fromUnderlyingTypeCasting = CastOperator.None,
            StringCaseSensitivity stringCaseSensitivity = StringCaseSensitivity.CaseSensitive,
            UnderlyingTypeEqualityGeneration underlyingTypeEqualityGeneration = UnderlyingTypeEqualityGeneration.GenerateOperators,
            string emptyValueName = "Empty"
        ) { }
    }
}
