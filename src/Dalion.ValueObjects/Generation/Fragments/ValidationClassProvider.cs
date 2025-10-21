namespace Dalion.ValueObjects.Generation.Fragments;

internal class ValidationClassProvider : IFragmentProvider
{
    public string? ProvideFragment(AttributeConfiguration config, GenerationTarget target)
    {
        return @"
        private class Validation
        {
            public static readonly Validation Ok = new(string.Empty);
            private readonly bool _isSuccess;
        
            private Validation(string reason)
            {
                ErrorMessage = reason;
                _isSuccess = string.IsNullOrEmpty(reason);
            }
        
            public string ErrorMessage { get; }
            public bool IsSuccess => _isSuccess;
        
            public System.Collections.Generic.Dictionary<object, object>? Data { get; private set; }
        
            public static Validation Invalid(string reason = """")
            {
                if (string.IsNullOrEmpty(reason))
                {
                    return new Validation(""[none provided]"");
                }
        
                return new Validation(reason);
            }
        
            public Validation WithData(object key, object value)
            {
                Data ??= new System.Collections.Generic.Dictionary<object, object>();
                Data[key] = value;
                return this;
            }
        }".Trim();
    }
}