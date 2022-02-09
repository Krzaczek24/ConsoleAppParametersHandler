using System;

namespace ConsoleAppParametersHandler
{
    public class ParameterDefinition
    {
        public char Code { get; }

        public string Description { get; }

        public bool Found { get; set; }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                Validate(value);
                _value = ModifyValue(value);
            }
        }

        private Func<bool> _conditionalRequirement;
        public bool ConditionalRequirement => _conditionalRequirement != null ? _conditionalRequirement() : false;

        private bool _required;
        public bool Required => _required || ConditionalRequirement;

        private bool _valueExpected;
        public bool ValueExpected => _valueExpected || Required;

        public ParameterDefinition(char code, string description, bool required = false, bool valueExpected = false, Func<bool> conditionalRquirement = null)
        {
            if (char.IsWhiteSpace(code))
            {
                throw new ArgumentException($"Code character cannot be a white space character. Passed [{code}].");
            }

            if (required && !valueExpected)
            {
                throw new ArgumentException($"Parameter without value expectation cannot be required. Code [{code}].");
            }

            _required = required;
            _conditionalRequirement = conditionalRquirement;
            _valueExpected = valueExpected;
            Code = char.ToLower(code);
            Description = description;
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Value for '-{Code}' parameter cannot be empty.");
            if (value.StartsWith(ParametersHelper.ARG_PREFIX))
                throw new ArgumentException($"Value for '-{Code}' parameter cannot starts with '{ParametersHelper.ARG_PREFIX}'.");
            if (!ValidateValue(value))
                throw new ArgumentException($"Invalid value for '-{Code}' parameter, passed [{value}].");
        }

        protected virtual bool ValidateValue(string value)
        {            
            return true;
        }

        protected virtual string ModifyValue(string value)
        {
            return value;
        }
    }
}
