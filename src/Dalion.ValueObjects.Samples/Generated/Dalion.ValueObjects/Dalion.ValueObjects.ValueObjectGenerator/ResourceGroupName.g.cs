
        #nullable enable

        namespace Dalion.ValueObjects.Samples {
            [System.Diagnostics.DebuggerDisplay("ResourceGroupName {Value}")]
            public readonly partial record struct ResourceGroupName : IEquatable<ResourceGroupName>,
                                                                IEquatable<System.String>,
                                                                IComparable<ResourceGroupName>,
                                                                IComparable {
                private readonly System.String _value;

                public System.String Value => _value;

                
                private ResourceGroupName(System.String value, bool validation = true) {
                    if (validation) {
                        
                  var validationResult = Validate(value);
                  if (!validationResult.IsSuccess) {
                      throw new System.InvalidOperationException(validationResult.ErrorMessage);
                  }
                    }
                    _value = value ?? System.String.Empty;
                }

                public static ResourceGroupName From(System.String? value) {
                    if (string.IsNullOrWhiteSpace(value)) {
                        return Empty;
                    }

                    return new ResourceGroupName(value);
                }

                public static bool TryFrom(System.String? value, out ResourceGroupName result) {
                    result = string.IsNullOrWhiteSpace(value) ? Empty : new ResourceGroupName(value, validation: false);
                    return result.IsInitialized() && Validate(result._value).IsSuccess;
                }


                public static ResourceGroupName Empty => new ResourceGroupName(System.String.Empty, validation: false);

                public bool IsInitialized() => !System.String.IsNullOrWhiteSpace(_value);

                
                /// <inheritdoc />
                public bool Equals(ResourceGroupName? other)
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
            
                    return System.String.IsNullOrWhiteSpace(other.Value.Value)
                        ? System.String.IsNullOrWhiteSpace(this._value)
                        : System.String.Equals(this._value, other.Value.Value, System.StringComparison.OrdinalIgnoreCase);
                }

                /// <inheritdoc />
                public bool Equals(ResourceGroupName other)
                {
                    if (!other.IsInitialized())
                    {
                        return !IsInitialized();
                    }

                    if (other.IsInitialized() != IsInitialized())
                    {
                        return false;
                    }
            
                    return System.String.IsNullOrWhiteSpace(other.Value)
                        ? System.String.IsNullOrWhiteSpace(this._value)
                        : System.String.Equals(this._value, other.Value, System.StringComparison.OrdinalIgnoreCase);
                }
            
                /// <inheritdoc />
                public bool Equals(System.String? other)
                {
                    return System.String.IsNullOrWhiteSpace(other)
                        ? System.String.IsNullOrWhiteSpace(this._value)
                        : System.String.Equals(this._value, other, System.StringComparison.OrdinalIgnoreCase);
                }
            
                public bool Equals(ResourceGroupName? other, IEqualityComparer<ResourceGroupName> comparer)
                {
                    if (other == null) return false;
                    return comparer.Equals(this, other.Value);
                }
            
                public bool Equals(System.String? primitive, StringComparer comparer)
                {
                    return comparer.Equals(this.Value, primitive);
                }
            
                /// <inheritdoc />
                public override int GetHashCode() {
                    if (!IsInitialized()) return 0;
                    return StringComparer.OrdinalIgnoreCase.GetHashCode(this._value);
                }

                
                public int CompareTo(ResourceGroupName other) => this.Value.CompareTo(other.Value);

                public int CompareTo(System.String? other) => this.Value.CompareTo(other);
            
                public int CompareTo(object? other)
                {
                    if (other == null)
                        return 1;
                    if (other is ResourceGroupName other1)
                        return this.CompareTo(other1);
                    if (other is System.String v)
                        return this.CompareTo(v);
                    throw new System.ArgumentException(
                        "Cannot compare to object as it is not of type ResourceGroupName",
                        nameof(other)
                    );
                }


                
                /// <summary>
                ///     An implicit conversion from <see cref="ResourceGroupName" /> to <see cref="System.String" />.
                /// </summary>
                /// <param name="id">The value to convert.</param>
                /// <returns>The System.String representation of the value object.</returns>
                public static implicit operator System.String(ResourceGroupName id)
                {
                    return id.Value;
                }
            
                /// <summary>
                ///     An explicit conversion from <see cref="System.String" /> to <see cref="ResourceGroupName" />.
                /// </summary>
                /// <param name="value">The value to convert.</param>
                /// <returns>The <see cref="ResourceGroupName" /> instance created from the input value.</returns>
                public static explicit operator ResourceGroupName(System.String value)
                {
                    return ResourceGroupName.From(value);
                }

                /// <inheritdoc />
                public override string ToString()
                {
                    return Value.ToString();
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
        