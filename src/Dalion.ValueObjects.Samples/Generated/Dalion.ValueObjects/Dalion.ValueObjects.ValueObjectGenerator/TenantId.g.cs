
        #nullable enable

        namespace Dalion.ValueObjects.Samples {
            [System.Diagnostics.DebuggerDisplay("TenantId {Value}")]
            public readonly partial record struct TenantId : IEquatable<TenantId>,
               IEquatable<System.Guid> {
                private readonly System.Guid _value;

                public System.Guid Value => _value;

                
                [System.Diagnostics.DebuggerStepThrough]
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public TenantId()
                {
                    _value = default;
                }

                private TenantId(System.Guid value, bool validation = true) {
                    if (validation) {
                        
                    }
                    _value = value;
                }

                public static TenantId From(System.Guid value) {
                    if (value == default) {
                        return Empty;
                    }

                    return new TenantId(value);
                }

                public static bool TryFrom(System.Guid value, out TenantId result) {
                    result = value == default ? Empty : new TenantId(value, validation: false);
                    return result.IsInitialized();
                }


                public static TenantId Empty => new TenantId(default, validation: false);

                public bool IsInitialized() => _value != default;

                
                /// <inheritdoc />
                public bool Equals(TenantId? other)
                {
                    if (other == null) return false;

                    if (!other.Value.IsInitialized())
                    {
                        return !IsInitialized();
                    }

                    if (other.Value.IsInitialized() != IsInitialized())
                    {
                        return false;
                    }
            
                    return EqualityComparer<System.Guid>.Default.Equals(this._value, other.Value.Value);
                }

                /// <inheritdoc />
                public bool Equals(TenantId other)
                {
                    if (!other.IsInitialized())
                    {
                        return !IsInitialized();
                    }

                    if (other.IsInitialized() != IsInitialized())
                    {
                        return false;
                    }
            
                    return EqualityComparer<System.Guid>.Default.Equals(this._value, other.Value);
                }
            
                /// <inheritdoc />
                public bool Equals(System.Guid other)
                {
                    return EqualityComparer<System.Guid>.Default.Equals(this._value, other);
                }
            
                public bool Equals(TenantId? other, IEqualityComparer<TenantId> comparer)
                {
                    if (other == null) return false;
                    return comparer.Equals(this, other.Value);
                }
            
                /// <inheritdoc />
                public override int GetHashCode() {
                    if (!IsInitialized()) return 0;
                    return EqualityComparer<System.Guid>.Default.GetHashCode(this._value);
                }

                
    public static bool operator ==(TenantId left, System.Guid right) => left.Value.Equals(right);

    public static bool operator ==(System.Guid left, TenantId right) => right.Value.Equals(left);

    public static bool operator !=(System.Guid left, TenantId right) => !(left == right);

    public static bool operator !=(TenantId left, System.Guid right) => !(left == right);


                

                

                
                /// <summary>
                ///     An explicit conversion from <see cref="System.Guid" /> to <see cref="TenantId" />.
                /// </summary>
                /// <param name="value">The value to convert.</param>
                /// <returns>The <see cref="TenantId" /> instance created from the input value.</returns>
                public static explicit operator TenantId(System.Guid value)
                {
                    return TenantId.From(value);
                }

                
                /// <inheritdoc />
                public override string ToString()
                {
                    return Value.ToString() ?? "";
                }

                /// <inheritdoc cref="M:System.String.ToString(System.IFormatProvider)" />
                public string ToString(IFormatProvider? provider)
                {
                    return Value.ToString(format: null, provider: provider) ?? "";
                }


                
private class Validation
{
    public static readonly Validation Ok = new(string.Empty);

    private Validation(string reason)
    {
        ErrorMessage = reason;
    }

    public string ErrorMessage { get; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public Dictionary<object, object>? Data { get; private set; }

    public static Validation Invalid(string reason = "")
    {
        if (string.IsNullOrEmpty(reason))
        {
            return new Validation("[none provided]");
        }

        return new Validation(reason);
    }

    public Validation WithData(object key, object value)
    {
        Data ??= new Dictionary<object, object>();
        Data[key] = value;
        return this;
    }
}
private class ValueObjectValidationException : Exception
{
    private const string DefaultMessage = "Validation of the value object failed.";

    public ValueObjectValidationException()
        : base(DefaultMessage) { }

    public ValueObjectValidationException(string message)
        : base(message) { }

    public ValueObjectValidationException(Exception innerException)
        : base(DefaultMessage, innerException) { }

    public ValueObjectValidationException(string message, Exception innerException)
        : base(message, innerException) { }
}
            }
        }
        